using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TradeMaster.Core.Integrations.Binance.Options;

namespace TradeMaster.Core.Infrastructure.Options;

public class BinanceOptionsSetup : IConfigureOptions<BinanceOptions>
{
    private const string SectionName = "Binance";

    private readonly IConfiguration _configuration;

    public BinanceOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(BinanceOptions options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
