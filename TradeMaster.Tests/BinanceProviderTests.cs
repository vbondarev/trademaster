using Microsoft.Extensions.DependencyInjection;
using TradeMaster.Binance;
using TradeMaster.Extensions;
using TradeMaster.Models;
using Xunit;

namespace TradeMaster.Tests;

public class BinanceProviderTests : IDisposable
{
    //private const string SecretKey = "fqJebg89z5utgKIdbsXJRXoiYXshdFhSVzAFsqHs8tsNG0hkq6GXBmGqhVbMC9WG";
    
    private readonly ServiceProvider _provider;

    public BinanceProviderTests()
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
        var connector = _provider.GetRequiredService<IBinanceProvider>();
        var startTime = DateTimeOffset.Now.AddHours(-8);
        var endTime = DateTimeOffset.Now;
     
        var maxPrice = await connector.GetMaxPrice(Coins.BTC, Coins.USDT, Interval.Minute, startTime, endTime);
        
        Assert.True(maxPrice > 0);
    }

    public void Dispose()
    {
        _provider.Dispose();
    }
}
