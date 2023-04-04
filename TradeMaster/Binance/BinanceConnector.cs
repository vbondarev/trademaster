using Binance.Spot;
using TradeMaster.Binance.Responses;

namespace TradeMaster.Binance;

public class BinanceConnector
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
    public async Task<bool> GetSystemStatus()
    {
        using var httpClient = _httpFactory.CreateClient();

        var wallet = new Wallet(httpClient);
        var response = await wallet.SystemStatus();
        var systemStatus = System.Text.Json.JsonSerializer.Deserialize<SystemStatusResponse>(response);

        return systemStatus!.Status == 0;
    }

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
