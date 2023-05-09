using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TradeMaster.Core.Binance.Enums;
using TradeMaster.Core.Binance.Requests;
using TradeMaster.Core.Binance.Responses;
using TradeMaster.Core.Binance.Responses.Enums;
using TradeMaster.Core.Enums;
using TradeMaster.Core.Exceptions;
using TradeMaster.Core.Models;
using TradeMaster.Core.Options;

namespace TradeMaster.Core.Binance;

internal class BinanceProvider : IBinanceProvider
{
    private readonly IBinanceConnector _connector;
    private readonly TradeOptions _options;
    private readonly ILogger<BinanceProvider> _logger;

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

    /// <summary>
    /// Метод покупки монеты. В данном методе необходимо реализовать механизм, который вернет успех.
    /// В случае, если в течение 10 минут ордер на покупку не будет исполнен, необходимо вернуть false в свойстве Success.
    /// Важно мониторить результат выполнения ордера, и как только он будет исполнен, вернуть true в свойстве Success.
    /// Если через 10 минут ордер не исполнен, Антон перезапустит механизм покупки.
    /// </summary>
    public async Task<OrderResultModel> BuyCoins(Coin baseCoin, Coin quotedCoin, OrderType orderType, decimal quantity, decimal price)
    {
        var account = await _connector.GetAccountInformation();
        var balance = account.Balances.Single(b => b.Asset == quotedCoin.ToString());
        var orderCost = price * quantity;
        if (balance.Free < orderCost)
        {
            _logger.LogWarning("На балансе недостаточно средств для покупки {Quantity} {BaseCoin} по цене {Price}", baseCoin, quantity, price);
            return new OrderResultModel(0, false);
        }

        var buyOrderRequest = new BuyOrderRequest(baseCoin, quotedCoin, orderType, quantity, price);
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

        return new OrderResultModel(order.ExecutedQty, true);
    }

    public async Task<OrderResultModel> SellCoins(Coin baseCoin, Coin quotedCoin, OrderType orderType, decimal quantity, decimal price)
    {
        var sellOrderRequest = new SellOrderRequest(baseCoin, quotedCoin, orderType, quantity, price);
        var sellOrder = await _connector.CreateSellOrder(sellOrderRequest);

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

        return new OrderResultModel(order.ExecutedQty, true);
    }

    /// <summary>
    /// Удаление существующего лимитного ордера на покупку котируемой монеты
    /// </summary>
    /// <param name="quotedCoin"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool DeleteBuyLimitOrder(Coin quotedCoin)
    {
        throw new NotImplementedException();
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

    public bool CellStopLimitOrderCheck(Coin coin)
    {
        throw new NotImplementedException();
    }

    public bool DeleteCellStopLimitOrder(Coin btc)
    {
        throw new NotImplementedException();
    }
}
