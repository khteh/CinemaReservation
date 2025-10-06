using System.Reflection;
namespace CinemaReservation.Tests;

public class CinemaTests : IClassFixture<TestFixture>
{
   private FieldInfo _seatMapField = typeof(Cinema).GetField("_seatMap", BindingFlags.Instance | BindingFlags.NonPublic);
   private readonly IServiceProvider _serviceProvider;
   public CinemaTests(TestFixture testFixture) => _serviceProvider = testFixture.Host.Services;
   [Fact]
   public void CreateMovieTests()
   {
      string title = "Sex and the City";
      Cinema cinema = (Cinema)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(Cinema));
      Dictionary<string, SeatMap> seatMap = (Dictionary<string, SeatMap>)_seatMapField.GetValue(cinema);
      cinema.CreateMovie(title, 10, 10);
      Assert.NotNull(seatMap);
      Assert.True(seatMap.ContainsKey(title.ToLower()));
   }
   [Fact]
   public void InvalidArgumentsShouldThrowTests()
   {
      Cinema cinema = (Cinema)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(Cinema));
      //act
      Action act1 = () => cinema.Reserve(string.Empty, nameof(InvalidArgumentsShouldThrowTests), 10);
      //assert
      InvalidOperationException invalidOperation = Assert.Throws<InvalidOperationException>(act1);
      //The thrown exception can be used for even more detailed assertions.
      Assert.Equal($"Invalid movie title! {nameof(InvalidArgumentsShouldThrowTests)}", invalidOperation.Message);

      string title = "Sex and the City";
      cinema.CreateMovie(title, 10, 10);
      //act
      Action act2 = () => cinema.Reserve(string.Empty, title.ToLower(), -1);
      //assert
      ArgumentOutOfRangeException outofrange = Assert.Throws<ArgumentOutOfRangeException>(act2);
      //The thrown exception can be used for even more detailed assertions.
      Assert.Equal($"Specified argument was out of the range of valid values. (Parameter 'tickets')", outofrange.Message);

      Reservation reservation = cinema.Reserve(string.Empty, title.ToLower(), 101);
      Assert.Null(reservation);
   }
   [Fact]
   public void ReservationWithoutConfirmationTests()
   {
      string title = "The Avengers";
      Cinema cinema = (Cinema)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(Cinema));
      Dictionary<string, SeatMap> seatMap = (Dictionary<string, SeatMap>)_seatMapField.GetValue(cinema);
      cinema.CreateMovie(title, 10, 10);
      Assert.NotNull(seatMap);
      Assert.True(seatMap.ContainsKey(title.ToLower()));

      Reservation reservation = cinema.Reserve(string.Empty, title.ToLower(), 10);
      Assert.NotNull(reservation);
      Assert.Equal("GIC0000", reservation.Id);
      Assert.Equal(new Dictionary<int, List<int>>() { { 0, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } } }, reservation.Seats);
      Assert.False(reservation.Confirmed);
      Assert.Equal(100, cinema.SeatsAvailable(title.ToLower()));

      reservation = cinema.Reserve(string.Empty, title.ToLower(), 15);
      Assert.NotNull(reservation);
      Assert.Equal("GIC0001", reservation.Id);
      Assert.Equal(new Dictionary<int, List<int>>() {
                         /*
                            0 1 2 3 4 5 6 7 8 9
                            x x x x x x x x x x
                         */
                         { 0, new List<int>(){0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } }, // <- Still row 0 because the previous reservation was not confirmed
                          /*
                            0 1 2 3 4 5 6 7 8 9
                                x x x x x
                          */
                         { 1, new List<int>(){2, 3, 4, 5, 6 } },
                         }, reservation.Seats);
      Assert.False(reservation.Confirmed);
      Assert.Equal(100, cinema.SeatsAvailable(title.ToLower()));
   }
   [Fact]
   public void DefaultSeatReservationTests()
   {
      string title = "The Avengers";
      Cinema cinema = (Cinema)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(Cinema));
      Dictionary<string, SeatMap> seatMap = (Dictionary<string, SeatMap>)_seatMapField.GetValue(cinema);
      cinema.CreateMovie(title, 10, 10);
      Assert.NotNull(seatMap);
      Assert.True(seatMap.ContainsKey(title.ToLower()));

      Reservation reservation = cinema.Reserve(string.Empty, title.ToLower(), 10);
      Assert.NotNull(reservation);
      cinema.Confirm(title, reservation.Id);
      Assert.True(reservation.Confirmed);
      Assert.Equal("GIC0000", reservation.Id);
      Assert.Equal(90, cinema.SeatsAvailable(title.ToLower()));
      Assert.Equal(new Dictionary<int, List<int>>() { { 0, new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } } }, reservation.Seats);

      reservation = cinema.Reserve(string.Empty, title.ToLower(), 15);
      Assert.NotNull(reservation);
      cinema.Confirm(title, reservation.Id);
      Assert.True(reservation.Confirmed);
      Assert.Equal("GIC0001", reservation.Id);
      Assert.Equal(75, cinema.SeatsAvailable(title.ToLower()));
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

      reservation = cinema.Reserve(string.Empty, title.ToLower(), 8);
      Assert.NotNull(reservation);
      cinema.Confirm(title, reservation.Id);
      Assert.True(reservation.Confirmed);
      Assert.Equal("GIC0002", reservation.Id);
      Assert.Equal(67, cinema.SeatsAvailable(title.ToLower()));
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

      reservation = cinema.Reserve(string.Empty, title.ToLower(), 7);
      Assert.NotNull(reservation);
      cinema.Confirm(title, reservation.Id);
      Assert.True(reservation.Confirmed);
      Assert.Equal("GIC0003", reservation.Id);
      Assert.Equal(60, cinema.SeatsAvailable(title.ToLower()));
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
      Cinema cinema = (Cinema)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(Cinema));
      Dictionary<string, SeatMap> seatMap = (Dictionary<string, SeatMap>)_seatMapField.GetValue(cinema);
      cinema.CreateMovie(title, 10, 10);
      Assert.NotNull(seatMap);
      Assert.True(seatMap.ContainsKey(title.ToLower()));

      Reservation reservation = cinema.Reserve(string.Empty, title.ToLower(), 10, 2, 3); // C04
      Assert.NotNull(reservation);
      cinema.Confirm(title, reservation.Id);
      Assert.True(reservation.Confirmed);
      Assert.Equal("GIC0000", reservation.Id);
      Assert.Equal(90, cinema.SeatsAvailable(title.ToLower()));
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

      reservation = cinema.Reserve(string.Empty, title.ToLower(), 15, 1, 3); // B04
      Assert.NotNull(reservation);
      cinema.Confirm(title, reservation.Id);
      Assert.True(reservation.Confirmed);
      Assert.Equal("GIC0001", reservation.Id);
      Assert.Equal(75, cinema.SeatsAvailable(title.ToLower()));
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

      reservation = cinema.Reserve(string.Empty, title.ToLower(), 13, 0, 3); // A04
      Assert.NotNull(reservation);
      cinema.Confirm(title, reservation.Id);
      Assert.True(reservation.Confirmed);
      Assert.Equal("GIC0002", reservation.Id);
      Assert.Equal(62, cinema.SeatsAvailable(title.ToLower()));
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
      reservation = cinema.Reserve(string.Empty, title.ToLower(), 19);
      Assert.NotNull(reservation);
      cinema.Confirm(title, reservation.Id);
      Assert.True(reservation.Confirmed);
      Assert.Equal("GIC0003", reservation.Id);
      Assert.Equal(43, cinema.SeatsAvailable(title.ToLower()));
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
   [Fact]
   public void ChangeExistingReservationSeatShouldPreserveIdTests()
   {
      string title = "Mission Impossible - Death Reckoning";
      Cinema cinema = (Cinema)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(Cinema));
      Dictionary<string, SeatMap> seatMap = (Dictionary<string, SeatMap>)_seatMapField.GetValue(cinema);
      cinema.CreateMovie(title, 10, 10);
      Assert.NotNull(seatMap);
      Assert.True(seatMap.ContainsKey(title.ToLower()));

      // Default Reservation
      Reservation reservation = cinema.Reserve(string.Empty, title.ToLower(), 10);
      Assert.NotNull(reservation);
      Assert.Equal("GIC0000", reservation.Id);
      Assert.Equal(100, cinema.SeatsAvailable(title.ToLower()));
      Assert.Equal(new Dictionary<int, List<int>>() { 
                         /*
                            0 1 2 3 4 5 6 7 8 9
                            x x x x x x x x x x
                         */
                            { 0, new List<int>() { 0,1,2,3, 4, 5, 6, 7, 8, 9 } }
                        }, reservation.Seats);

      reservation = cinema.Reserve(reservation.Id, title.ToLower(), 15, 2, 3); // C04
      Assert.NotNull(reservation);
      Assert.Equal("GIC0000", reservation.Id);
      Assert.Equal(100, cinema.SeatsAvailable(title.ToLower()));
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

      reservation = cinema.Reserve(reservation.Id, title.ToLower(), 15, 1, 3); // B04
      Assert.NotNull(reservation);
      Assert.Equal("GIC0000", reservation.Id);
      Assert.Equal(100, cinema.SeatsAvailable(title.ToLower()));
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
      cinema.Confirm(title, reservation.Id);
      Assert.True(reservation.Confirmed);
      Assert.Equal(85, cinema.SeatsAvailable(title.ToLower()));
   }
   [Fact]
   public void OverlappingSeatReservationTests()
   {
      string title = "Mission Impossible - Death Reckoning";
      Cinema cinema = (Cinema)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(Cinema));
      Dictionary<string, SeatMap> seatMap = (Dictionary<string, SeatMap>)_seatMapField.GetValue(cinema);
      cinema.CreateMovie(title, 10, 10);
      Assert.NotNull(seatMap);
      Assert.True(seatMap.ContainsKey(title.ToLower()));

      Reservation reservation = cinema.Reserve(string.Empty, title.ToLower(), 10, 2, 3); // C04
      Assert.NotNull(reservation);
      cinema.Confirm(title, reservation.Id);
      Assert.True(reservation.Confirmed);
      Assert.Equal("GIC0000", reservation.Id);
      Assert.Equal(90, cinema.SeatsAvailable(title.ToLower()));
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

      reservation = cinema.Reserve(string.Empty, title.ToLower(), 10, 3, 1); // D02
      Assert.NotNull(reservation);
      cinema.Confirm(title, reservation.Id);
      Assert.True(reservation.Confirmed);
      Assert.Equal("GIC0001", reservation.Id);
      Assert.Equal(80, cinema.SeatsAvailable(title.ToLower()));
      Assert.Equal(new Dictionary<int, List<int>>() {
                          /*
                            0 1 2 3 4 5 6 7 8 9
                              # # x x x # # # #
                          */
                         { 3, new List<int>(){1,2,6,7,8,9 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                                  x x x x
                          */
                         { 4, new List<int>(){3,4,5,6 } }
                         }, reservation.Seats);

      reservation = cinema.Reserve(string.Empty, title.ToLower(), 10, 4, 2); // E03
      Assert.NotNull(reservation);
      cinema.Confirm(title, reservation.Id);
      Assert.True(reservation.Confirmed);
      Assert.Equal("GIC0002", reservation.Id);
      Assert.Equal(70, cinema.SeatsAvailable(title.ToLower()));
      Assert.Equal(new Dictionary<int, List<int>>() {
                          /*
                            0 1 2 3 4 5 6 7 8 9
                                # x x x x # # #
                          */
                         { 4, new List<int>(){2,7,8,9 } },
                          /*
                            0 1 2 3 4 5 6 7 8 9
                                x x x x x x
                          */
                         { 5, new List<int>(){2, 3,4,5,6,7 } },
                         }, reservation.Seats);

   }
}
