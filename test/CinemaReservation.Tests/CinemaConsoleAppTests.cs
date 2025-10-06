using CinemaConsole;
using System.Reflection;
namespace CinemaReservation.Tests;

public class CinemaConsoleAppTests : IClassFixture<TestFixture>
{
    private readonly IServiceProvider _serviceProvider;
    public CinemaConsoleAppTests(TestFixture testFixture) => _serviceProvider = testFixture.Host.Services;
    [Theory]
    [InlineData("A05", 0, 4)]
    [InlineData("AB05", -1, -1)]
    [InlineData("A123", -1, -1)]
    [InlineData("AB123", -1, -1)]
    [InlineData("J10", 9, 9)]
    [InlineData("K05", -1, -1)] // row should be <= J (10)
    [InlineData("E00", -1, -1)] // col should be >= 1
    [InlineData("E05", 4, 4)]
    [InlineData("E11", -1, -1)] // col should be <= 10
    public void ParseSeatTests(string str, int row, int seat)
    {
        CinemaConsoleApp sm = (CinemaConsoleApp)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(CinemaConsoleApp));
        MethodInfo _parseSeat = sm.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.Name == "ParseSeat" && x.IsPrivate).First();
        Assert.NotNull(_parseSeat);
        // private (int, int) ParseSeat(string seat, int rows, int seats)
        (int row, int col) result = ((int row, int col))_parseSeat.Invoke(sm, new object[] { str, 10, 10 });
        Assert.Equal(row, result.row);
        Assert.Equal(seat, result.col);
    }
}
