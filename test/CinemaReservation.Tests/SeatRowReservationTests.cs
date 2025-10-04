using System.Reflection;

namespace CinemaReservation.Tests;

public class SeatRowReservationTests
{
    private FieldInfo _field = typeof(SeatRow).GetField("_offset", BindingFlags.Instance | BindingFlags.NonPublic);
    [Fact]
    public void ReserveWholeRowShouldPassTests()
    {
        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x		<= (10 - 4) / 2 = 3

        0 1 2 3 4 5 6 7 8 9
                x x x		<= (10 - 3) / 2 = 3.5
              x x x			<= (10 - 3) / 2 = 3.5 (floor)

        0 1 2 3 4 5 6 7 8 9
                x x			<= (10 - 2) / 2 = 4

        0 1 2 3 4 5 6 7 8 9 10
              x x x x		<= (11 - 4) / 2 = 3.5 (floor)

        0 1 2 3 4 5 6 7 8 9 10
                x x x		<= (11 - 3) / 2 = 4

        0 1 2 3 4 5 6 7 8 9 10
                x x			<= (11 - 2) / 2 = 4.5 (floor)     
        */
        SeatRow row = new SeatRow(10);
        int reserved = row.Reserve(10);
        Assert.Equal(10, reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _offset
        int _offset = (int)_field.GetValue(row);
        Assert.Equal(-1, _offset);

        reserved = row.Reserve(1);
        Assert.Equal(0, reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _offset
        _offset = (int)_field.GetValue(row);
        Assert.Equal(-1, _offset);
    }
    [Fact]
    public void ReserveSectionsShouldPassTests()
    {
        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x		<= (10 - 4) / 2 = 3
        */
        SeatRow row = new SeatRow(10);
        int reserved = row.Reserve(4);
        Assert.Equal(4, reserved);
        Assert.Equal(6, row.AvailableSeats());
        // Validate _offset
        int _offset = (int)_field.GetValue(row);
        Assert.Equal(7, _offset);

        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x x x
         */
        reserved = row.Reserve(2);
        Assert.Equal(2, reserved);
        Assert.Equal(4, row.AvailableSeats());
        // Validate _offset
        _offset = (int)_field.GetValue(row);
        Assert.Equal(9, _offset);

        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x x x x
         */
        reserved = row.Reserve(1);
        Assert.Equal(1, reserved);
        Assert.Equal(3, row.AvailableSeats());
        // Validate _offset
        _offset = (int)_field.GetValue(row);
        Assert.Equal(2, _offset);

        /*
        0 1 2 3 4 5 6 7 8 9
            x x x x x x x x
         */
        reserved = row.Reserve(1);
        Assert.Equal(1, reserved);
        Assert.Equal(2, row.AvailableSeats());
        // Validate _offset
        _offset = (int)_field.GetValue(row);
        Assert.Equal(1, _offset);

        /*
        0 1 2 3 4 5 6 7 8 9
        x x x x x x x x x x
         */
        reserved = row.Reserve(2);
        Assert.Equal(2, reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _offset
        _offset = (int)_field.GetValue(row);
        Assert.Equal(-1, _offset);

        reserved = row.Reserve(1);
        Assert.Equal(0, reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _offset
        _offset = (int)_field.GetValue(row);
        Assert.Equal(-1, _offset);
    }
    [Fact]
    public void ReserveBiggerThanARowShouldPassTests()
    {
        SeatRow row = new SeatRow(10);
        int reserved = row.Reserve(11);
        Assert.Equal(10, reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _offset
        int _offset = (int)_field.GetValue(row);
        Assert.Equal(-1, _offset);

        reserved = row.Reserve(1);
        Assert.Equal(0, reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _offset
        _offset = (int)_field.GetValue(row);
        Assert.Equal(-1, _offset);
    }
    [Fact]
    public void ReserveSpecificSeatShouldPassTests()
    {
        /*
        0 1 2 3 4 5 6 7 8 9
            x x x x
         */
        SeatRow row = new SeatRow(10);
        int reserved = row.Reserve(2, 4);
        Assert.Equal(4, reserved);
        Assert.Equal(6, row.AvailableSeats());
        // Validate _offset
        int _offset = (int)_field.GetValue(row);
        Assert.Equal(6, _offset);

        /*
        0 1 2 3 4 5 6 7 8 9
            x x x x x x x
         */
        reserved = row.Reserve(4, 3);
        Assert.Equal(3, reserved);
        Assert.Equal(3, row.AvailableSeats());
        // Validate _offset
        _offset = (int)_field.GetValue(row);
        Assert.Equal(9, _offset);

        /*
        0 1 2 3 4 5 6 7 8 9
          x x x x x x x x
         */
        reserved = row.Reserve(1, 1);
        Assert.Equal(1, reserved);
        Assert.Equal(2, row.AvailableSeats());
        // Validate _offset
        _offset = (int)_field.GetValue(row);
        Assert.Equal(9, _offset);

        /*
        0 1 2 3 4 5 6 7 8 9
        x x x x x x x x x x
         */
        reserved = row.Reserve(0, 3);
        Assert.Equal(2, reserved);
        Assert.Equal(0, row.AvailableSeats());
        // Validate _offset
        _offset = (int)_field.GetValue(row);
        Assert.Equal(-1, _offset);
    }
}
