namespace CinemaReservation.Tests;

public class SeatMapTests
{
    [Fact]
    public void AvailableSeatCountTests()
    {
        SeatMap sm = new SeatMap(nameof(AvailableSeatCountTests), 10, 10);
        Assert.Equal(100, sm.SeatsAvailable());
    }
}
