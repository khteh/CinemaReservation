using CinemaReservation;
using CinemaReservation.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using static System.Console;
namespace CinemaConsole;
public partial class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            var app = CreateHostBuilder(args)
                .Build();
            app.Services.GetRequiredService<CinemaConsoleApp>() // Resolve your main application class
               .Run(args); // Execute your application logic
            return 0;
        }
        catch (Exception e)
        {
            WriteLine($"Exception! {e}");
            return -1;
        }
    }
    internal static IHostBuilder CreateHostBuilder(string[] args)
    {
        string contentRootFull = Path.GetFullPath(Directory.GetCurrentDirectory());
        string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(cfg =>
            {
                cfg.SetBasePath(contentRootFull);
                cfg.AddJsonFile("appsettings.json", false, true);
                //cfg.AddJsonFile($"appsettings.{environment}.json", false, true);
                cfg.AddEnvironmentVariables().Build();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTransient<ISeatAllocationStrategy, MiddleToRightStrategy>();
                services.AddTransient<Cinema>(); // Register your main application class
                services.AddSingleton<CinemaConsoleApp>(); // Register your main application class
            })
            .UseSerilog((ctx, svc, config) => config.ReadFrom.Configuration(ctx.Configuration).ReadFrom.Services(svc).Enrich.FromLogContext());
    }
}