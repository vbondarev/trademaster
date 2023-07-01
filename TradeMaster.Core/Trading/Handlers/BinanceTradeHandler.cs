using Microsoft.Extensions.Logging;
using TradeMaster.Core.Integrations.Binance;
using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Models;

namespace TradeMaster.Core.Trading.Handlers;

internal class BinanceTradeHandler : IBinanceTradeHandler
{
    private readonly IBinanceProvider _binanceProvider;
    private readonly IBinanceConnector _binanceConnector;
    private readonly ILogger<BinanceTradeHandler> _logger;

    private long _currentBuyOrderId;
    private long _currentSellOrderId;

    public BinanceTradeHandler(IBinanceProvider binanceProvider, IBinanceConnector binanceConnector, ILogger<BinanceTradeHandler> logger)
    {
        _binanceProvider = binanceProvider;
        _binanceConnector = binanceConnector;
        _logger = logger;
    }

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

        //получаем время последней цены монеты
        var lastCoinPrice = await _binanceProvider.GetLastPrice(baseCoin, quotedCoin);
        var currentDateTime = lastCoinPrice.Time;

        var intervalList = new List<CostLimits>();

        //формирование списка параметров стоимости монеты в заданых интервалах
        DateTimeOffset startDateTime;
        DateTimeOffset endDateTime;
        decimal upperCostBound;
        decimal lowerCostBound;
        RateType rateType;
        var rate = 0d;
        
        while (intervalCount != 0)
        {
            startDateTime = currentDateTime - (intervalCountValue * intervalCount);
            endDateTime = intervalCount == 1
                ? currentDateTime
                : currentDateTime - (intervalCountValue * (intervalCount - 1));
            upperCostBound = await _binanceProvider.GetMaxPrice(baseCoin, quotedCoin, interval, startDateTime, endDateTime);
            lowerCostBound = await _binanceProvider.GetMinPrice(baseCoin, quotedCoin, interval, startDateTime, endDateTime);
            rateType = upperCostBound == lowerCostBound ? RateType.Neutral :
                upperCostBound > lowerCostBound ? RateType.Negative : RateType.Positive;
            
            switch (rateType)
            {
                case RateType.Neutral:
                    rate = 0;
                    break;
                case RateType.Negative:
                    rate = (double)(100 - (lowerCostBound / (upperCostBound / 100)));
                    break;
                case RateType.Positive:
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
    
    public decimal CalculateBuyOrderPrice(Trend trend, HistoryPriceModel historyPrice)
    {
        return trend switch
        {
            Trend.Bear => CalculateBearBuyOrderPrice(historyPrice),
            Trend.Bull => CalculateBullBuyOrderPrice(historyPrice),
            Trend.Flat => CalculateFlatBuyOrderPrice(historyPrice),
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
        //Определяем усредненное значение процентных коэффициентов во временных интервалах
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

    /// <remarks>
    /// Предположим, что пока будем закупать на все средства на спотовом аккаунте
    /// </remarks>
    public async Task<decimal> CalculateOrderAmount(Coin coin)
    {
        return await _binanceProvider.GetAccountBalance(coin);
    }

    public decimal CalculateSellOrderPrice(Coin baseCoin, Coin quotedCoin, Trend trend, HistoryPriceModel historyPriceModel, decimal price)
    {
        return trend switch
        {
            Trend.Bear => CalculateBearCellOrderPrice(baseCoin, quotedCoin, historyPriceModel, price),
            Trend.Bull => CalculateBullCellOrderPrice(baseCoin, quotedCoin, historyPriceModel, price),
            Trend.Flat => CalculateFlatCellOrderPrice(baseCoin, quotedCoin, historyPriceModel, price),
            _ => 0
        };
    }

    public async Task<OrderOperation> CreateBuyOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal quantity)
    {
        var account = await _binanceConnector.GetAccountInformation();
        var balance = account.Balances.Single(b => b.Asset == quotedCoin.ToString());
        var orderCost = price * quantity;
        if (balance.Free < orderCost)
        {
            _logger.LogWarning("На балансе недостаточно средств для покупки {Quantity} {BaseCoin} по цене {Price}", baseCoin, quantity, price);
            return new OrderOperation(0, 0, false);
        }
        
        var order = await _binanceProvider.CreateBuyLimitOrder(baseCoin, quotedCoin, price, quantity);
        
        _currentBuyOrderId = order.Id;

        return new OrderOperation(order.Id, order.ExecutedQty, true);
    }

    public async Task<OrderOperation> CancelBuyOrder(Coin baseCoin, Coin quotedCoin)
    {
        var order = await _binanceProvider.CancelOrder(baseCoin, quotedCoin, _currentBuyOrderId);
        return new OrderOperation(order.Id, order.ExecutedQty, true);
    }

    public async Task<OrderOperation> GetSellStopOrder(Coin baseCoin, Coin quotedCoin)
    {
        var order = await _binanceProvider.GetSellStopLossLimitOrder(baseCoin, quotedCoin, _currentSellOrderId);
        return order is null 
            ? new OrderOperation(0, 0, false) 
            : new OrderOperation(order.Id, order.ExecutedQty, true);
    }

    public async Task<OrderOperation> CancelSellStopOrder(Coin baseCoin, Coin quotedCoin)
    {
        var order = await _binanceProvider.CancelOrder(baseCoin, quotedCoin, _currentSellOrderId);

        return new OrderOperation(order.Id, order.ExecutedQty, true);
    }

    public async Task<OrderOperation> CreateSellOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal stopLimitSellPrice, decimal quantity)
    {
        var order = await _binanceProvider.CreateSellStopLossLimitOrder(baseCoin, quotedCoin, price, stopLimitSellPrice, quantity);
        
        return new OrderOperation(order.Id, order.ExecutedQty, true);
    }

    public async Task<OrderOperation> CreateSellStopOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal quantity)
    {
        var order = await _binanceProvider.CreateSellLimitOrder(baseCoin, quotedCoin, price, quantity);
        
        return new OrderOperation(order.Id, order.ExecutedQty, true);
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
