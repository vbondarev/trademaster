using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace TradeMaster.Core.Options;

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
