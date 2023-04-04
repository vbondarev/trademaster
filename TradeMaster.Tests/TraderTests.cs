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
        
        var riskHandler = new RiskManagementHandler();
        var tradeHandler = new TradeHandler(mockConnector.Object);
        var trader = new Trader(mockConnector.Object, tradeHandler, riskHandler);
        
        trader.StartTrading();
    }
}
