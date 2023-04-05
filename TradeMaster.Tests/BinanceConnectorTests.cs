using Microsoft.Extensions.DependencyInjection;
using TradeMaster.Binance;
using TradeMaster.Binance.Requests;
using TradeMaster.Extensions;
using Xunit;

namespace TradeMaster.Tests;

public class BinanceConnectorTests : IDisposable
{
    private const string ApiKey = "RsRx0PpBwrfhZdCnb1IXdbdLIydZ7oabhlGCRDgjS7LR4xVXekaMWdIfsyWJc91r";
    private const string SecretKey = "fqJebg89z5utgKIdbsXJRXoiYXshdFhSVzAFsqHs8tsNG0hkq6GXBmGqhVbMC9WG";
    
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
    public async Task Request_Should_Return_Maximum_Price_In_The_Interval()
    {
        var connector = _provider.GetRequiredService<IBinanceConnector>();
        var startTime = DateTimeOffset.Now.AddHours(-8);
        var endTime = DateTimeOffset.Now;
        var request = new GetMaxPriceRequest(Coins.BTC, Coins.USDT, startTime, endTime);
        var response = await connector.GetMaxPrice(request);
        
    }

    public void Dispose()
    {
        _provider.Dispose();
    }
}
