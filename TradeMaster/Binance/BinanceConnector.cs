namespace TradeMaster.Binance;

public class BinanceConnector
{
    /// <summary>
    /// Метод для покупки криптовалюты на Binance
    /// </summary>
    public bool BuyCoins(Coins coin)
    {
        return true;
    }

    /// <summary>
    /// Метод для продажи криптовалюты на Binance
    /// </summary>
    public bool CellCoins(Coins coin)
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
}