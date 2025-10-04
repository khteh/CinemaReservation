using System.Reflection;

namespace CinemaReservation.Tests;

public class SeatMapTests
{
    private FieldInfo _rowfield = typeof(SeatMap).GetField("_rows", BindingFlags.Instance | BindingFlags.NonPublic);
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
    [Fact]
    public void DefaultSeatReservationTests()
    {
        SeatMap sm = new SeatMap(nameof(AvailableSeatCountTests), 10, 10);
        List<SeatRow> rows = (List<SeatRow>)_rowfield.GetValue(sm);
        FieldInfo _field = typeof(SeatRow).GetField("_offset", BindingFlags.Instance | BindingFlags.NonPublic);

        int seats = sm.SeatsAvailable();
        Assert.Equal(100, seats);
        Reservation reservation = sm.Reserve(10, string.Empty);
        Assert.NotNull(reservation);
        seats = sm.SeatsAvailable();
        Assert.Equal(90, seats);
        Assert.Equal("GIC0000", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() { { 0, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } } }, reservation.Seats);
        Assert.Equal(0, rows[0].AvailableSeats());
        // Validate _offset
        int _offset = (int)_field.GetValue(rows[0]);
        Assert.Equal(-1, _offset);

        reservation = sm.Reserve(15, string.Empty);
        Assert.NotNull(reservation);
        seats = sm.SeatsAvailable();
        Assert.Equal(75, seats);
        Assert.Equal("GIC0001", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() {
                         /*
                            0 1 2 3 4 5 6 7 8 9
                            x x x x x x x x x x
                         */
                         { 1, new List<int>(){0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                                x x x x x
                          */
                         { 2, new List<int>(){2, 3, 4, 5, 6 } },
                         }, reservation.Seats);
        // Validate _offset
        Assert.Equal(0, rows[0].AvailableSeats());
        _offset = (int)_field.GetValue(rows[0]);
        Assert.Equal(-1, _offset);

        Assert.Equal(0, rows[1].AvailableSeats());
        _offset = (int)_field.GetValue(rows[1]);
        Assert.Equal(-1, _offset);

        Assert.Equal(5, rows[2].AvailableSeats());
        _offset = (int)_field.GetValue(rows[2]);
        Assert.Equal(7, _offset);

        reservation = sm.Reserve(8, string.Empty);
        Assert.NotNull(reservation);
        seats = sm.SeatsAvailable();
        Assert.Equal(67, seats);
        Assert.Equal("GIC0002", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() {
                         /*
                            0 1 2 3 4 5 6 7 8 9
                                x x x x x x x x
                         */
                         { 2, new List<int>(){7, 8, 9 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                                x x x x x
                          */
                         { 3, new List<int>(){2,3,4,5,6 } },
                         }, reservation.Seats);
        // Validate _offset
        Assert.Equal(0, rows[0].AvailableSeats());
        _offset = (int)_field.GetValue(rows[0]);
        Assert.Equal(-1, _offset);

        Assert.Equal(0, rows[1].AvailableSeats());
        _offset = (int)_field.GetValue(rows[1]);
        Assert.Equal(-1, _offset);

        Assert.Equal(2, rows[2].AvailableSeats());
        _offset = (int)_field.GetValue(rows[2]);
        Assert.Equal(1, _offset);

        Assert.Equal(5, rows[3].AvailableSeats());
        _offset = (int)_field.GetValue(rows[3]);
        Assert.Equal(7, _offset);

        reservation = sm.Reserve(7, string.Empty);
        Assert.NotNull(reservation);
        seats = sm.SeatsAvailable();
        Assert.Equal(60, seats);
        Assert.Equal("GIC0003", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() {
                         /*
                            0 1 2 3 4 5 6 7 8 9
                            x x x x x x x x x x
                         */
                         { 2, new List<int>(){0, 1 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                                x x x x x x x x
                          */
                         { 3, new List<int>(){7,8,9 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                                    x x
                          */
                         { 4, new List<int>(){4,5 } },
                         }, reservation.Seats);
        // Validate _offset
        Assert.Equal(0, rows[0].AvailableSeats());
        _offset = (int)_field.GetValue(rows[0]);
        Assert.Equal(-1, _offset);

        Assert.Equal(0, rows[1].AvailableSeats());
        _offset = (int)_field.GetValue(rows[1]);
        Assert.Equal(-1, _offset);

        Assert.Equal(0, rows[2].AvailableSeats());
        _offset = (int)_field.GetValue(rows[2]);
        Assert.Equal(-1, _offset);

        Assert.Equal(2, rows[3].AvailableSeats());
        _offset = (int)_field.GetValue(rows[3]);
        Assert.Equal(1, _offset);

        Assert.Equal(8, rows[4].AvailableSeats());
        _offset = (int)_field.GetValue(rows[4]);
        Assert.Equal(6, _offset);
    }
}
