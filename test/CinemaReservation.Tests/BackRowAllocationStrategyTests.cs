using CinemaReservation.Strategies;
using Microsoft.Extensions.Logging;
using Moq;

namespace CinemaReservation.Tests;

public class BackRowAllocationStrategyTests : IClassFixture<TestFixture>
{
    private readonly IRowAllocationStrategy _strategy = new BackRowAllocationStrategy();
    private readonly ISeatAllocationStrategy _seatStrategy = new MiddleToRightStrategy();
    [Fact]
    public void MiddleToRightSeatReservationShouldPassTests()
    {
        List<SeatRow> rows = new List<SeatRow>();
        var logger = new Mock<ILogger<SeatRow>>();
        for (int i = 0; i < 10; i++)
            rows.Add(new SeatRow(logger.Object, _seatStrategy, 10));
        Dictionary<int, List<int>> seats = new Dictionary<int, List<int>>();
        int row = 0;
        int tickets = _strategy.Allocate(row, 10, rows, seats);
        Assert.Equal(0, tickets);
        Assert.Equal(10, rows[row].AvailableSeats());
        Assert.Equal(new Dictionary<int, List<int>>() { { 0, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } } }, seats);
        rows[row].Confirm(seats[0]);
        Assert.Equal(0, rows[row].AvailableSeats());

        seats.Clear();
        tickets = _strategy.Allocate(row, 5, rows, seats);
        Assert.Equal(0, tickets);
        Assert.Equal(10, rows[row + 1].AvailableSeats()); // Next row from the back.
        Assert.Equal(new Dictionary<int, List<int>>() { { row + 1, new List<int>() { 2, 3, 4, 5, 6 } } }, seats);
        rows[row + 1].Confirm(seats[row + 1]);
        Assert.Equal(5, rows[row + 1].AvailableSeats());
        /*
         * Row 1: ' ', ' ', 'x', 'x', 'x', 'x', 'x', ' ' ,' ', ' '
         */

        seats.Clear();
        tickets = _strategy.Allocate(row, 9, rows, seats);
        Assert.Equal(0, tickets);
        Assert.Equal(new Dictionary<int, List<int>>() {
                { row + 1, new List<int>() { 7, 8, 9 } },
                { row + 2, new List<int>() { 2,3,4,5,6,7 } }
            }, seats);
        Assert.Equal(5, rows[row + 1].AvailableSeats());
        Assert.Equal(10, rows[row + 2].AvailableSeats());
        rows[row + 1].Confirm(seats[row + 1]);
        rows[row + 2].Confirm(seats[row + 2]);
        Assert.Equal(2, rows[row + 1].AvailableSeats());
        Assert.Equal(4, rows[row + 2].AvailableSeats());
        /*
         * Row 1: ' ', ' ', 'x', 'x', 'x', 'x', 'x', 'x' , 'x', 'x'
         * Row 2: ' ', ' ', 'x', 'x', 'x', 'x', 'x', 'x' , ' ', ' '
         */

        seats.Clear();
        tickets = _strategy.Allocate(row, 13, rows, seats);
        Assert.Equal(0, tickets);
        Assert.Equal(new Dictionary<int, List<int>>() {
                { row + 1, new List<int>() { 0, 1 } },
                { row + 2, new List<int>() { 8, 9 } },
                { row + 3, new List<int>() { 0,1,2,3,4,5,6,7,8 } },
            }, seats);
        Assert.Equal(2, rows[row + 1].AvailableSeats());
        Assert.Equal(4, rows[row + 2].AvailableSeats());
        Assert.Equal(10, rows[row + 3].AvailableSeats());
        rows[row + 1].Confirm(seats[row + 1]);
        rows[row + 2].Confirm(seats[row + 2]);
        rows[row + 3].Confirm(seats[row + 3]);
        Assert.Equal(0, rows[row + 1].AvailableSeats());
        Assert.Equal(2, rows[row + 2].AvailableSeats());
        Assert.Equal(1, rows[row + 3].AvailableSeats());
        /*
         * Row 1: 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x' , 'x', 'x'
         * Row 2: ' ', ' ', 'x', 'x', 'x', 'x', 'x', 'x' , 'x', 'x'
         * Row 3: 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x' , 'x', ' '
         */
    }
}