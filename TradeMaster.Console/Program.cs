using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using TradeMaster.Core.Trading;

namespace TradeMaster.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        SelfLog.Enable(msg => Debug.WriteLine(msg));
        SelfLog.Enable(System.Console.Error);
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            using var hosting = CreateHostBuilder(args).Build();
            var trader = hosting.Services.GetRequiredService<ITrader>();

            await trader.StartTrading();
        }
        catch (Exception e)
        {
            Log.Error(e, "Application launch error");
            throw;
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
            .AddAppConfiguration()
            .AddLoggers()
            .AddServices();
    }
}
