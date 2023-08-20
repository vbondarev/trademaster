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
            .AddTestConfiguration()
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
    public async Task Should_Return_Buy_Limit_Order()
    {
        var quantity = 0.001m;
        var price = (await _provider.GetLastPrice(Coin.BTC, Coin.USDT)).Price;
        var orderInfo = await _provider.CreateBuyLimitOrder(Coin.BTC, Coin.USDT, price, quantity);
        
        Assert.Equal(OrderType.LIMIT, orderInfo.Type);
        Assert.Equal(OrderSide.BUY, orderInfo.Side);
        Assert.Equal(quantity, orderInfo.ExecutedQty);
    }
    
    [Fact]
    public async Task Should_Return_Sell_Limit_Order()
    {
        var quantity = 0.001m;
        var price = (await _provider.GetLastPrice(Coin.BTC, Coin.USDT)).Price;
        var orderInfo =  await _provider.CreateSellLimitOrder(Coin.BTC, Coin.USDT, price, quantity);
        
        Assert.Equal(OrderType.LIMIT, orderInfo.Type);
        Assert.Equal(OrderSide.SELL, orderInfo.Side);
        Assert.Equal(quantity, orderInfo.ExecutedQty);
    }
    
    [Fact]
    public async Task Should_Return_Sell_Stop_Loss_Order()
    {
        var quantity = 0.001m;
        var price = (await _provider.GetLastPrice(Coin.BTC, Coin.USDT)).Price;
        var stopLimitPrice = price - 100m;
        var orderInfo =  await _provider.CreateSellStopLossLimitOrder(Coin.BTC, Coin.USDT, price, stopLimitPrice, quantity);
        
        Assert.Equal(OrderType.STOP_LOSS_LIMIT, orderInfo.Type);
        Assert.Equal(OrderSide.SELL, orderInfo.Side);
        Assert.Equal(quantity, orderInfo.ExecutedQty);
    }

    public void Dispose()
    {
        _sp.Dispose();
    }
}
