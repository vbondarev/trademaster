using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using TradeMaster.Core.Infrastructure.Extensions;

namespace TradeMaster.Console;

internal static class ConfigurationExtensions
{
    public static IHostBuilder AddAppConfiguration(this IHostBuilder builder)
    {
        return builder
            .ConfigureAppConfiguration((_, confBuilder) =>
            {
                confBuilder.AddJsonFile("appSettings.json", false, true);
            });
    }

    public static IHostBuilder AddServices(this IHostBuilder builder)
    {
        return builder
            .ConfigureServices((_, services) =>
            {
                services.AddBinance();
            });
    }

    public static IHostBuilder AddLoggers(this IHostBuilder builder)
    {
        return builder.UseSerilog((ctx, conf) =>
        {
            conf.ReadFrom.Configuration(ctx.Configuration);
        });
    }
}
