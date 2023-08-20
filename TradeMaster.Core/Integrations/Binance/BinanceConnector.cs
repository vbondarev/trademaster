using System.Text.Json;
using Binance.Common;
using Microsoft.Extensions.Logging;
using TradeMaster.Core.Integrations.Binance.Common.Json;
using TradeMaster.Core.Integrations.Binance.Exceptions;
using TradeMaster.Core.Integrations.Binance.Requests;
using TradeMaster.Core.Integrations.Binance.Responses;

namespace TradeMaster.Core.Integrations.Binance;

internal class BinanceConnector : IBinanceConnector
{
    private readonly BinanceApiAdapter _adapter;
    private readonly ILogger<BinanceConnector> _logger;

    public BinanceConnector(BinanceApiAdapter adapter, ILogger<BinanceConnector> logger)
    {
        _adapter = adapter;
        _logger = logger;
    }

    /// <summary>
    /// GET /sapi/v1/system/status
    /// </summary>
    public async Task<SystemStatusResponse> GetSystemStatus()
    {
        var wallet = _adapter.GetWallet();
        var response = await wallet.SystemStatus();
        
        return Json.Deserialize<SystemStatusResponse>(response)!;
    }

    /// <summary>
    /// POST /api/v3/order (HMAC SHA256)
    /// </summary>
    public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest request)
    {
        return await ExecuteRequest(async () =>
        {
            var spotAccountTrade = _adapter.GetSpotAccountTrade();
            var response = await spotAccountTrade.NewOrder(
                request.CoinsPair, 
                request.Side, 
                request.OrderType, 
                newClientOrderId: request.ClientOrderId,
                quantity: request.Quantity, 
                price: request.Price,
                timeInForce: request.TimeInForce);
        
            return Json.Deserialize<BuyOrderResponse>(response)!;
        });
    }

    /// <summary>
    /// POST /api/v3/order (HMAC SHA256)
    /// </summary>
    public async Task<SellOrderResponse> CreateSellLimitOrder(SellLimitOrderRequest request)
    {
        return await ExecuteRequest(async () =>
        {
            var spotAccountTrade = _adapter.GetSpotAccountTrade();
            var response = await spotAccountTrade.NewOrder(
                request.CoinsPair, 
                request.Side, 
                request.OrderType, 
                quantity: request.Quantity, 
                price: request.Price,
                timeInForce: request.TimeInForce);
        
            return Json.Deserialize<SellOrderResponse>(response)!;
        });
    }
    
    /// <summary>
    /// POST /api/v3/order (HMAC SHA256)
    /// </summary>
    public async Task<SellOrderResponse> CreateSellStopLossLimitOrder(SellStopLossLimitOrderRequest request)
    {
        return await ExecuteRequest(async () =>
        {
            var spotAccountTrade = _adapter.GetSpotAccountTrade();
            var response = await spotAccountTrade.NewOrder(
                request.CoinsPair,
                request.Side,
                request.OrderType,
                quantity: request.Quantity,
                price: request.Price,
                stopPrice: request.StopLimitPrice,
                timeInForce: request.TimeInForce);

            return Json.Deserialize<SellOrderResponse>(response)!;
        });
    }

    /// <summary>
    /// GET /api/v3/order (HMAC SHA256)
    /// </summary>
    public async Task<QueryOrderResponse> QueryOrder(QueryOrderRequest request)
    {
        return await ExecuteRequest(async () =>
        {
            var spotAccountTrade = _adapter.GetSpotAccountTrade();
            var response = await spotAccountTrade.QueryOrder(request.CoinsPair, orderId: request.OrderId);
        
            return Json.Deserialize<QueryOrderResponse>(response)!;
        });
    }
    
    /// <summary>
    /// GET /api/v3/myTrades (HMAC SHA256)
    /// </summary>
    public async Task<IEnumerable<TradeListResponse>> GetAccountTradeList(TradeListRequest request)
    {
        return await ExecuteRequest(async () =>
        {
            var spotAccountTrade = _adapter.GetSpotAccountTrade();
            var response = await spotAccountTrade.AccountTradeList(request.CoinsPair, orderId: request.OrderId);
        
            return Json.Deserialize<IEnumerable<TradeListResponse>>(response)!;
        });
    }

    /// <summary>
    /// GET /api/v3/klines
    /// </summary>
    public async Task<string[][]> GetCandlestickData(CandlestickDataRequest request)
    {
        return await ExecuteRequest(async () =>
        {
            var market = _adapter.GetMarket();
            var response = await market.KlineCandlestickData(request.CoinsPair, request.Interval, request.StartTime, request.EndTime);
            var candlesticks = Json.Deserialize<IEnumerable<object[]>>(response)!;

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
        });
    }
    
    /// <summary>
    /// GET /api/v3/ticker/price
    /// </summary>
    public async Task<SymbolPriceTickerResponse> GetSymbolPriceTicker(LastPriceRequest request)
    {
        return await ExecuteRequest(async () =>
        {
            var market = _adapter.GetMarket();
            var response = await market.SymbolPriceTicker(request.CoinsPair);
        
            return Json.Deserialize<SymbolPriceTickerResponse>(response)!;
        });
    }
    
    /// <summary>
    /// GET /api/v3/account (HMAC SHA256)
    /// </summary>
    public async Task<AccountInformationResponse> GetAccountInformation()
    {
        return await ExecuteRequest(async () =>
        {
            var spotAccountTrade = _adapter.GetSpotAccountTrade();
            var response = await spotAccountTrade.AccountInformation();

            return Json.Deserialize<AccountInformationResponse>(response)!;
        });
    }

    /// <summary>
    /// GET /api/v3/openOrders (HMAC SHA256)
    /// </summary>
    public async Task<IEnumerable<OpenOrderResponse>> GetOpenOrders(OpenOrdersRequest request)
    {
        return await ExecuteRequest(async () =>
        {
            var spotAccountTrade = _adapter.GetSpotAccountTrade();
            var response = await spotAccountTrade.CurrentOpenOrders(request.CoinsPair);
        
            return Json.Deserialize<IEnumerable<OpenOrderResponse>>(response)!;
        });
    }

    public async Task<IEnumerable<CancelOrderResponse>> CancelAllOpenOrders(CancelAllOpenOrdersRequest request)
    {
        return await ExecuteRequest(async () =>
        {
            var spotAccountTrade = _adapter.GetSpotAccountTrade();
            var response = await spotAccountTrade.CancelAllOpenOrdersOnASymbol(request.CoinsPair);
        
            return Json.Deserialize<IEnumerable<CancelOrderResponse>>(response)!;
        });
    }

    /// <summary>
    /// DELETE /api/v3/order (HMAC SHA256)
    /// </summary>
    public async Task<CancelOrderResponse> CancelOrder(CancelOrderRequest request)
    {
        return await ExecuteRequest(async () =>
        {
            var spotAccountTrade = _adapter.GetSpotAccountTrade();
            var response = await spotAccountTrade.CancelOrder(request.CoinsPair, orderId: request.OrderId);
            return Json.Deserialize<CancelOrderResponse>(response)!;
        });
    }
    
    private async Task<T> ExecuteRequest<T>(Func<Task<T>> func)
    {
        try
        {
            return await func();
        }
        catch (BinanceClientException e)
        {
            _logger.LogError(e, "Code: {Code}, Message: {Message}", e.Code, e.Message);
            throw new BinanceConnectorException(e.Code, e.Message, e);
        }
    }
}
