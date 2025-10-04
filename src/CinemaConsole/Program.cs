using CinemaReservation;
using Microsoft.Extensions.Configuration;
using System.CommandLine;
using static System.Console;
internal class Program
{
    private static Dictionary<int, string> _movies = new Dictionary<int, string>() {
        { 1, "Sex and the City"},
        { 2, "The Avengers"},
        { 3, "Mission Impossible - Death Reckoning"}
    };
    private static Dictionary<string, SeatMap> _cinemas = new Dictionary<string, SeatMap>() {
        { "Sex and the City", new SeatMap("Sex and the City")},
        { "The Avengers", new SeatMap("The Avengers")},
        { "Mission Impossible - Death Reckoning", new SeatMap("Mission Impossible - Death Reckoning")}
    };
    private static Dictionary<string, Reservation> _reservations = new Dictionary<string, Reservation>();
    private static async Task<int> Main(string[] args)
    {
        try
        {
            string contentRootFull = Path.GetFullPath(Directory.GetCurrentDirectory());
            string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
            IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath(contentRootFull)
                    .AddJsonFile($"appsettings.{environment}.json", false, true)
                    .AddEnvironmentVariables().Build();
            RootCommand rootCommand = new RootCommand("Reserve and check seats reservation for a movie in a cinema");
            Option<bool> moviesOption = new("--movies")
            {
                Description = "Show available movies"
            };
            rootCommand.Options.Add(moviesOption);
            rootCommand.SetAction(parseResult =>
            {
                if (parseResult.GetValue(moviesOption))
                {
                    foreach (KeyValuePair<int, string> movie in _movies)
                        WriteLine($"{movie.Key}: {movie.Value}");
                }
                return 0;
            });
            Option<int> movieOption = new("--movie")
            {
                Description = "Movie Title",
                DefaultValueFactory = parseResult => 1
            };
            Option<string> seatOption = new("--seat")
            {
                Description = "Preferred Seat - One alphabet for row and 2 digits for the seat number in the row. Example: A05",
                DefaultValueFactory = parseResult => string.Empty
            };
            Option<int> ticketsOption = new("--tickets")
            {
                Description = "Number of tickets to purchase",
                DefaultValueFactory = parseResult => 1
            };
            var reservation = new Command("reserve", "Reserve seats for a movie in a cinema")
            {
                movieOption,
                seatOption,
                ticketsOption
            };
            rootCommand.Subcommands.Add(reservation);
            reservation.SetAction(parseResult =>
                Reserve(parseResult.GetValue(movieOption), parseResult.GetValue(ticketsOption), parseResult.GetValue(seatOption)));
            Option<string> idOption = new Option<string>("--status")
            {
                Description = "Check the status of reservation using a provided ID"
            };
            var status = new Command("status", "Check the status of reservation using the provided ID")
            {
                idOption,
            };
            rootCommand.Subcommands.Add(status);
            status.SetAction(parseResult => CheckReservation(parseResult.GetValue(idOption)));
            return rootCommand.Parse(args).Invoke();
        }
        catch (Exception e)
        {
            WriteLine($"Exception! {e}");
            return -1;
        }
    }
    private void PrintSeats(SeatMap seatmap)
    {
        WriteLine("\t\tSCREEN\t\t");
        foreach ()
    }
    internal static void CheckReservation(string id)
    {
        try
        {
            if (!string.IsNullOrEmpty(id) && _reservations.ContainsKey(id))
            {
                WriteLine($"Reservation {id}: Seats: {_reservations[id].Seats}");
            }
            else
                WriteLine($"{nameof(CheckReservation)} Invalid reservation id: {id}!");
        }
        catch (Exception e)
        {
            WriteLine($"Exception! {e}");
        }
    }
    internal static void Reserve(int movie, int tickets, string seat)
    {
        try
        {
            if (_movies.ContainsKey(movie))
            {
                WriteLine($"{nameof(Reserve)} Reserving movie '{_movies[movie]}' for {tickets} tickets starting from seat '{seat}'");
            }
            else
                WriteLine($"{nameof(Reserve)} Invalid movie id: {movie}!");
        }
        catch (Exception e)
        {
            WriteLine($"Exception! {e}");
        }
    }
}
