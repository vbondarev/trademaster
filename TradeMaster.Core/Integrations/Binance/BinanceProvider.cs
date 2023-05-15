using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TradeMaster.Core.Integrations.Binance.Enums;
using TradeMaster.Core.Integrations.Binance.Exceptions;
using TradeMaster.Core.Integrations.Binance.Options;
using TradeMaster.Core.Integrations.Binance.Requests;
using TradeMaster.Core.Integrations.Binance.Responses;
using TradeMaster.Core.Integrations.Binance.Responses.Enums;
using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Models;

namespace TradeMaster.Core.Integrations.Binance;

internal class BinanceProvider : IBinanceProvider
{
    private readonly IBinanceConnector _connector;
    private readonly TradeOptions _options;
    private readonly ILogger<BinanceProvider> _logger;

    private long _orderId;

    public BinanceProvider(IBinanceConnector connector, IOptions<TradeOptions> options, ILogger<BinanceProvider> logger)
    {
        _connector = connector;
        _options = options.Value;
        _logger = logger;
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

    public async Task<OrderResultModel> CreateBuyOrder(Coin baseCoin, Coin quotedCoin, OrderType orderType, decimal price, decimal quantity)
    {
        var account = await _connector.GetAccountInformation();
        var balance = account.Balances.Single(b => b.Asset == quotedCoin.ToString());
        var orderCost = price * quantity;
        if (balance.Free < orderCost)
        {
            _logger.LogWarning("На балансе недостаточно средств для покупки {Quantity} {BaseCoin} по цене {Price}", baseCoin, quantity, price);
            return new OrderResultModel(0, false);
        }

        var buyOrderRequest = new BuyOrderRequest(baseCoin, quotedCoin, orderType, price, quantity);
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
        
        _orderId = buyOrder.OrderId;

        return new OrderResultModel(order.ExecutedQty, true);
    }

    public async Task<OrderResultModel> CreateSellLimitOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal quantity)
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
        
        _orderId = sellOrder.OrderId;

        return new OrderResultModel(order.ExecutedQty, true);
    }
    
    public async Task<OrderResultModel> CreateSellStopLossLimitOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal stopLimitPrice, decimal quantity)
    {
        var sellOrderRequest = new SellStopLossLimitOrderRequest(baseCoin, quotedCoin, price, stopLimitPrice, quantity);
        var sellOrder = await _connector.CreateSellStopLossLimitOrder(sellOrderRequest);
        
        return new OrderResultModel(sellOrder.ExecutedQty, true);
    }

    /// <summary>
    /// Удаление ордера происходит через его отмену
    /// </summary>
    public async Task<bool> DeleteBuyLimitOrder(Coin baseCoin, Coin quotedCoin)
    {
        var cancelOrderRequest = new CancelOrderRequest(baseCoin, quotedCoin, _orderId);
        var canceledOrder = await _connector.CancelOrder(cancelOrderRequest);

        if (canceledOrder.Status == OrderStatus.CANCELED) return true;
        throw new BinanceProviderException($"Не удалось отменить ордер {_orderId}");
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

        throw new InvalidCastException($"Не удалось преобразовать актуальную цену {response.Price} в число");
    }

    public async Task<decimal> GetAccountBalance(Coin coin)
    {
        var response = await _connector.GetAccountInformation();
        var balance = response.Balances.Single(balance => balance.Asset == coin.ToString());

        return balance.Free;
    }

    public bool GetSellStopLimitOrder(Coin coin)
    {
        throw new NotImplementedException();
    }

    public bool DeleteSellStopLimitOrder(Coin btc)
    {
        throw new NotImplementedException();
    }
}
