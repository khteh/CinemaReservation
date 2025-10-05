using CinemaReservation.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static System.Console;
namespace CinemaConsole;
public partial class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            string contentRootFull = Path.GetFullPath(Directory.GetCurrentDirectory());
            string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
            // Configure and run the host
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cfg =>
                {
                    cfg.SetBasePath(contentRootFull);
                    cfg.AddJsonFile($"appsettings.{environment}.json", false, true);
                    cfg.AddEnvironmentVariables().Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<ISeatAllocationStrategy, MiddleToRightStrategy>();
                    services.AddSingleton<CinemaConsoleApp>(); // Register your main application class
                })
                .Build()
                .Services.GetRequiredService<CinemaConsoleApp>() // Resolve your main application class
                .Run(args); // Execute your application logic
            return 0;
        }
        catch (Exception e)
        {
            WriteLine($"Exception! {e}");
            return -1;
        }
    }
}
public partial class Program { } // so you can reference it from tests