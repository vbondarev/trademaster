using System;
using TradeMaster.Models;

namespace TradeMaster.Binance;

public class BinanceConnector
{
    /// <summary>
    /// Метод для покупки криптовалюты на Binance
    /// </summary>
    public bool BuyCoins(Coins coin, OrderTypes orderType, decimal price)
    {
        return true;
    }

    /// <summary>
    /// Метод для продажи криптовалюты на Binance
    /// </summary>
    public bool CellCoins(Coins coin, OrderTypes orderType, decimal price)
    {
        return true;
    }

    /// <summary>
    /// Получить максимальную стоимость в определенном интервале
    /// </summary>
    /// <returns></returns>
    public decimal GetMaxPrice(Coins coin, DateTime startDateTime, DateTime endDateTime)
    {
        return 0;
    }
    
    /// <summary>
    /// Получить минимальную стоимость в определенном интервале
    /// </summary>
    /// <returns></returns>
    public decimal GetMinPrice(Coins coin, DateTime startDateTime, DateTime endDateTime)
    {
        return 0;
    }

    /// <summary>
    /// Получить актуальную стоимость и время последней стоимости монеты
    /// </summary>
    /// <returns></returns>
    public CoinPriceModel GetLastCoinPrice(Coins coin)
    {
        return new CoinPriceModel()
        {
            Coin = coin,
            Price = 0,
            Time = DateTime.Now
        };
    }
    
    
}
