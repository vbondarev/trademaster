using Moq;
using TradeMaster.Binance;
using TradeMaster.Handlers;
using TradeMaster.Models;
using Xunit;

namespace TradeMaster.Tests;

public class TraderTests
{
    [Fact]
    public async Task Test_Debug()
    {
        var mockConnector = new Mock<IBinanceProvider>();
        mockConnector
            .Setup(m => m.GetTotalAmount(Coins.USDT))
            .Returns(1000);
        
        mockConnector
            .Setup(m => m.GetTotalAmount(Coins.BTC))
            .Returns(1000);
        
        mockConnector
            .Setup(m => m.GetLastCoinPrice(Coins.USDT,Coins.BTC))
            .Returns(new CoinPriceModel{Coin = Coins.BTC, Price = 28500, Time = DateTime.Now});
        
        mockConnector
            .SetupSequence(m => m.GetMaxPrice(
                It.IsAny<Coins>(), 
                It.IsAny<Coins>(), 
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
            .SetupSequence(m => m.GetMinPrice(Coins.USDT,Coins.BTC, Interval.Minute, It.IsAny<DateTimeOffset>(),It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(28450)
            .ReturnsAsync(28470)
            .ReturnsAsync(28200)
            .ReturnsAsync(28300)
            .ReturnsAsync(28250)
            .ReturnsAsync(28200)
            .ReturnsAsync(28100)
            .ReturnsAsync(28050);
        
        mockConnector
            .Setup(m => m.BuyCoins(Coins.BTC, OrderTypes.Limit, It.IsAny<decimal>(), It.IsAny<decimal>()))
            .Returns((decimal)0.03566);
        
        var riskHandler = new RiskManagementHandler();
        var tradeHandler = new TradeHandler(mockConnector.Object);
        var trader = new Trader(mockConnector.Object, tradeHandler, riskHandler);
        
        //Необходимо зафиксировать сумму и монету, с которой начнется торговля
        await trader.StartTrading(Coins.USDT, 1000);
    }
}
