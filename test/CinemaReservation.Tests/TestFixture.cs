using CinemaConsole;
using CinemaReservation.Strategies;
using Microsoft.Extensions.Hosting;

namespace CinemaReservation.Tests;

public class TestFixture : IDisposable
{
    public IHost Host { get; private set; }
    public ISeatAllocationStrategy Strategy { get; private set; }
    public TestFixture()
    {
        Host = Program.CreateHostBuilder([]).Build();
        Strategy = Host.Services.GetRequiredService<ISeatAllocationStrategy>();
    }
    public void Dispose()
    {
        Host?.Dispose();
        GC.SuppressFinalize(this);
    }
}
