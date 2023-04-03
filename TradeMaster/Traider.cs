using System;
using System.Linq;
using TradeMaster.Models;

namespace TradeMaster;

public class Traider
{
    public int TestMethod( int a, int b)
    {
        var arr = new double[] { 0.17, 0.07, 0.95, 0.35, 0.17, 0.17, 0.35 };
        double avg = arr.Average();
        Console.WriteLine("Average = "+avg);
        return a + b;
    }

    public double CalculateBuyOrderPrice(Trend trend, HistoryPriceModel historyPriceModel)
    {
        return trend switch
        {
            Trend.Bear => CalculateBearBuyOrderPrice(historyPriceModel),
            Trend.Bull => CalculateBullBuyOrderPrice(historyPriceModel),
            Trend.Flat => CalculateFlatBuyOrderPrice(historyPriceModel),
            _ => 0
        };
    }

    private double CalculateFlatBuyOrderPrice(HistoryPriceModel historyPriceModel)
    {
        return 0;
    }

    private double CalculateBullBuyOrderPrice(HistoryPriceModel historyPriceModel)
    {
        return 0;
    }

    private double CalculateBearBuyOrderPrice(HistoryPriceModel historyPriceModel)
    {
        //Определяем усредненное значение процентных процентных коэффициентов во временных интервалах
        var averageRate = historyPriceModel.CostLimits.Average(cl => cl.Rate);
        //Определяем процентный коэффициент разницы между последней нижней стоимостью в 15-минутном интервале и ценой ордера на покупку
        var resultEstimate = averageRate / historyPriceModel.IntervalCount;
        //Определяем поледнюю нижнюю стоимость в интервалах
        var lastLowerPrice = historyPriceModel.CostLimits.Max(cl => cl.LowerCostBound);
        //Определяем стоимость ордера на покупку
        var buyOrderPrice = lastLowerPrice - (lastLowerPrice - 1%);


        return 0;
    }
}