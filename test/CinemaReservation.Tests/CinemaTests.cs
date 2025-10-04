using System.Reflection;
namespace CinemaReservation.Tests;

public class CinemaTests
{
    private FieldInfo _seatMapField = typeof(Cinema).GetField("_seatMap", BindingFlags.Instance | BindingFlags.NonPublic);
    [Fact]
    public void CreateMovieTests()
    {
        string title = "Sex and the City";
        Cinema cinema = new Cinema();
        Dictionary<string, SeatMap> seatMap = (Dictionary<string, SeatMap>)_seatMapField.GetValue(cinema);
        cinema.CreateMovie(title, 10, 10);
        Assert.NotNull(seatMap);
        Assert.True(seatMap.ContainsKey(title.ToLower()));
    }
    [Fact]
    public void InvalidArgumentsShouldThrowTests()
    {
        Cinema cinema = new Cinema();
        //act
        Action act1 = () => cinema.Reserve(nameof(InvalidArgumentsShouldThrowTests), 10, string.Empty);
        //assert
        InvalidOperationException invalidOperation = Assert.Throws<InvalidOperationException>(act1);
        //The thrown exception can be used for even more detailed assertions.
        Assert.Equal($"Invalid movie title! {nameof(InvalidArgumentsShouldThrowTests)}", invalidOperation.Message);

        string title = "Sex and the City";
        cinema.CreateMovie(title, 10, 10);
        //act
        Action act2 = () => cinema.Reserve(title.ToLower(), -1, string.Empty);
        //assert
        ArgumentOutOfRangeException outofrange = Assert.Throws<ArgumentOutOfRangeException>(act2);
        //The thrown exception can be used for even more detailed assertions.
        Assert.Equal($"Specified argument was out of the range of valid values. (Parameter 'tickets')", outofrange.Message);

        Reservation reservation = cinema.Reserve(title.ToLower(), 101, string.Empty);
        Assert.Null(reservation);
    }
    [Fact]
    public void DefaultSeatReservationTests()
    {
        string title = "The Avengers";
        Cinema cinema = new Cinema();
        Dictionary<string, SeatMap> seatMap = (Dictionary<string, SeatMap>)_seatMapField.GetValue(cinema);
        cinema.CreateMovie(title, 10, 10);
        Assert.NotNull(seatMap);
        Assert.True(seatMap.ContainsKey(title.ToLower()));

        Reservation reservation = cinema.Reserve(title.ToLower(), 10, string.Empty);
        Assert.NotNull(reservation);
        Assert.Equal("GIC0000", reservation.Id);
        Assert.Equal(new Dictionary<int, List<int>>() { { 0, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } } }, reservation.Seats);

        reservation = cinema.Reserve(title.ToLower(), 15, string.Empty);
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

        reservation = cinema.Reserve(title.ToLower(), 8, string.Empty);
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

        reservation = cinema.Reserve(title.ToLower(), 7, string.Empty);
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
    }
    [Fact]
    public void SpecificSeatReservationTests()
    {
        string title = "Mission Impossible - Death Reckoning";
        Cinema cinema = new Cinema();
        Dictionary<string, SeatMap> seatMap = (Dictionary<string, SeatMap>)_seatMapField.GetValue(cinema);
        cinema.CreateMovie(title, 10, 10);
        Assert.NotNull(seatMap);
        Assert.True(seatMap.ContainsKey(title.ToLower()));

        Reservation reservation = cinema.Reserve(title.ToLower(), 10, "C04");
        Assert.NotNull(reservation);
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

        reservation = cinema.Reserve(title.ToLower(), 15, "B04");
        Assert.NotNull(reservation);
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

        reservation = cinema.Reserve(title.ToLower(), 13, "A04");
        Assert.NotNull(reservation);
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

        // Default Reservation
        reservation = cinema.Reserve(title.ToLower(), 19, string.Empty);
        Assert.NotNull(reservation);
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
    }
}
