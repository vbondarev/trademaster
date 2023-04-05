using Moq;
using TradeMaster.Binance;
using TradeMaster.Handlers;
using TradeMaster.Models;
using Xunit;

namespace TradeMaster.Tests;

public class TraderTests
{
    [Fact]
    public void Test()
    {
        var mockConnector = new Mock<IBinanceConnector>();
        mockConnector
            .Setup(m => m.GetTotalAmount(Coins.USDT))
            .Returns(1000);
        
        mockConnector
            .Setup(m => m.GetLastCoinPrice(Coins.BTC))
            .Returns(new CoinPriceModel{Coin = Coins.BTC, Price = 28500, Time = DateTime.Now});
        
        mockConnector
            .SetupSequence(m => m.GetMaxPrice(Coins.BTC, It.IsAny<DateTime>(),It.IsAny<DateTime>()))
            .Returns(28500)
            .Returns(28450)
            .Returns(28470)
            .Returns(28200)
            .Returns(28300)
            .Returns(28500)
            .Returns(28200)
            .Returns(28100);
        
        mockConnector
            .SetupSequence(m => m.GetMinPrice(Coins.BTC, It.IsAny<DateTime>(),It.IsAny<DateTime>()))
            .Returns(28450)
            .Returns(28470)
            .Returns(28200)
            .Returns(28300)
            .Returns(28250)
            .Returns(28200)
            .Returns(28100)
            .Returns(28050);
        
        var riskHandler = new RiskManagementHandler();
        var tradeHandler = new TradeHandler(mockConnector.Object);
        var trader = new Trader(mockConnector.Object, tradeHandler, riskHandler);
        
        trader.StartTrading();
    }
}
