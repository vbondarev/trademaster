using Microsoft.Extensions.DependencyInjection;
using TradeMaster.Core.Infrastructure.Extensions;
using TradeMaster.Core.Integrations.Binance;
using TradeMaster.Core.Integrations.Binance.Enums;
using TradeMaster.Core.Integrations.Binance.Requests;
using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Models;
using Xunit;

namespace TradeMaster.Core.Tests;

public class BinanceConnectorTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly IBinanceConnector _connector;

    public BinanceConnectorTests()
    {
        var services = new ServiceCollection();
        services
            .AddCustomConfiguration()
            .AddCustomLogging()
            .AddBinance();
        
        _provider = services.BuildServiceProvider();
        _connector = _provider.GetRequiredService<IBinanceConnector>();

        //_connector.CancelAllOpenOrders(new CancelAllOpenOrdersRequest(Coin.BTC, Coin.USDT)).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Request_Should_Get_System_Status()
    {
        var connector = _provider.GetRequiredService<IBinanceConnector>();
        var response = await connector.GetSystemStatus();
        
        Assert.True(response.Status == 0);
    }
    
    [Fact]
    public async Task Request_Should_Return_Candlestick_Data()
    {
        var startTime = DateTimeOffset.Now.AddHours(-8);
        var endTime = DateTimeOffset.Now;
        var request = new CandlestickDataRequest(Coin.BTC, Coin.USDT, Interval.Minute, startTime, endTime);
        var response = await _connector.GetCandlestickData(request);
     
        Assert.NotEmpty(response);
    }
    
    [Fact]
    public async Task Request_Should_Create_Buy_Limit_Order()
    {
        var price = await GetLastPrice(Coin.BTC, Coin.USDT);
        
        var buyOrderRequest = new BuyLimitOrderRequest(Coin.BTC, Coin.USDT, price, 0.001m);
        var buyOrderResponse = await _connector.CreateBuyOrder(buyOrderRequest);

        var queryOrderRequest = new QueryOrderRequest(Coin.BTC, Coin.USDT, buyOrderResponse.OrderId);
        var queryOrderResponse = await _connector.QueryOrder(queryOrderRequest);

        Assert.Equal(buyOrderResponse.OrderId, queryOrderResponse.OrderId);
        Assert.Equal(OrderStatus.FILLED, queryOrderResponse.Status);
        Assert.Equal(OrderSide.BUY, queryOrderResponse.Side);
        Assert.Equal(price, queryOrderResponse.Price);
    }
    
    [Fact]
    public async Task Request_Should_Create_Sell_Limit_Order()
    {
        var price = await GetLastPrice(Coin.BTC, Coin.USDT);
        
        var sellOrderRequest = new SellLimitOrderRequest(Coin.BTC, Coin.USDT, price, 0.001m);
        var sellOrderResponse = await _connector.CreateSellLimitOrder(sellOrderRequest);

        var queryOrderRequest = new QueryOrderRequest(Coin.BTC, Coin.USDT, sellOrderResponse.OrderId);
        var queryOrderResponse = await _connector.QueryOrder(queryOrderRequest);

        Assert.Equal(sellOrderResponse.OrderId, queryOrderResponse.OrderId);
        Assert.Equal(OrderStatus.NEW, queryOrderResponse.Status);
        Assert.Equal(OrderSide.SELL, queryOrderResponse.Side);
        Assert.Equal(OrderType.LIMIT, queryOrderResponse.Type);
        Assert.Equal(price, queryOrderResponse.Price);
    }
    
    [Fact]
    public async Task Request_Should_Create_Sell_Stop_Loss_Limit_Order()
    {
        var price = await GetLastPrice(Coin.BTC, Coin.USDT);
        var stopLimitPrice = price - 100m;
        
        var sellOrderRequest = new SellStopLossLimitOrderRequest(Coin.BTC, Coin.USDT, price, stopLimitPrice, 0.001m);
        var sellOrderResponse = await _connector.CreateSellStopLossLimitOrder(sellOrderRequest);

        var queryOrderRequest = new QueryOrderRequest(Coin.BTC, Coin.USDT, sellOrderResponse.OrderId);
        var queryOrderResponse = await _connector.QueryOrder(queryOrderRequest);

        Assert.Equal(sellOrderResponse.OrderId, queryOrderResponse.OrderId);
        Assert.Equal(OrderStatus.NEW, queryOrderResponse.Status);
        Assert.Equal(OrderSide.SELL, queryOrderResponse.Side);
        Assert.Equal(OrderType.STOP_LOSS_LIMIT, queryOrderResponse.Type);
        Assert.Equal(price, queryOrderResponse.Price);
    }
    
    [Fact]
    public async Task Request_Should_Cancel_Order()
    {
        var price = await GetLastPrice(Coin.BTC, Coin.USDT);
        price += 1m;
        
        var buyOrderRequest = new BuyLimitOrderRequest(Coin.BTC, Coin.USDT, price, 0.001m);
        var buyOrderResponse = await _connector.CreateBuyOrder(buyOrderRequest);

        await Task.Delay(4000);

        var cancelOrderRequest = new CancelOrderRequest(Coin.BTC, Coin.USDT, buyOrderResponse.OrderId);
        var cancelOrderResponse = await _connector.CancelOrder(cancelOrderRequest);

        Assert.Equal(buyOrderResponse.OrderId, cancelOrderResponse.OrderId);
        Assert.Equal(OrderStatus.CANCELED, cancelOrderResponse.Status);
        Assert.Equal(OrderSide.BUY, cancelOrderResponse.Side);
        Assert.Equal(price, cancelOrderResponse.Price);
    }

    public void Dispose()
    {
        _provider.Dispose();
    }

    private async Task<decimal> GetLastPrice(Coin baseCoin, Coin quotedCoin)
    {
        var provider = _provider.GetRequiredService<IBinanceProvider>();
        return (await provider.GetLastPrice(baseCoin, quotedCoin)).Price;
    }
}
