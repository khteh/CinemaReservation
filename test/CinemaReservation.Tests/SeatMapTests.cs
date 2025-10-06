using System.Reflection;

namespace CinemaReservation.Tests;

public class SeatMapTests : IClassFixture<TestFixture>
{
    private FieldInfo _rowfield = typeof(SeatMap).GetField("_rows", BindingFlags.Instance | BindingFlags.NonPublic);
    private readonly IServiceProvider _serviceProvider;
    public SeatMapTests(TestFixture testFixture) => _serviceProvider = testFixture.Host.Services;
    [Fact]
    public void AvailableSeatCountTests()
    {
        SeatMap sm = (SeatMap)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(SeatMap), new object[] { nameof(AvailableSeatCountTests), 10, 10 });
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
        SeatMap sm = (SeatMap)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(SeatMap), new object[] { nameof(AvailableSeatCountTests), 10, 10 });
        MethodInfo _parseSeat = sm.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.Name == "ParseSeat" && x.IsPrivate).First();
        Assert.NotNull(_parseSeat);
        (int row, int col) result = ((int row, int col))_parseSeat.Invoke(sm, new object[] { str });
        Assert.Equal(row, result.row);
        Assert.Equal(seat, result.col);
    }
    [Fact]
    public void ReservationWithoutConfirmationTests()
    {
        SeatMap sm = (SeatMap)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(SeatMap), new object[] { nameof(AvailableSeatCountTests), 10, 10 });
        List<SeatRow> rows = (List<SeatRow>)_rowfield.GetValue(sm);
        FieldInfo _field = typeof(SeatRow).GetField("_index", BindingFlags.Instance | BindingFlags.NonPublic);

        int seats = sm.SeatsAvailable();
        Assert.Equal(100, seats);
        Reservation reservation = sm.Reserve(10, string.Empty);
        Assert.NotNull(reservation);
        seats = sm.SeatsAvailable();
        Assert.Equal(100, seats);
        Assert.Equal("GIC0000", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() { { 0, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } } }, reservation.Seats);
        Assert.Equal(10, rows[0].AvailableSeats());
        // Validate _index
        int _index = (int)_field.GetValue(rows[0]);
        Assert.Equal(0, _index);
    }
    [Fact]
    public void DefaultSeatReservationTests()
    {
        SeatMap sm = (SeatMap)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(SeatMap), new object[] { nameof(AvailableSeatCountTests), 10, 10 });
        List<SeatRow> rows = (List<SeatRow>)_rowfield.GetValue(sm);
        FieldInfo _field = typeof(SeatRow).GetField("_index", BindingFlags.Instance | BindingFlags.NonPublic);
        List<List<char>> map = new List<List<char>>();

        int seats = sm.SeatsAvailable();
        Assert.Equal(100, seats);
        Reservation reservation = sm.Reserve(10, string.Empty);
        Assert.NotNull(reservation);
        sm.ConfirmReservation(reservation.Id);
        sm.ShowMap(reservation.Id, map);
        Assert.Equal([['#', '#', '#', '#', '#', '#', '#', '#', '#', '#'], /* Row A*/
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
        ], map);
        seats = sm.SeatsAvailable();
        Assert.Equal(90, seats);
        Assert.Equal("GIC0000", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() { { 0, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } } }, reservation.Seats);
        Assert.Equal(0, rows[0].AvailableSeats());
        // Validate _index
        int _index = (int)_field.GetValue(rows[0]);
        Assert.Equal(-1, _index);

        reservation = sm.Reserve(15, string.Empty);
        Assert.NotNull(reservation);
        sm.ConfirmReservation(reservation.Id);
        map.Clear();
        sm.ShowMap(reservation.Id, map);
        Assert.Equal([['x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x'], /* Row A*/
            ['#', '#', '#', '#', '#', '#', '#', '#', '#', '#'],
            [' ', ' ', '#', '#', '#', '#', '#', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
        ], map);
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
        // Validate _index
        Assert.Equal(0, rows[0].AvailableSeats());
        _index = (int)_field.GetValue(rows[0]);
        Assert.Equal(-1, _index);

        Assert.Equal(0, rows[1].AvailableSeats());
        _index = (int)_field.GetValue(rows[1]);
        Assert.Equal(-1, _index);

        Assert.Equal(5, rows[2].AvailableSeats());
        _index = (int)_field.GetValue(rows[2]);
        Assert.Equal(7, _index);

        reservation = sm.Reserve(8, string.Empty);
        Assert.NotNull(reservation);
        sm.ConfirmReservation(reservation.Id);
        map.Clear();
        sm.ShowMap(reservation.Id, map);
        Assert.Equal([['x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x'], /* Row A*/
            ['x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x'],
            [' ', ' ', 'x', 'x', 'x', 'x', 'x', '#', '#', '#'],
            [' ', ' ', '#', '#', '#', '#', '#', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
        ], map);
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
        // Validate _index
        Assert.Equal(0, rows[0].AvailableSeats());
        _index = (int)_field.GetValue(rows[0]);
        Assert.Equal(-1, _index);

        Assert.Equal(0, rows[1].AvailableSeats());
        _index = (int)_field.GetValue(rows[1]);
        Assert.Equal(-1, _index);

        Assert.Equal(2, rows[2].AvailableSeats());
        _index = (int)_field.GetValue(rows[2]);
        Assert.Equal(1, _index);

        Assert.Equal(5, rows[3].AvailableSeats());
        _index = (int)_field.GetValue(rows[3]);
        Assert.Equal(7, _index);

        reservation = sm.Reserve(7, string.Empty);
        Assert.NotNull(reservation);
        sm.ConfirmReservation(reservation.Id);
        map.Clear();
        sm.ShowMap(reservation.Id, map);
        Assert.Equal([['x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x'], /* Row A*/
            ['x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x'],
            ['#', '#', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x'],
            [' ', ' ', 'x', 'x', 'x', 'x', 'x', '#', '#', '#'],
            [' ', ' ', ' ', ' ', '#', '#', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
        ], map);
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
        // Validate _index
        Assert.Equal(0, rows[0].AvailableSeats());
        _index = (int)_field.GetValue(rows[0]);
        Assert.Equal(-1, _index);

        Assert.Equal(0, rows[1].AvailableSeats());
        _index = (int)_field.GetValue(rows[1]);
        Assert.Equal(-1, _index);

        Assert.Equal(0, rows[2].AvailableSeats());
        _index = (int)_field.GetValue(rows[2]);
        Assert.Equal(-1, _index);

        Assert.Equal(2, rows[3].AvailableSeats());
        _index = (int)_field.GetValue(rows[3]);
        Assert.Equal(1, _index);

        Assert.Equal(8, rows[4].AvailableSeats());
        _index = (int)_field.GetValue(rows[4]);
        Assert.Equal(6, _index);
    }
    [Fact]
    public void SpecificSeatReservationTests()
    {
        List<List<char>> map = new List<List<char>>();
        SeatMap sm = (SeatMap)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(SeatMap), new object[] { nameof(AvailableSeatCountTests), 10, 10 });
        List<SeatRow> rows = (List<SeatRow>)_rowfield.GetValue(sm);
        FieldInfo _field = typeof(SeatRow).GetField("_index", BindingFlags.Instance | BindingFlags.NonPublic);

        int seats = sm.SeatsAvailable();
        Assert.Equal(100, seats);
        Reservation reservation = sm.Reserve(10, "C04");
        Assert.NotNull(reservation);
        sm.ConfirmReservation(reservation.Id);
        sm.ShowMap(reservation.Id, map);
        Assert.Equal([[' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '], /* Row A*/
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', '#', '#', '#', '#', '#', '#', '#'],
            [' ', ' ', ' ', '#', '#', '#', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
        ], map);
        seats = sm.SeatsAvailable();
        Assert.Equal(90, seats);
        Assert.Equal("GIC0000", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() { 
                         /*
                            0 1 2 3 4 5 6 7 8 9
                                  x x x x x x x
                         */
                            { 2, new List<int>() { 3, 4, 5, 6, 7, 8, 9 } },
                         /*
                            0 1 2 3 4 5 6 7 8 9
                                  x x x
                         */
                            { 3, new List<int>() { 3, 4, 5 } }
                        }, reservation.Seats);
        // Validate _index
        Assert.Equal(3, rows[2].AvailableSeats());
        int _index = (int)_field.GetValue(rows[2]);
        Assert.Equal(2, _index);

        Assert.Equal(7, rows[3].AvailableSeats());
        _index = (int)_field.GetValue(rows[3]);
        Assert.Equal(6, _index);

        reservation = sm.Reserve(15, "B04");
        Assert.NotNull(reservation);
        sm.ConfirmReservation(reservation.Id);
        map.Clear();
        sm.ShowMap(reservation.Id, map);
        Assert.Equal([[' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '], /* Row A*/
            [' ', ' ', ' ', '#', '#', '#', '#', '#', '#', '#'],
            ['#', '#', '#', 'x', 'x', 'x', 'x', 'x', 'x', 'x'],
            [' ', ' ', ' ', 'x', 'x', 'x', '#', '#', '#', '#'],
            [' ', ' ', ' ', ' ', '#', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
        ], map);
        seats = sm.SeatsAvailable();
        Assert.Equal(75, seats);
        Assert.Equal("GIC0001", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() {
                         /*
                            0 1 2 3 4 5 6 7 8 9
                                  x x x x x x x
                         */
                         { 1, new List<int>(){3, 4, 5, 6, 7, 8, 9 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                            x x x x x x x x x x
                          */
                         { 2, new List<int>(){0, 1, 2 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                                  x x x x x x x
                          */
                         { 3, new List<int>(){6,7,8,9 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                                    x
                          */
                         { 4, new List<int>(){4 } },
                         }, reservation.Seats);
        // Validate _index
        Assert.Equal(3, rows[1].AvailableSeats());
        _index = (int)_field.GetValue(rows[1]);
        Assert.Equal(2, _index);

        Assert.Equal(0, rows[2].AvailableSeats());
        _index = (int)_field.GetValue(rows[2]);
        Assert.Equal(-1, _index);

        Assert.Equal(3, rows[3].AvailableSeats());
        _index = (int)_field.GetValue(rows[3]);
        Assert.Equal(2, _index);

        Assert.Equal(9, rows[4].AvailableSeats());
        _index = (int)_field.GetValue(rows[4]);
        Assert.Equal(5, _index);

        reservation = sm.Reserve(13, "A04");
        Assert.NotNull(reservation);
        sm.ConfirmReservation(reservation.Id);
        map.Clear();
        sm.ShowMap(reservation.Id, map);
        Assert.Equal([[' ', ' ', ' ', '#', '#', '#', '#', '#', '#', '#'], /* Row A*/
            ['#', '#', '#', 'x', 'x', 'x', 'x', 'x', 'x', 'x'],
            ['x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x'],
            ['#', '#', '#', 'x', 'x', 'x', 'x', 'x', 'x', 'x'],
            [' ', ' ', ' ', ' ', 'x', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
        ], map);
        seats = sm.SeatsAvailable();
        Assert.Equal(62, seats);
        Assert.Equal("GIC0002", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() {
                         /*
                            0 1 2 3 4 5 6 7 8 9
                                  x x x x x x x
                         */
                         { 0, new List<int>(){3,4,5,6,7,8,9 } },
                         /*
                            0 1 2 3 4 5 6 7 8 9
                            x x x x x x x x x x
                         */
                         { 1, new List<int>(){0,1,2 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                            x x x x x x x x x x
                          */
                         { 3, new List<int>(){0,1,2 } },
                         }, reservation.Seats);
        // Validate _index
        Assert.Equal(3, rows[0].AvailableSeats());
        _index = (int)_field.GetValue(rows[0]);
        Assert.Equal(2, _index);

        Assert.Equal(0, rows[1].AvailableSeats());
        _index = (int)_field.GetValue(rows[1]);
        Assert.Equal(-1, _index);

        Assert.Equal(0, rows[2].AvailableSeats());
        _index = (int)_field.GetValue(rows[2]);
        Assert.Equal(-1, _index);

        Assert.Equal(0, rows[3].AvailableSeats());
        _index = (int)_field.GetValue(rows[3]);
        Assert.Equal(-1, _index);

        // Default Reservation
        reservation = sm.Reserve(19, string.Empty);
        Assert.NotNull(reservation);
        sm.ConfirmReservation(reservation.Id);
        map.Clear();
        sm.ShowMap(reservation.Id, map);
        Assert.Equal([['#', '#', '#', 'x', 'x', 'x', 'x', 'x', 'x', 'x'], /* Row A*/
            ['x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x'],
            ['x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x'],
            ['x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x'],
            [' ', ' ', ' ', ' ', 'x', '#', '#', '#', '#', '#'],
            ['#', '#', '#', '#', '#', '#', '#', '#', '#', '#'],
            [' ', ' ', ' ', ' ', '#', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
        ], map);
        seats = sm.SeatsAvailable();
        Assert.Equal(43, seats);
        Assert.Equal("GIC0003", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() {
                         /*
                            0 1 2 3 4 5 6 7 8 9
                            x x x x x x x x x x
                         */
                         { 0, new List<int>(){0, 1, 2 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                                    x x x x x x
                          */
                         { 4, new List<int>(){5,6,7,8,9 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                            x x x x x x x x x x
                          */
                         { 5, new List<int>(){0,1,2,3,4,5,6,7,8,9 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                                    x
                          */
                         { 6, new List<int>(){4 } },
                         }, reservation.Seats);
        // Validate _index
        Assert.Equal(0, rows[0].AvailableSeats());
        _index = (int)_field.GetValue(rows[0]);
        Assert.Equal(-1, _index);

        Assert.Equal(0, rows[1].AvailableSeats());
        _index = (int)_field.GetValue(rows[1]);
        Assert.Equal(-1, _index);

        Assert.Equal(0, rows[2].AvailableSeats());
        _index = (int)_field.GetValue(rows[2]);
        Assert.Equal(-1, _index);

        Assert.Equal(0, rows[3].AvailableSeats());
        _index = (int)_field.GetValue(rows[3]);
        Assert.Equal(-1, _index);

        Assert.Equal(4, rows[4].AvailableSeats());
        _index = (int)_field.GetValue(rows[4]);
        Assert.Equal(3, _index);

        Assert.Equal(0, rows[5].AvailableSeats());
        _index = (int)_field.GetValue(rows[5]);
        Assert.Equal(-1, _index);

        Assert.Equal(9, rows[6].AvailableSeats());
        _index = (int)_field.GetValue(rows[6]);
        Assert.Equal(5, _index);
    }
}
