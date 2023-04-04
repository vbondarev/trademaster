using Microsoft.Extensions.DependencyInjection;
using TradeMaster.Binance;
using TradeMaster.Extensions;
using Xunit;

namespace TradeMaster.Tests;

public class BinanceConnectorTests
{
    private const string ApiKey = "RsRx0PpBwrfhZdCnb1IXdbdLIydZ7oabhlGCRDgjS7LR4xVXekaMWdIfsyWJc91r";
    private const string SecretKey = "fqJebg89z5utgKIdbsXJRXoiYXshdFhSVzAFsqHs8tsNG0hkq6GXBmGqhVbMC9WG";
    
    [Fact]
    public async Task Request_Should_Return_Status_Normal()
    {
        var services = new ServiceCollection();
        services
            .AddCustomConfiguration()
            .AddCustomLogging()
            .AddBinance();
        await using var provider = services.BuildServiceProvider();

        var connector = provider.GetRequiredService<BinanceConnector>();
        var result = await connector.GetSystemStatus();
        
        Assert.True(result);
    }
}
