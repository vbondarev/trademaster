using Microsoft.Extensions.DependencyInjection;
using TradeMaster.Binance;
using TradeMaster.Binance.Requests;
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
    public async Task Request_Should_Return_NewOrderId()
    {
        var connector = _provider.GetRequiredService<IBinanceConnector>();
        var request = new BuyOrderRequest(Coin.BTC, Coin.USDT, OrderType.Limit, 0.001m, 27450.00m);
        var response = await connector.CreateBuyOrder(request);

        var tradeListRequest = new TradeListRequest(Coin.BTC, Coin.USDT, response.OrderId);
        var a = await connector.GetAccountTradeList(tradeListRequest);

        Assert.True(response.OrderId > 0);
    }

    public void Dispose()
    {
        _provider.Dispose();
    }
}
