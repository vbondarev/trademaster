using TradeMaster.Binance;
using TradeMaster.Enums;
using TradeMaster.Models;

namespace TradeMaster.Handlers;

internal class TradeHandler
{
    private readonly IBinanceProvider _binanceProvider;

    public TradeHandler(IBinanceProvider binanceProvider)
    {
        _binanceProvider = binanceProvider;
    }

    /// <summary>
    /// Метод формирования истории изменений цены в заданом диапазоне
    /// </summary>
    /// <returns></returns>
    public async Task<HistoryPriceModel> GeneratePriceHistory(Coin baseCoin, Coin quotedCoin, Interval interval, int intervalCount)
    {
        //Необходимо для интервала на основе количества интервалов рассчитать время, на которое необходимо уменьшить
        //дату последней цены
        var intervalCountValue = interval switch
        {
            Interval.Minute => TimeSpan.FromMinutes(1),
            Interval.QuarterHour => TimeSpan.FromMinutes(15),
            Interval.HalfHour => TimeSpan.FromMinutes(30),
            Interval.Hour => TimeSpan.FromHours(1),
            Interval.TwoHour => TimeSpan.FromHours(2),
            Interval.FourHour => TimeSpan.FromHours(4),
            Interval.Day => TimeSpan.FromDays(1),
            Interval.Week => TimeSpan.FromDays(7),
            Interval.Month => TimeSpan.FromDays(30),
            _ => TimeSpan.Zero
        };

        var history = new HistoryPriceModel { Interval = interval, IntervalCount = intervalCount };

        //получаем время последнюй цены монеты
        var lastCoinPrice = await _binanceProvider.GetLastPrice(baseCoin, quotedCoin);
        var currentDateTime = lastCoinPrice.Time;

        var intervalList = new List<CostLimits>();

        //формирование списка параметров стоимости монеты в заданых интервалах
        DateTimeOffset startDateTime;
        DateTimeOffset endDateTime;
        decimal upperCostBound;
        decimal lowerCostBound;
        RateTypes rateType;
        var rate = 0d;
        
        while (intervalCount != 0)
        {
            startDateTime = currentDateTime - (intervalCountValue * intervalCount);
            endDateTime = intervalCount == 1
                ? currentDateTime
                : currentDateTime - (intervalCountValue * (intervalCount - 1));
            upperCostBound = await _binanceProvider.GetMaxPrice(baseCoin, quotedCoin, interval, startDateTime, endDateTime);
            lowerCostBound = await _binanceProvider.GetMinPrice(baseCoin, quotedCoin, interval, startDateTime, endDateTime);
            rateType = upperCostBound == lowerCostBound ? RateTypes.Neutral :
                upperCostBound > lowerCostBound ? RateTypes.Negative : RateTypes.Positive;
            
            switch (rateType)
            {
                case RateTypes.Neutral:
                    rate = 0;
                    break;
                case RateTypes.Negative:
                    rate = (double)(100 - (lowerCostBound / (upperCostBound / 100)));
                    break;
                case RateTypes.Positive:
                    rate = (double)(100 - (upperCostBound / (lowerCostBound / 100)));
                    break;
            }
            
            intervalList.Add( new CostLimits()
            {
                IntervalNumber = intervalCount,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                UpperCostBound = upperCostBound,
                LowerCostBound = lowerCostBound,
                RateType = rateType,
                Rate = rate
            });
            
            intervalCount--;
        }

        history.CostLimits = intervalList;

        return history;
    }
    
    public decimal CalculateBuyOrderPrice(Trend trend, HistoryPriceModel historyPriceModel)
    {
        return trend switch
        {
            Trend.Bear => CalculateBearBuyOrderPrice(historyPriceModel),
            Trend.Bull => CalculateBullBuyOrderPrice(historyPriceModel),
            Trend.Flat => CalculateFlatBuyOrderPrice(historyPriceModel),
            _ => 0
        };
    }

