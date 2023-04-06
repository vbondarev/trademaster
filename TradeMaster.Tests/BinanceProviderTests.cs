using Microsoft.Extensions.DependencyInjection;
using TradeMaster.Binance;
using TradeMaster.Binance.Responses;
using TradeMaster.Extensions;
using TradeMaster.Models;
using Xunit;

namespace TradeMaster.Tests;

public class BinanceProviderTests : IDisposable
{
    //private const string SecretKey = "fqJebg89z5utgKIdbsXJRXoiYXshdFhSVzAFsqHs8tsNG0hkq6GXBmGqhVbMC9WG";
    private readonly IBinanceProvider _provider;
    private readonly ServiceProvider _sp;

    public BinanceProviderTests()
    {
        var services = new ServiceCollection();
        services
            .AddCustomConfiguration()
            .AddCustomLogging()
            .AddBinance();
        
        _sp = services.BuildServiceProvider();
        _provider = _sp.GetRequiredService<IBinanceProvider>();
    }

    [Fact]
    public async Task Request_Should_Return_Status_Normal()
    {
        var status = await _provider.GetSystemStatus();
        
        Assert.Equal(BinanceStatus.Normal, status);
    }
    
    [Fact]
    public async Task Request_Should_Return_Maximum_Price_In_The_Interval()
    {
        var startTime = DateTimeOffset.Now.AddHours(-8);
        var endTime = DateTimeOffset.Now;
        
        var maxPrice = await _provider.GetMaxPrice(Coins.BTC, Coins.USDT, Interval.Minute, startTime, endTime);
        
        Assert.True(maxPrice > 0);
    }
    
    [Fact]
    public async Task Request_Should_Return_Maximum_Min_In_The_Interval()
    {
        var startTime = DateTimeOffset.Now.AddHours(-8);
        var endTime = DateTimeOffset.Now;
     
        var minPrice = await _provider.GetMinPrice(Coins.BTC, Coins.USDT, Interval.EightHour, startTime, endTime);
        
        Assert.True(minPrice > 0);
    }

    public void Dispose()
    {
        _sp.Dispose();
    }
}
