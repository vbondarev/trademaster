using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace TradeMaster.Core.Options;

public class TradingOptionsSetup : IConfigureOptions<TradeOptions>
{
    private const string SectionName = "Binance:Trading";

    private readonly IConfiguration _configuration;

    public TradingOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(TradeOptions options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
