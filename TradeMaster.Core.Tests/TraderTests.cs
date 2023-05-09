﻿using Moq;
using TradeMaster.Core.Binance;
using TradeMaster.Core.Binance.Enums;
using TradeMaster.Core.Enums;
using TradeMaster.Core.Handlers;
using TradeMaster.Core.Models;
using Xunit;

namespace TradeMaster.Core.Tests;

public class TraderTests
{
    [Fact]
    public async Task Test_Debug()
    {
        var mockConnector = new Mock<IBinanceProvider>();
        mockConnector
            .Setup(m => m.GetAccountBalance(Coin.USDT))
            .ReturnsAsync(1000);
        
        // mockConnector
        //     .Setup(m => m.GetLastPrice(Coin.USDT,Coin.BTC))
        //     .ReturnsAsync(new CoinPriceModel(Coin.BTC, Coin.USDT) {Price = 28500, Time = DateTime.Now});
        
        mockConnector
            .SetupSequence(m => m.GetLastPrice(
                It.IsAny<Coin>(), 
                It.IsAny<Coin>()))
            .ReturnsAsync(new CoinPriceModel(Coin.BTC, Coin.USDT) {Price = 28500, Time = DateTime.Now})
            .ReturnsAsync(new CoinPriceModel(Coin.BTC, Coin.USDT) {Price = 28000, Time = DateTime.Now});
        
        mockConnector
            .SetupSequence(m => m.GetMaxPrice(
                It.IsAny<Coin>(), 
                It.IsAny<Coin>(), 
                It.IsAny<Interval>(), 
                It.IsAny<DateTimeOffset>(), 
                It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(28500)
            .ReturnsAsync(28450)
            .ReturnsAsync(28470)
            .ReturnsAsync(28200)
            .ReturnsAsync(28300)
            .ReturnsAsync(28500)
            .ReturnsAsync(28200)
            .ReturnsAsync(28100);
        
        mockConnector
            .SetupSequence(m => m.GetMinPrice(Coin.USDT,Coin.BTC, Interval.QuarterHour, It.IsAny<DateTimeOffset>(),It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(28450)
            .ReturnsAsync(28470)
            .ReturnsAsync(28200)
            .ReturnsAsync(28300)
            .ReturnsAsync(28250)
            .ReturnsAsync(28200)
            .ReturnsAsync(28100)
            .ReturnsAsync(28050);
        
        mockConnector
            .Setup(m => m.BuyCoins(Coin.BTC, Coin.USDT, OrderType.LIMIT, It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync(new OrderResultModel( 0.03566m, true));
        
        var riskHandler = new RiskManagementHandler();
        var tradeHandler = new TradeHandler(mockConnector.Object);
        var trader = new Trader(mockConnector.Object, tradeHandler, riskHandler);
        
        //Необходимо зафиксировать сумму и монету, с которой начнется торговля
        var result = await trader.StartTrading(Coin.USDT, Coin.BTC, 1000, 10);
    }
}