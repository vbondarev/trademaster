using System.Text.Json;
using Binance.Common;
using Binance.Spot;
using Microsoft.Extensions.Options;
using TradeMaster.Binance.Common;
using TradeMaster.Binance.Enums;
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
    public async Task<SystemStatusResponse> GetSystemStatus()
    {
        using var httpClient = _httpFactory.CreateClient();

        var wallet = new Wallet(httpClient);
        var response = await wallet.SystemStatus();
        var status = Json.Deserialize<SystemStatusResponse>(response);

        return status ?? throw new BinanceConnectorException("Не удалось получить состояние системы");
    }

    /// <summary>
    /// POST /api/v3/order (HMAC SHA256)
    /// </summary>
    public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest request)
    {
        using var httpClient = _httpFactory.CreateClient();

        var signature = new BinanceHmac(_options.SecretKey);
        var spotAccountTrade = new SpotAccountTrade(httpClient, signature, apiKey:_options.ApiKey, baseUrl:_options.BaseUri);
        var response = await spotAccountTrade.NewOrder(
            request.CoinsPair, 
            request.Side, 
            request.OrderType, 
            quantity: request.Quantity, 
            price: request.Price,
            timeInForce: request.TimeInForce);
        var buyOrder = Json.Deserialize<BuyOrderResponse>(response);
        
        return buyOrder ?? throw new BinanceConnectorException($"Не удалось создать ордер на покупку {request.CoinsPair}");
    }

    /// <summary>
    /// GET /api/v3/order (HMAC SHA256)
    /// </summary>
    public async Task<QueryOrderResponse> QueryOrder(QueryOrderRequest request)
    {
        using var httpClient = _httpFactory.CreateClient();
        
        var signature = new BinanceHmac(_options.SecretKey);
        var spotAccountTrade = new SpotAccountTrade(httpClient, signature, apiKey:_options.ApiKey, baseUrl:_options.BaseUri);
        var response = await spotAccountTrade.QueryOrder(request.CoinsPair, orderId: request.OrderId);
        var order = Json.Deserialize<QueryOrderResponse>(response);

        return order ??
               throw new BinanceConnectorException($"Не удалось получить информацию по идентификатору ордера {request.OrderId}");
    }

    /// <summary>
    /// Метод для продажи криптовалюты на Binance
    /// </summary>
    public bool CellCoins(Coin coin, OrderType orderType, decimal price, decimal amount)
    {
        return true;
    }

    
    /// <summary>
    /// GET /api/v3/myTrades (HMAC SHA256)
    /// </summary>
    public async Task<IEnumerable<TradeListResponse>> GetAccountTradeList(TradeListRequest request)
    {
        using var httpClient = _httpFactory.CreateClient();

        var signature = new BinanceHmac(_options.SecretKey);
        var spotAccountTrade = new SpotAccountTrade(httpClient, signature, apiKey:_options.ApiKey, baseUrl:_options.BaseUri);
        var response = await spotAccountTrade.AccountTradeList(request.CoinsPair, orderId: request.OrderId);
        var tradeList = Json.Deserialize<IEnumerable<TradeListResponse>>(response);

        return tradeList ?? throw new BinanceConnectorException("Не удалось получить список сделок пользователя");
    }

    /// <summary>
    /// GET /api/v3/klines
    /// </summary>
    public async Task<string[][]> GetCandlestickData(CandlestickDataRequest request)
    {
        using var httpClient = _httpFactory.CreateClient();

        var market = new Market(httpClient);
        var response = await market.KlineCandlestickData(request.CoinsPair, request.Interval, request.StartTime, request.EndTime);
        var candlesticks = Json.Deserialize<object[][]>(response);

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
    public async Task<SymbolPriceTickerResponse> GetSymbolPriceTicker(LastPriceRequest request)
    {
        using var httpClient = _httpFactory.CreateClient();
     
        var market = new Market(httpClient);
        var response = await market.SymbolPriceTicker(request.CoinsPair);
        var price = Json.Deserialize<SymbolPriceTickerResponse>(response);

        return price ?? throw new BinanceConnectorException(
            $"Не удалось получить актуальную стоимость торговой пары {request.CoinsPair}");
    }
    
    /// <summary>
    /// GET /api/v3/account (HMAC SHA256)
    /// </summary>
    public async Task<AccountInformationResponse> GetAccountInformation()
    {
        using var httpClient = _httpFactory.CreateClient();

        var signature = new BinanceHmac(_options.SecretKey); 
        var spotAccountTrade = new SpotAccountTrade(httpClient, signature, apiKey:_options.ApiKey, baseUrl:_options.BaseUri);
        var response = await spotAccountTrade.AccountInformation();
        var account = Json.Deserialize<AccountInformationResponse>(response);

        return account ?? throw new BinanceConnectorException("Не удалось получить данные о спотовом аккаунте пользователя");
    }

    /// <summary>
    /// Проверка существования стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="coin"></param>
    public bool CellStopLimitOrderCheck(Coin coin)
    {
        return true;
    }

    /// <summary>
    /// Удаление существующего стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="btc"></param>
    public bool DeleteCellStopLimitOrder(Coin btc)
    {
        return true;
    }
}
