using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TradeMaster.Binance;
using TradeMaster.Options;

namespace TradeMaster.Extensions;

public static class ServicesExtensions
{
    
    public static IServiceCollection AddCustomConfiguration(this IServiceCollection services)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true);
        services.AddSingleton<IConfiguration>(sp => configurationBuilder.Build());
        services.ConfigureOptions<BinanceOptionsSetup>();
        
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
            var baseAddress = options.SpotUri;
            client.BaseAddress = new Uri(baseAddress);
        });
        services.AddTransient<IBinanceConnector, BinanceConnector>();
        return services;
    }
}
