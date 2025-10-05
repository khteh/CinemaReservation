using CinemaReservation;
using CinemaReservation.Strategies;
using static System.Console;
namespace CinemaConsole;

public class CinemaConsoleApp
{
    private readonly ISeatAllocationStrategy _strategy;
    private Dictionary<int, string> _movies = new Dictionary<int, string>() {
        { 1, "Sex and the City"},
        { 2, "The Avengers"},
        { 3, "Mission Impossible - Death Reckoning"}
    };
    private Dictionary<string, SeatMap> _cinemas;
    private Dictionary<string, Reservation> _reservations = new Dictionary<string, Reservation>();
    public CinemaConsoleApp(ISeatAllocationStrategy strategy)
    {
        _strategy = strategy;
        _cinemas = new Dictionary<string, SeatMap>() {
                { "Sex and the City", new SeatMap(strategy, "Sex and the City")},
                { "The Avengers", new SeatMap(strategy, "The Avengers")},
                { "Mission Impossible - Death Reckoning", new SeatMap(strategy, "Mission Impossible - Death Reckoning")}
            };
    }
    public void Run(string[] args)
    {
        WriteLine($"Please define movie title and seat map in [Title] [Rows] [SeatsPerRow] format:\n>");
        string input = ReadLine();
        if (!string.IsNullOrEmpty(input))
        {

            WriteLine($"Welcome to GIC Cinemas");
            WriteLine($"[1] Book tickets for");
        }
    }
    private void PrintSeats(SeatMap seatmap)
    {
        WriteLine("\t\tSCREEN\t\t");
        //foreach ()
    }
    private void CheckReservation(string id)
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
    private void Reserve(int movie, int tickets, string seat)
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
