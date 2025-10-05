using CinemaReservation;
using CinemaReservation.Strategies;
using static System.Console;
namespace CinemaConsole;

public class CinemaConsoleApp
{
    private Cinema _cinema;
    private readonly ISeatAllocationStrategy _strategy;
    private Dictionary<string, Reservation> _reservations = new Dictionary<string, Reservation>();
    public CinemaConsoleApp(ISeatAllocationStrategy strategy)
    {
        _strategy = strategy;
        _cinema = new Cinema(_strategy);
    }
    public void Run(string[] args)
    {
        bool leave = false;
        string title = string.Empty;
        int rows = -1, seats = -1;
        bool movieCreated = false;
        while (!leave)
        {
            if (string.IsNullOrEmpty(title) || rows < 0 || seats < 0 || !movieCreated)
            {
                WriteLine($"Please define movie title and seat map in [Title] [Rows] [SeatsPerRow] format:");
                Write("> ");
                string input = ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    string[] values = input.Split(' ');
                    title = values[0].Trim();
                    rows = int.Parse(values[1].Trim());
                    seats = int.Parse(values[2].Trim());
                }
                if (!string.IsNullOrEmpty(title) && rows > 0 && rows <= 26 && seats > 0 && seats <= 50)
                {
                    _cinema.CreateMovie(title.ToLower(), rows, seats);
                    movieCreated = true;
                }
            }
            else
            {
                WriteLine("Welcome to GIC Cinemas!");
                WriteLine($"[1] Book tickets for {title} ({_cinema.SeatsAvailable(title.ToLower())} seats available)");
                WriteLine("[2] Check reservations");
                WriteLine("[3] Exit");
                WriteLine("Please enter your selection:");
                Write("> ");
                string input = ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    switch (input)
                    {
                        case "1":
                            WriteLine("Enter #tickets to purchase. [ENTER to return to main menu]:");
                            Write("> ");
                            input = ReadLine();
                            if (!string.IsNullOrEmpty(input))
                            {
                                int tickets = int.Parse(input.Trim());
                                if (tickets > 0 && tickets <= _cinema.SeatsAvailable(title.ToLower()))
                                {
                                }
                            }
                            break;
                        case "2":
                            break;
                        case "3":
                            leave = true;
                            break;
                    }
                }
            }
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
        }
        catch (Exception e)
        {
            WriteLine($"Exception! {e}");
        }
    }
}
