using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using TradeMaster.Core.Infrastructure.Extensions;

namespace TradeMaster.Console;

public static class Program
{
    public static void Main(string[] args)
    {
        Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
        Serilog.Debugging.SelfLog.Enable(System.Console.Error);
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            using var hosting = CreateHostBuilder(args).Build();
        }
        catch (Exception e)
        {
            Log.Error(e, "Application launch error");
        }
        finally
        {
            Log.CloseAndFlush();
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
