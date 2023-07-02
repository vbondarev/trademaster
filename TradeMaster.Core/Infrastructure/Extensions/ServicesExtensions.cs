using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TradeMaster.Core.Infrastructure.Options;
using TradeMaster.Core.Integrations.Binance;
using TradeMaster.Core.Integrations.Binance.Options;
using TradeMaster.Core.Trading;
using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Handlers;
using TradeMaster.Core.Trading.Strategies;

namespace TradeMaster.Core.Infrastructure.Extensions;

public static class ServicesExtensions
{
    
    public static IServiceCollection AddCustomConfiguration(this IServiceCollection services)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true);
        services.AddSingleton<IConfiguration>(_ => configurationBuilder.Build());
        
        
        return services;
    }
    
    
    public static IServiceCollection AddBinance(this IServiceCollection services)
    {
        AddHttp(services);
        AddServices(services);
        AddConfiguration(services);

        
        return services;
    }

    private static void AddConfiguration(IServiceCollection services)
    {
        services.ConfigureOptions<BinanceOptionsSetup>();
        services.ConfigureOptions<TradingOptionsSetup>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<BinanceApiAdapter>();
        services.AddScoped<IBinanceTradeHandler, BinanceTradeHandler>();
        services.AddTransient<IBinanceConnector, BinanceConnector>();
        services.AddTransient<IBinanceProvider, BinanceProvider>();
        services.AddTransient<BinanceBearTrendStrategy>();
        services.AddTransient<RiskManagementHandler>();
        services.AddTransient<ITrader, Trader>(sp =>
            new Trader(new Dictionary<Trend, ITrendStrategy>
            {
                { Trend.Bear, sp.GetRequiredService<BinanceBearTrendStrategy>() },
            }, sp.GetRequiredService<ILogger<Trader>>()));
    }

    private static void AddHttp(IServiceCollection services)
    {
        services.AddHttpClient("Binance", (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<BinanceOptions>>().Value;
            var baseAddress = options.BaseUri;
            client.BaseAddress = new Uri(baseAddress);
        });
    }
}
