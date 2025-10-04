using System.Reflection;

namespace CinemaReservation.Tests;

public class SeatMapTests
{
    [Fact]
    public void AvailableSeatCountTests()
    {
        SeatMap sm = new SeatMap(nameof(AvailableSeatCountTests), 10, 10);
        Assert.Equal(100, sm.SeatsAvailable());
    }
    [Theory]
    [InlineData("A05", 0, 5)]
    [InlineData("AB05", -1, -1)]
    [InlineData("A123", -1, -1)]
    [InlineData("AB123", -1, -1)]
    [InlineData("J10", 9, 10)]
    [InlineData("K05", -1, -1)] // row should be <= J (10)
    [InlineData("E00", -1, -1)] // col should be >= 1
    [InlineData("E05", 4, 5)]
    [InlineData("E11", -1, -1)] // col should be <= 10
    public void ParseSeatTests(string str, int row, int seat)
    {
        SeatMap seatMap = new SeatMap(nameof(ParseSeatTests), 10, 10);
        MethodInfo _parseSeat = seatMap.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.Name == "ParseSeat" && x.IsPrivate).First();
        Assert.NotNull(_parseSeat);
        (int row, int col) result = ((int row, int col))_parseSeat.Invoke(seatMap, new object[] { str });
        Assert.Equal(row, result.row);
        Assert.Equal(seat, result.col);
    }
}
