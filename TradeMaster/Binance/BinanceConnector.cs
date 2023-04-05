using System.Text.Json;
using Binance.Spot;
using TradeMaster.Binance.Responses;
using TradeMaster.Models;

namespace TradeMaster.Binance;

internal class BinanceConnector : IBinanceConnector
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
    /// <param name="coin"></param>
    /// <param name="orderType"></param>
    /// <param name="price"></param>
    /// <param name="amount"></param>
    /// <returns>Возвращает количество купленных монет</returns>
    public decimal BuyCoins(Coins coin, OrderTypes orderType, decimal price, decimal amount)
    {
        return 0;
    }

    /// <summary>
    /// Метод для продажи криптовалюты на Binance
    /// </summary>
    public bool CellCoins(Coins coin, OrderTypes orderType, decimal price, decimal amount)
    {
        return true;
    }
    
    /// <summary>
    /// Получить максимальную стоимость в определенном интервале
    /// </summary>
    /// <returns></returns>
    public decimal GetMaxPrice(Coins baseCoin, Coins quotedCoin, DateTime startDateTime, DateTime endDateTime)
    {
        return 0;
    }
    
    /// <summary>
    /// Получить минимальную стоимость в определенном интервале
    /// </summary>
    /// <returns></returns>
    public decimal GetMinPrice(Coins baseCoin, Coins quotedCoin, DateTime startDateTime, DateTime endDateTime)
    {
        return 0;
    }

    /// <summary>
    /// Получить актуальную стоимость и время последней стоимости монеты
    /// </summary>
    /// <returns></returns>
    public CoinPriceModel GetLastCoinPrice(Coins baseCoin, Coins quotedCoin)
    {
        return new CoinPriceModel()
        {
            Coin = quotedCoin,
            Price = 0,
            Time = DateTime.Now
        };
    }
    
    /// <summary>
    /// Получить общую сумму монет на спотовом аккаунте
    /// </summary>
    /// <returns></returns>
    public decimal GetTotalAmount(Coins coin)
    {
        return 0;
    }

    /// <summary>
    /// Проверка существования стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="coin"></param>
    /// <returns></returns>
    public bool CellStopLimitOrderCheck(Coins coin)
    {
        return true;
    }

    /// <summary>
    /// Удаление существующего стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="btc"></param>
    /// <returns></returns>
    public bool DeleteCellStopLimitOrder(Coins btc)
    {
        return true;
    }
}
