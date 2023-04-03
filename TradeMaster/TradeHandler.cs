using TradeMaster.Binance;
using TradeMaster.Models;

namespace TradeMaster;

public class TradeHandler
{
    HistoryPriceModel GeneratePriceHistory()
    {
        return new HistoryPriceModel();
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
                var priceHistory = GeneratePriceHistory();
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
