using System.Globalization;
using Microsoft.Extensions.Options;
using TradeMaster.Core.Integrations.Binance.Dtos;
using TradeMaster.Core.Integrations.Binance.Enums;
using TradeMaster.Core.Integrations.Binance.Exceptions;
using TradeMaster.Core.Integrations.Binance.Options;
using TradeMaster.Core.Integrations.Binance.Requests;
using TradeMaster.Core.Integrations.Binance.Responses;
using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Models;

namespace TradeMaster.Core.Integrations.Binance;

internal class BinanceProvider : IBinanceProvider
{
    private readonly IBinanceConnector _connector;
    private readonly TradeOptions _options;

    public BinanceProvider(IBinanceConnector connector, IOptions<TradeOptions> options)
    {
        _connector = connector;
        _options = options.Value;
    }

    public async Task<BinanceStatus> GetSystemStatus()
    {
        var status = await _connector.GetSystemStatus();

        return status.Status switch
        {
            0 => BinanceStatus.Normal,
            1 => BinanceStatus.Maintenance,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    public async Task<OrderInfo> CreateBuyLimitOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal quantity)
    {
        var buyOrderRequest = new BuyLimitOrderRequest(baseCoin, quotedCoin, price, quantity);
        var buyOrder = await _connector.CreateBuyOrder(buyOrderRequest);
        var orderQueryRequest = new QueryOrderRequest(baseCoin, quotedCoin, buyOrder.OrderId);

        QueryOrderResponse order;

        var orderExecutionTime = 1;
        do
        {
            await Task.Delay(TimeSpan.FromSeconds(orderExecutionTime));
            order = await _connector.QueryOrder(orderQueryRequest);

            orderExecutionTime *= 2;
        }
        while (order.Status != OrderStatus.FILLED || orderExecutionTime >= _options.BuyOrderLifeMaxTimeSeconds);
        
        return new OrderInfo(buyOrder.OrderId, order.ExecutedQty, buyOrder.Side, buyOrder.Status, order.Type);
    }

    public async Task<OrderInfo> CreateSellLimitOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal quantity)
    {
        var sellOrderRequest = new SellLimitOrderRequest(baseCoin, quotedCoin, price, quantity);
        var sellOrder = await _connector.CreateSellLimitOrder(sellOrderRequest);
        var orderQueryRequest = new QueryOrderRequest(baseCoin, quotedCoin, sellOrder.OrderId);

        QueryOrderResponse order;

        var orderExecutionTime = 1;
        do
        {
            await Task.Delay(TimeSpan.FromSeconds(orderExecutionTime));
            order = await _connector.QueryOrder(orderQueryRequest);

            orderExecutionTime *= 2;
        }
        while (order.Status != OrderStatus.FILLED || orderExecutionTime >= _options.SellOrderLifeMaxTimeSeconds);
        
        return new OrderInfo(sellOrder.OrderId, order.ExecutedQty, order.Side, order.Status, order.Type);
    }
    
    public async Task<OrderInfo> CreateSellStopLossLimitOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal stopLimitPrice, decimal quantity)
    {
        var sellOrderRequest = new SellStopLossLimitOrderRequest(baseCoin, quotedCoin, price, stopLimitPrice, quantity);
        var sellOrder = await _connector.CreateSellStopLossLimitOrder(sellOrderRequest);

        return new OrderInfo(sellOrder.OrderId, sellOrder.ExecutedQty, sellOrder.Side, sellOrder.Status, sellOrder.Type);
    }

    public async Task<OrderInfo> CancelOrder(Coin baseCoin, Coin quotedCoin, long orderId)
    {
        var request = new CancelOrderRequest(baseCoin, quotedCoin, orderId);
        var order = await _connector.CancelOrder(request);

        if (order.OrderId == orderId && order.Status == OrderStatus.CANCELED)
        {
            return new OrderInfo(order.OrderId, order.ExecutedQty, order.Side, order.Status, order.Type);
        }

        return new OrderInfo(0, 0, 0, 0, 0);
    }

    public async Task<decimal> GetMaxPrice(Coin baseCoin, Coin quotedCoin, Interval interval, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        var request = new CandlestickDataRequest(baseCoin, quotedCoin, interval, startTime, endTime);
        var candlesticks = await _connector.GetCandlestickData(request);
        var highestPrices = new List<decimal>(candlesticks.Length); 
        const int HighestPriceIndex = 2;
        
        foreach (var candlestick in candlesticks)
        {
            if (!decimal.TryParse(candlestick[HighestPriceIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out var highPrice))
            {
                throw new BinanceProviderException("Не удалось получить максимальную цену свечи");
            }

            highestPrices.Add(highPrice);
        }

        return highestPrices.Max();
    }

    public async Task<decimal> GetMinPrice(Coin baseCoin, Coin quotedCoin, Interval interval, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        var request = new CandlestickDataRequest(baseCoin, quotedCoin, interval, startTime, endTime);
        var candlesticks = await _connector.GetCandlestickData(request);
        var lowestPrices = new List<decimal>(candlesticks.Length); 
        const int LowestPriceIndex = 3;
        
        foreach (var candlestick in candlesticks)
        {
            if (!decimal.TryParse(candlestick[LowestPriceIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out var highPrice))
            {
                throw new BinanceProviderException("Не удалось получить минимальную цену свечи");
            }

            lowestPrices.Add(highPrice);
        }

        return lowestPrices.Min();
    }

    public async Task<CoinPriceModel> GetLastPrice(Coin baseCoin, Coin quotedCoin)
    {
        var request = new LastPriceRequest(baseCoin, quotedCoin);
        var response = await _connector.GetSymbolPriceTicker(request);

        if (decimal.TryParse(response.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
        {
            return new CoinPriceModel(baseCoin, quotedCoin) { Price = price, Time = DateTimeOffset.Now };    
        }

        throw new BinanceProviderException($"Не удалось преобразовать актуальную цену {response.Price} в число");
    }

    public async Task<decimal> GetAccountBalance(Coin coin)
    {
        var response = await _connector.GetAccountInformation();
        var balance = response.Balances.Single(balance => balance.Asset == coin.ToString());

        return balance.Free;
    }
    
    public async Task<OrderInfo?> GetSellStopLossLimitOrder(Coin baseCoin, Coin quotedCoin, long orderId)
    {
        var openOrdersRequest = new OpenOrdersRequest(baseCoin, quotedCoin);
        var orders = await _connector.GetOpenOrders(openOrdersRequest);
        var order = orders
            .SingleOrDefault(order =>
                order.Id == orderId &&
                order is { Side: OrderSide.SELL, Type: OrderType.STOP_LOSS_LIMIT });

        return order is null ? null : new OrderInfo(order.Id, order.ExecutedQty, order.Side, order.Status, order.Type);
    }
}