    private decimal CalculateFlatBuyOrderPrice(HistoryPriceModel historyPriceModel)
    {
        return 0;
    }

    private decimal CalculateBullBuyOrderPrice(HistoryPriceModel historyPriceModel)
    {
        return 0;
    }

    private decimal CalculateBearBuyOrderPrice(HistoryPriceModel historyPriceModel)
    {
        //Определяем усредненное значение процентных процентных коэффициентов во временных интервалах
        var averageRate = historyPriceModel.CostLimits.Average(cl => cl.Rate);
        
        //Определяем процентный коэффициент разницы между последней нижней стоимостью в 15-минутном интервале и ценой ордера на покупку
        var resultEstimate = averageRate / historyPriceModel.IntervalCount;
        
        //Определяем поледнюю нижнюю стоимость в интервалах
        //var lastLowerPrice = historyPriceModel.CostLimits.Max(cl => cl.LowerCostBound);
        var lastLowerPrice = historyPriceModel.CostLimits.First(cl => cl.IntervalNumber == 1).LowerCostBound;
        
        //Определяем стоимость ордера на покупку
        var buyOrderPrice = lastLowerPrice - (lastLowerPrice / 100 * (decimal)resultEstimate);

        return buyOrderPrice;
    }

    /// <summary>
    /// Метод рассчета суммы ордера
    /// </summary>
    /// <returns></returns>
    public async Task<decimal> CalculateOrderAmount(Coin coin)
    {
        //Предположим что пока будем закупать на все средства на спотовом аккаунте
        //и также что покупать монеты будем за USDT
        return await _binanceProvider.GetTotalAmount(coin);
    }

    public decimal CalculateCellOrderPrice(Coin baseCoin, Coin quotedCoin, Trend trend, HistoryPriceModel historyPriceModel, decimal buyPrice)
    {
        return trend switch
        {
            Trend.Bear => CalculateBearCellOrderPrice(baseCoin, quotedCoin, historyPriceModel, buyPrice),
            Trend.Bull => CalculateBullCellOrderPrice(baseCoin, quotedCoin, historyPriceModel, buyPrice),
            Trend.Flat => CalculateFlatCellOrderPrice(baseCoin, quotedCoin, historyPriceModel, buyPrice),
            _ => 0
        };
    }
    
    private decimal CalculateFlatCellOrderPrice(Coin baseCoin, Coin quotedCoin, HistoryPriceModel historyPriceModel, decimal buyPrice)
    {
        return 0;
    }

    private decimal CalculateBullCellOrderPrice(Coin baseCoin, Coin quotedCoin, HistoryPriceModel historyPriceModel, decimal buyPrice)
    {
        return 0;
    }

    private decimal CalculateBearCellOrderPrice(Coin baseCoin, Coin quotedCoin, HistoryPriceModel historyPriceModel, decimal buyPrice)
    {
        decimal buyOrderPrice;
        //Определяем усредненное значение процентных процентных коэффициентов во временных интервалах
        var averageRate = historyPriceModel.CostLimits.Average(cl => cl.Rate);
        //Определяем процентный коэффициент разницы между последней нижней стоимостью в 15-минутном интервале и ценой ордера на покупку
        var resultEstimate = averageRate / historyPriceModel.IntervalCount;
        
        //Текущая стоимость котируемой монеты
        var lastCoinPrice = _binanceProvider.GetLastPrice(baseCoin, quotedCoin).Result.Price;

        //Если цена по которой купили котируемую монету ниже, чем текущая стоимость монеты, то рассчет цены продажи будем вести от текущей стоимости
        if (buyPrice < lastCoinPrice)
        {
            //Определяем стоимость ордера на покупку
            buyOrderPrice = lastCoinPrice + (lastCoinPrice / 100 * (decimal)resultEstimate);
        }
        else
        {
            buyOrderPrice = buyPrice + (buyPrice / 100 * (decimal)resultEstimate);
        }

        return buyOrderPrice;
    }

}
