using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TradeMaster.Core.Infrastructure.Options;
using TradeMaster.Core.Integrations.Binance;
using TradeMaster.Core.Integrations.Binance.Options;

namespace TradeMaster.Core.Infrastructure.Extensions;

public static class ServicesExtensions
{
    
    public static IServiceCollection AddCustomConfiguration(this IServiceCollection services)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true);
        services.AddSingleton<IConfiguration>(_ => configurationBuilder.Build());
        services.ConfigureOptions<BinanceOptionsSetup>();
        services.ConfigureOptions<TradingOptionsSetup>();
        
        return services;
    }
    
    public static IServiceCollection AddCustomLogging(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });
        return services;
    }
    
    public static IServiceCollection AddBinance(this IServiceCollection services)
    {
        services.AddHttpClient<IBinanceConnector, BinanceConnector>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<BinanceOptions>>().Value;
            var baseAddress = options.BaseUri;
            client.BaseAddress = new Uri(baseAddress);
        });
        
        services.AddTransient<IBinanceConnector, BinanceConnector>();
        services.AddTransient<IBinanceProvider, BinanceProvider>();
        return services;
    }
}
