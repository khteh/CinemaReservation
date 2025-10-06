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
    [Fact]
    public void ReservationWithoutConfirmationTests()
    {
        SeatMap sm = (SeatMap)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(SeatMap), new object[] { nameof(AvailableSeatCountTests), 10, 10 });
        List<SeatRow> rows = (List<SeatRow>)_rowfield.GetValue(sm);
        FieldInfo _field = typeof(SeatRow).GetField("_index", BindingFlags.Instance | BindingFlags.NonPublic);

        int seats = sm.SeatsAvailable();
        Assert.Equal(100, seats);
        Reservation reservation = sm.Reserve(string.Empty, 10);
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
        Reservation reservation = sm.Reserve(string.Empty, 10);
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

        reservation = sm.Reserve(string.Empty, 15);
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

        reservation = sm.Reserve(string.Empty, 8);
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

        reservation = sm.Reserve(string.Empty, 7);
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
        SeatMap sm = (SeatMap)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(SeatMap), new object[] { nameof(SpecificSeatReservationTests), 10, 10 });
        List<SeatRow> rows = (List<SeatRow>)_rowfield.GetValue(sm);
        FieldInfo _field = typeof(SeatRow).GetField("_index", BindingFlags.Instance | BindingFlags.NonPublic);

        int seats = sm.SeatsAvailable();
        Assert.Equal(100, seats);
        Reservation reservation = sm.Reserve(string.Empty, 10, 2, 3); // C04
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

        reservation = sm.Reserve(string.Empty, 15, 1, 3); // B04
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

        reservation = sm.Reserve(string.Empty, 13, 0, 3); // A04
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
        reservation = sm.Reserve(string.Empty, 19);
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
    [Fact]
    public void ChangeExistingReservationSeatShouldPreserveIdTests()
    {
        SeatMap sm = (SeatMap)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(SeatMap), new object[] { nameof(AvailableSeatCountTests), 10, 10 });
        List<SeatRow> rows = (List<SeatRow>)_rowfield.GetValue(sm);
        FieldInfo _field = typeof(SeatRow).GetField("_index", BindingFlags.Instance | BindingFlags.NonPublic);
        List<List<char>> map = new List<List<char>>();

        int seats = sm.SeatsAvailable();
        Assert.Equal(100, seats);
        Reservation reservation = sm.Reserve(string.Empty, 10);
        Assert.NotNull(reservation);
        //sm.ConfirmReservation(reservation.Id);
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
        Assert.Equal(100, seats);
        Assert.Equal("GIC0000", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() { { 0, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } } }, reservation.Seats);
        Assert.Equal(10, rows[0].AvailableSeats());
        // Validate _index
        int _index = (int)_field.GetValue(rows[0]);
        Assert.Equal(0, _index);

        reservation = sm.Reserve(reservation.Id, 15, 2, 3); // C04
        Assert.NotNull(reservation);
        //sm.ConfirmReservation(reservation.Id);
        map.Clear();
        sm.ShowMap(reservation.Id, map);
        Assert.Equal([[' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '], /* Row A*/
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', '#', '#', '#', '#', '#', '#', '#'],
            [' ', '#', '#', '#', '#', '#', '#', '#', '#', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
        ], map);
        seats = sm.SeatsAvailable();
        Assert.Equal(100, seats);
        Assert.Equal("GIC0000", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() {
                         /*
                            0 1 2 3 4 5 6 7 8 9
                                  x x x x x x x
                         */
                         { 2, new List<int>(){3, 4, 5, 6, 7, 8, 9 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                              x x x x x x x x
                          */
                         { 3, new List<int>(){ 1, 2, 3, 4, 5, 6, 7, 8 } }
                         }, reservation.Seats);
        // Validate _index
        Assert.Equal(10, rows[0].AvailableSeats());
        _index = (int)_field.GetValue(rows[0]);
        Assert.Equal(0, _index);

        Assert.Equal(10, rows[1].AvailableSeats());
        _index = (int)_field.GetValue(rows[1]);
        Assert.Equal(0, _index);

        Assert.Equal(10, rows[2].AvailableSeats());
        _index = (int)_field.GetValue(rows[2]);
        Assert.Equal(0, _index);

        Assert.Equal(10, rows[3].AvailableSeats());
        _index = (int)_field.GetValue(rows[3]);
        Assert.Equal(0, _index);

        reservation = sm.Reserve(reservation.Id, 15, 1, 3); // B04
        Assert.NotNull(reservation);
        //sm.ConfirmReservation(reservation.Id);
        map.Clear();
        sm.ShowMap(reservation.Id, map);
        Assert.Equal([[' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '], /* Row A*/
            [' ', ' ', ' ', '#', '#', '#', '#', '#', '#', '#'],
            [' ', '#', '#', '#', '#', '#', '#', '#', '#', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
            [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '],
        ], map);
        seats = sm.SeatsAvailable();
        Assert.Equal(100, seats);
        Assert.Equal("GIC0000", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() {
                         /*
                            0 1 2 3 4 5 6 7 8 9
                                  x x x x x x x
                         */
                         { 1, new List<int>(){3, 4, 5, 6, 7, 8, 9 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                              x x x x x x x x
                          */
                         { 2, new List<int>(){ 1, 2, 3, 4, 5, 6, 7, 8 } }
                         }, reservation.Seats);
        // Validate _index
        Assert.Equal(10, rows[0].AvailableSeats());
        _index = (int)_field.GetValue(rows[0]);
        Assert.Equal(0, _index);

        Assert.Equal(10, rows[1].AvailableSeats());
        _index = (int)_field.GetValue(rows[1]);
        Assert.Equal(0, _index);

        Assert.Equal(10, rows[2].AvailableSeats());
        _index = (int)_field.GetValue(rows[2]);
        Assert.Equal(0, _index);

        Assert.Equal(10, rows[3].AvailableSeats());
        _index = (int)_field.GetValue(rows[3]);
        Assert.Equal(0, _index);
    }
}
