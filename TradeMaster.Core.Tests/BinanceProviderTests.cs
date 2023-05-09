using Microsoft.Extensions.DependencyInjection;
using TradeMaster.Core.Infrastructure.Extensions;
using TradeMaster.Core.Integrations.Binance;
using TradeMaster.Core.Integrations.Binance.Enums;
using TradeMaster.Core.Integrations.Binance.Responses;
using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Models;
using Xunit;

namespace TradeMaster.Core.Tests;

public class BinanceProviderTests : IDisposable
{
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
    public async Task Should_Return_System_Status()
    {
        var status = await _provider.GetSystemStatus();
        
        Assert.Equal(BinanceStatus.Normal, status);
    }
    
    [Fact]
    public async Task Should_Return_Maximum_Coins_Price_In_The_Interval()
    {
        var startTime = DateTimeOffset.Now.AddHours(-8);
        var endTime = DateTimeOffset.Now;
        
        var maxPrice = await _provider.GetMaxPrice(Coin.BTC, Coin.USDT, Interval.Minute, startTime, endTime);
        
        Assert.True(maxPrice > 0);
    }
    
    [Fact]
    public async Task Should_Return_Minimum_Coins_Price_In_The_Interval()
    {
        var startTime = DateTimeOffset.Now.AddHours(-8);
        var endTime = DateTimeOffset.Now;
     
        var minPrice = await _provider.GetMinPrice(Coin.BTC, Coin.USDT, Interval.EightHour, startTime, endTime);
        
        Assert.True(minPrice > 0);
    }
    
    [Fact]
    public async Task Should_Return_Last_Coins_Price()
    {
        var price = await _provider.GetLastPrice(Coin.BTC, Coin.USDT);
        
        Assert.True(price.Price > 0);
    }
    
    [Fact]
    public async Task Should_Return_Account_Balance()
    {
        var amount = await _provider.GetAccountBalance(Coin.USDT);
        
        Assert.True(amount > 0);
    }
    
    [Fact]
    public async Task Should_Buy_Coins()
    {
        var price = (await _provider.GetLastPrice(Coin.BTC, Coin.USDT)).Price;
        var quantity = 0.001m;
        var orderType = OrderType.LIMIT;
        var operation = await _provider.BuyCoins(Coin.BTC, Coin.USDT, orderType, quantity, price);
        
        Assert.True(operation.Success);
        Assert.Equal(quantity, operation.CoinCount);
    }
    
    [Fact]
    public async Task Should_Sell_Coins()
    {
        var price = (await _provider.GetLastPrice(Coin.BTC, Coin.USDT)).Price;
        var quantity = 0.001m;
        var orderType = OrderType.LIMIT;
        var operation =  await _provider.SellCoins(Coin.BTC, Coin.USDT, orderType, quantity, price);
        
        Assert.True(operation.Success);
        Assert.Equal(quantity, operation.CoinCount);
    }

    public void Dispose()
    {
        _sp.Dispose();
    }
}
