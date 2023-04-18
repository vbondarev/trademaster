using System.Text.Json;
using Binance.Common;
using Binance.Spot;
using Microsoft.Extensions.Options;
using TradeMaster.Binance.Requests;
using TradeMaster.Binance.Responses;
using TradeMaster.Enums;
using TradeMaster.Exceptions;
using TradeMaster.Options;

namespace TradeMaster.Binance;

internal class BinanceConnector : IBinanceConnector
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly BinanceOptions _options;

    public BinanceConnector(IHttpClientFactory httpFactory, IOptions<BinanceOptions> options)
    {
        _httpFactory = httpFactory;
        _options = options.Value;
    }

    /// <summary>
    /// GET /sapi/v1/system/status
    /// </summary>
    /// <returns></returns>
    public async Task<SystemStatusResponse> GetSystemStatus()
    {
        using var httpClient = _httpFactory.CreateClient();

        var wallet = new Wallet(httpClient);
        var response = await wallet.SystemStatus();
        var status = JsonSerializer.Deserialize<SystemStatusResponse>(response);

        return status ?? throw new BinanceConnectorException("Не удалось получить состояние системы");
    }

    /// <summary>
    /// Метод для покупки криптовалюты на Binance
    /// </summary>
    /// <param name="coin"></param>
    /// <param name="orderType"></param>
    /// <param name="price"></param>
    /// <param name="amount"></param>
    /// <returns>Возвращает количество купленных монет</returns>
    public decimal BuyCoins(Coin coin, OrderTypes orderType, decimal price, decimal amount)
    {
        return 0;
    }

    /// <summary>
    /// Метод для продажи криптовалюты на Binance
    /// </summary>
    public bool CellCoins(Coin coin, OrderTypes orderType, decimal price, decimal amount)
    {
        return true;
    }
    
    /// <summary>
    /// GET /api/v3/klines
    /// </summary>
    /// <returns></returns>
    public async Task<string[][]> GetCandlestickData(CandlestickDataRequest request)
    {
        using var httpClient = _httpFactory.CreateClient();

        var market = new Market(httpClient);
        var response = await market.KlineCandlestickData(request.CoinsPair, request.Interval, request.StartTime, request.EndTime);
        var candlesticks = JsonSerializer.Deserialize<object[][]>(response);

        if (candlesticks == null)
            throw new BinanceConnectorException(
                $"Не удалось получить свечи по торговой паре {request.CoinsPair} за интервал {request.StartTime}-{request.EndTime}");

        var data = new List<string[]>();
        foreach (var candlestick in candlesticks)
        {
            var elements = candlestick
                .Cast<JsonElement>()
                .Select(jsonElement => jsonElement.ToString())
                .ToArray();

            data.Add(elements);
        }

        return data.ToArray();
    }
    
    /// <summary>
    /// GET /api/v3/ticker/price
    /// </summary>
    /// <returns></returns>
    public async Task<SymbolPriceTickerResponse> GetSymbolPriceTicker(LastPriceRequest request)
    {
        using var httpClient = _httpFactory.CreateClient();
     
        var market = new Market(httpClient);
        var response = await market.SymbolPriceTicker(request.CoinsPair);
        var price = JsonSerializer.Deserialize<SymbolPriceTickerResponse>(response);

        return price ?? throw new BinanceConnectorException(
            $"Не удалось получить актуальную стоимость торговой пары {request.CoinsPair}");
    }
    
    /// <summary>
    /// GET /api/v3/account (HMAC SHA256)
    /// </summary>
    /// <returns></returns>
    public async Task<AccountInformationResponse> GetAccountInformation()
    {
        using var httpClient = _httpFactory.CreateClient();

        var signature = new BinanceHmac(_options.SecretKey); 
        var account = new SpotAccountTrade(httpClient, signature, apiKey:_options.ApiKey, baseUrl:_options.BaseUri);
        var response = await account.AccountInformation();
        var accountInfo = JsonSerializer.Deserialize<AccountInformationResponse>(response);

        return accountInfo ?? throw new BinanceConnectorException("Не удалось получить данные о спотовом аккаунте пользователя");
    }

    /// <summary>
    /// Проверка существования стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="coin"></param>
    /// <returns></returns>
    public bool CellStopLimitOrderCheck(Coin coin)
    {
        return true;
    }

    /// <summary>
    /// Удаление существующего стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="btc"></param>
    /// <returns></returns>
    public bool DeleteCellStopLimitOrder(Coin btc)
    {
        return true;
    }
}
