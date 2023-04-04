using System;
using System.Collections.Generic;
using TradeMaster.Binance;
using TradeMaster.Models;

namespace TradeMaster;

public class TradeHandler
{
    /// <summary>
    /// Метод формирования истории изменений цены в заданом диапазоне
    /// </summary>
    /// <returns></returns>
    HistoryPriceModel GeneratePriceHistory(Coins coin, Interval interval, int intervalCount)
    {
        var binanceConnector = new BinanceConnector();

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
            Interval.Year => TimeSpan.FromDays(365),
            _ => TimeSpan.Zero
        };

        var history = new HistoryPriceModel();
        history.Interval = interval;
        history.IntervalCount = intervalCount;

        //получаем время последнюй цены монеты
        var lastCoinPrice = binanceConnector.GetLastCoinPrice(coin);
        var currentDateTime = lastCoinPrice.Time;

        var intervalList = new List<CostLimits>();

        //формирование списка параметров стоимости монеты в заданых интервалах
        DateTime startDateTime;
        DateTime endDateTime;
        decimal upperCostBound;
        decimal lowerCostBound;
        RateTypes rateType;
        var rate = double.MinValue;
        
        while (intervalCount != 0)
        {
            startDateTime = currentDateTime - (intervalCountValue * intervalCount);
            endDateTime = intervalCount == 1
                ? currentDateTime
                : currentDateTime - (intervalCountValue * (intervalCount - 1));
            upperCostBound = binanceConnector.GetMaxPrice(coin, startDateTime, endDateTime);
            lowerCostBound = binanceConnector.GetMinPrice(coin, startDateTime, endDateTime);
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
    
    
    //Разработать функционал по временным интервалам покупки
    //Допустим будем производить следующую покупку через 30 секунд после продажи
    //На текущий момент работаем только с биткойном

    /// <summary>
    /// Начало торговли
    /// </summary>
    public void StartTrading()
    {
        var trendDefiner = new TrendHandler();
        var traider = new Traider();
        var binanceConnector = new BinanceConnector();
        
        //Определяем тренд
        var currentTrend = trendDefiner.DefineTrend();

        switch (currentTrend)
        {
            case Trend.Bear:
                //формируем историю изменений цены
                //пока что возьмем за образец сведения двухчасовой давности, но в дальнейшем
                //необходимо либо брать эту информацию из конфиг файлов, либо определять автоматически, что более приоритетно
                var priceHistory = GeneratePriceHistory(Coins.BTC, Interval.QuarterHour, 8);
                //формируем цену покупки
                var buyPrice = traider.CalculateBuyOrderPrice(Trend.Bear, priceHistory);
                //совершаем сделку через binanceConnector
                var result = binanceConnector.BuyCoins(Coins.BTC);
                
                
                
                break;
            case Trend.Bull:
                break;
            case Trend.Flat:
                break;
        }
    }
}
