﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TradeMaster.Core.Integrations.Binance.Options;

namespace TradeMaster.Core.Infrastructure.Options;

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
