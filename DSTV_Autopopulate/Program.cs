Efficienc
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.SystemConsole.Themes;

namespace DSTV_Autopopulate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate, applyThemeToRedirectedOutput: true)
                .CreateLogger();
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, $"DSTV TABLE Autopopulate faile initiation. Details: {ex.Message}");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
            }).ConfigureLogging((hostContext, builder) =>
            {
                builder.ConfigureSerilog(hostContext.Configuration);
            }).UseSerilog();
        }
            //=>
            //Host.CreateDefaultBuilder(args)
            //    .ConfigureServices((hostContext, services) =>
            //    {
            //        services.AddHostedService<Worker>();
            //    });

    }
    public static class LoggingBuilderExtensions
    {
        public static Microsoft.Extensions.Logging.ILoggingBuilder ConfigureSerilog(this ILoggingBuilder loggingBuilder, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            return loggingBuilder;
        }
    }
}
