using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TradeMaster.Core.Tests;

internal static class IntegrationExtensions
{
    public static ServiceCollection AddTestConfiguration(this ServiceCollection services)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.test.json") // Используем конфиг для тестов
            .Build();

        services.AddSingleton<IConfiguration>(config);

        return services;
    }
}
