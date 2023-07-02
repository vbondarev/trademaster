using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TradeMaster.Core.Infrastructure.Extensions;
using TradeMaster.Core.Trading;
using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Strategies;

namespace TradeMaster.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
        Serilog.Debugging.SelfLog.Enable(System.Console.Error);
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            using var hosting = CreateHostBuilder(args).Build();
            var trader = hosting.Services.GetRequiredService<ITrader>();

            await trader.StartTrading(new Dictionary<Trend, TradeParameter>
            {
                {
                    Trend.Bear,
                    new TradeParameter
                    {
                        BaseCoin = Coin.BTC, QuotedCoin = Coin.USDT, TotalOrderCount = 1, StartAmount = 10
                    }
                }
            });
        }
        catch (Exception e)
        {
            Log.Error(e, "Application launch error");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
    
    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, builder) =>
            {
                builder.AddJsonFile("appSettings.json", false, true);
            })
            .ConfigureLogging((_, builder) => { builder.AddSerilog(); })
            .ConfigureServices((_, services) =>
            {
                services.AddBinance();
            });
    }
}
