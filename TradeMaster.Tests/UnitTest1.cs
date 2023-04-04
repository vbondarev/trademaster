namespace TradeMaster.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var class1 = new Traider();
        var result = class1.TestMethod(2, 3);
        
        Assert.True(result == 5);
    }
    
    [Fact]
    public void Test2()
    {
        var tradeHandler = new TradeHandler();
        tradeHandler.StartTrading();
    }
    
    
}
