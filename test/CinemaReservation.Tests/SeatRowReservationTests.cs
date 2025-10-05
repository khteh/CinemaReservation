using CinemaReservation.Strategies;
using System.Reflection;

namespace CinemaReservation.Tests;

public class SeatRowReservationTests : IClassFixture<TestFixture>
{
    private ISeatAllocationStrategy _strategy;
    private FieldInfo _field = typeof(SeatRow).GetField("_index", BindingFlags.Instance | BindingFlags.NonPublic);
    public SeatRowReservationTests(TestFixture testFixture) => _strategy = testFixture.Strategy;
    [Fact]
    public void ReserveWholeRowShouldPassTests()
    {
        SeatRow row = new SeatRow(_strategy, 10);
        List<int> reserved = row.Reserve(10);
        Assert.Equal(10, reserved.Count);
        Assert.Equal([0, 1, 2, 3, 4, 5, 6, 7, 8, 9], reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _index
        int _index = (int)_field.GetValue(row);
        Assert.Equal(-1, _index);

        reserved = row.Reserve(1);
        Assert.Empty(reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _index
        _index = (int)_field.GetValue(row);
        Assert.Equal(-1, _index);
    }
    [Fact]
    public void ReserveSectionsShouldPassTests()
    {
        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x		<= (10 - 4) / 2 = 3
        */
        SeatRow row = new SeatRow(_strategy, 10);
        List<int> reserved = row.Reserve(4);
        Assert.Equal(4, reserved.Count);
        Assert.Equal([3, 4, 5, 6], reserved);
        Assert.Equal(6, row.AvailableSeats());
        // Validate _index
        int _index = (int)_field.GetValue(row);
        Assert.Equal(7, _index);

        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x x x
         */
        reserved = row.Reserve(2);
        Assert.Equal(2, reserved.Count);
        Assert.Equal([7, 8], reserved);
        Assert.Equal(4, row.AvailableSeats());
        // Validate _index
        _index = (int)_field.GetValue(row);
        Assert.Equal(9, _index);

        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x x x x
         */
        reserved = row.Reserve(1);
        Assert.Single(reserved);
        Assert.Equal([9], reserved);
        Assert.Equal(3, row.AvailableSeats());
        // Validate _index
        _index = (int)_field.GetValue(row);
        Assert.Equal(2, _index);

        /*
        0 1 2 3 4 5 6 7 8 9
            x x x x x x x x
         */
        reserved = row.Reserve(1);
        Assert.Single(reserved);
        Assert.Equal([2], reserved);
        Assert.Equal(2, row.AvailableSeats());
        // Validate _index
        _index = (int)_field.GetValue(row);
        Assert.Equal(1, _index);

        /*
        0 1 2 3 4 5 6 7 8 9
        x x x x x x x x x x
         */
        reserved = row.Reserve(2);
        Assert.Equal(2, reserved.Count);
        Assert.Equal([0, 1], reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _index
        _index = (int)_field.GetValue(row);
        Assert.Equal(-1, _index);

        reserved = row.Reserve(1);
        Assert.Empty(reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _index
        _index = (int)_field.GetValue(row);
        Assert.Equal(-1, _index);
    }
    [Fact]
    public void ReserveBiggerThanARowShouldPassTests()
    {
        SeatRow row = new SeatRow(_strategy, 10);
        List<int> reserved = row.Reserve(11);
        Assert.Equal(10, reserved.Count);
        Assert.Equal([0, 1, 2, 3, 4, 5, 6, 7, 8, 9], reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _index
        int _index = (int)_field.GetValue(row);
        Assert.Equal(-1, _index);

        reserved = row.Reserve(1);
        Assert.Empty(reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _index
        _index = (int)_field.GetValue(row);
        Assert.Equal(-1, _index);
    }
    [Fact]
    public void ReserveSpecificSeatShouldPassTests()
    {
        /*
        0 1 2 3 4 5 6 7 8 9
            x x x x
         */
        SeatRow row = new SeatRow(_strategy, 10);
        List<int> reserved = row.Reserve(2, 4);
        Assert.Equal(4, reserved.Count);
        Assert.Equal([2, 3, 4, 5], reserved);
        Assert.Equal(6, row.AvailableSeats());
        // Validate _index
        int _index = (int)_field.GetValue(row);
        Assert.Equal(6, _index);

        /*
        0 1 2 3 4 5 6 7 8 9
            x x x x x x x
         */
        reserved = row.Reserve(4, 3);
        Assert.Equal(3, reserved.Count);
        Assert.Equal([6, 7, 8], reserved);
        Assert.Equal(3, row.AvailableSeats());
        // Validate _index
        _index = (int)_field.GetValue(row);
        Assert.Equal(9, _index);

        /*
        0 1 2 3 4 5 6 7 8 9
          x x x x x x x x
         */
        reserved = row.Reserve(1, 1);
        Assert.Single(reserved);
        Assert.Equal([1], reserved);
        Assert.Equal(2, row.AvailableSeats());
        // Validate _index
        _index = (int)_field.GetValue(row);
        Assert.Equal(9, _index);

        /*
        0 1 2 3 4 5 6 7 8 9
        x x x x x x x x x x
         */
        reserved = row.Reserve(0, 3);
        Assert.Equal(2, reserved.Count);
        Assert.Equal([0, 9], reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _index
        _index = (int)_field.GetValue(row);
        Assert.Equal(-1, _index);
    }
}
