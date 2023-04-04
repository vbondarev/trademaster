using System.Text.Json;
using Binance.Spot;
using TradeMaster.Binance.Responses;
using TradeMaster.Models;

namespace TradeMaster.Binance;

internal class BinanceConnector
{
    private readonly IHttpClientFactory _httpFactory;

    public BinanceConnector(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    /// <summary>
    /// Запрашиваем статус системы
    /// </summary>
    /// <returns></returns>
    public async Task<SystemStatusResponse> GetSystemStatus()
    {
        using var httpClient = _httpFactory.CreateClient();

        var wallet = new Wallet(httpClient);
        var response = await wallet.SystemStatus();
        return JsonSerializer.Deserialize<SystemStatusResponse>(response)!;
    }

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
