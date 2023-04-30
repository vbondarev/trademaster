using Microsoft.Extensions.DependencyInjection;
using TradeMaster.Binance;
using TradeMaster.Binance.Enums;
using TradeMaster.Binance.Requests;
using TradeMaster.Binance.Responses.Enums;
using TradeMaster.Enums;
using TradeMaster.Extensions;
using TradeMaster.Models;
using Xunit;

namespace TradeMaster.Tests;

public class BinanceConnectorTests : IDisposable
{
    private readonly ServiceProvider _provider;

    public BinanceConnectorTests()
    {
        var services = new ServiceCollection();
        services
            .AddCustomConfiguration()
            .AddCustomLogging()
            .AddBinance();
        
        _provider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task Request_Should_Return_Status_Normal()
    {
        var connector = _provider.GetRequiredService<IBinanceConnector>();
        var response = await connector.GetSystemStatus();
        
        Assert.True(response.Status == 0);
    }
    
    [Fact]
    public async Task Request_Should_Return_CandlestickData()
    {
        var connector = _provider.GetRequiredService<IBinanceConnector>();
        var startTime = DateTimeOffset.Now.AddHours(-8);
        var endTime = DateTimeOffset.Now;
        var request = new CandlestickDataRequest(Coin.BTC, Coin.USDT, Interval.Minute, startTime, endTime);
        var response = await connector.GetCandlestickData(request);
     
        Assert.NotEmpty(response);
    }
    
    [Fact]
    public async Task Request_Should_Return_Created_Order()
    {
        var price = 29242.72000000m;
        
        var connector = _provider.GetRequiredService<IBinanceConnector>();
        var buyOrderRequest = new BuyOrderRequest(Coin.BTC, Coin.USDT, OrderType.LIMIT, 0.001m, price);
        var buyOrderResponse = await connector.CreateBuyOrder(buyOrderRequest);

        await Task.Delay(TimeSpan.FromSeconds(5));
        
        var queryOrderRequest = new QueryOrderRequest(Coin.BTC, Coin.USDT, buyOrderResponse.OrderId);
        var queryOrderResponse = await connector.QueryOrder(queryOrderRequest);

        Assert.Equal(buyOrderResponse.OrderId, queryOrderResponse.OrderId);
        Assert.Equal(OrderStatus.FILLED, queryOrderResponse.Status);
        Assert.Equal(OrderSide.BUY, queryOrderResponse.Side);
        Assert.Equal(price, queryOrderResponse.Price);
    }

    public void Dispose()
    {
        _provider.Dispose();
    }
}
