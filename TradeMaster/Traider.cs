using System;
using System.Linq;
using TradeMaster.Models;

namespace TradeMaster;

public class Traider
{
    public int TestMethod( int a, int b)
    {
        return a + b;
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
}