using CinemaReservation;
using CinemaReservation.Strategies;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using static System.Console;
namespace CinemaConsole;

public class CinemaConsoleApp
{
    private Cinema _cinema;
    private readonly ISeatAllocationStrategy _strategy;
    private Dictionary<string, Reservation> _reservations = new Dictionary<string, Reservation>();
    private readonly ILogger<CinemaConsoleApp> _logger;
    private readonly Regex _regex = new Regex(@"^([a-zA-Z]{1})([0-9]{2})$");
    public CinemaConsoleApp(ISeatAllocationStrategy strategy, ILogger<CinemaConsoleApp> logger, Cinema cinema)
    {
        _logger = logger;
        _strategy = strategy;
        _cinema = cinema;
    }
    public void Run(string[] args)
    {
        bool leave = false;
        string title = string.Empty, title_lower = string.Empty;
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
                    title_lower = title.ToLower();
                    rows = int.Parse(values[1].Trim());
                    seats = int.Parse(values[2].Trim());
                }
                if (!string.IsNullOrEmpty(title) && rows > 0 && rows <= 26 && seats > 0 && seats <= 50)
                {
                    _cinema.CreateMovie(title_lower, rows, seats);
                    movieCreated = true;
                }
            }
            else
            {
                WriteLine("Welcome to GIC Cinemas!");
                WriteLine($"[1] Book tickets for {title} ({_cinema.SeatsAvailable(title_lower)} seats available)");
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
                            HandleSeatReservation(title_lower);
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
    private void HandleSeatReservation(string title)
    {
        for (bool quit = false; !quit;)
        {
            WriteLine("Enter #tickets to purchase. [ENTER to return to main menu]:");
            Write("> ");
            string input = ReadLine();
            quit = string.IsNullOrEmpty(input);
            if (!quit)
            {
                _logger.LogInformation($"{nameof(CinemaConsoleApp)} input: {input}");
                int tickets = int.Parse(input.Trim());
                if (tickets > 0 && tickets <= _cinema.SeatsAvailable(title))
                {
                    Reservation reservation = _cinema.Reserve(title, tickets);
                    if (reservation != null && !string.IsNullOrEmpty(reservation.Id))
                    {
                        List<List<char>> map = new List<List<char>>();
                        _cinema.ShowMap(title, reservation.Id, map);
                        WriteLine($"Successfully reserved {tickets} {title} tickets!");
                        WriteLine($"Reservation Id: {reservation.Id}");
                        WriteLine($"Selected seats:");
                        WriteLine();
                        WriteLine("---------- SCREEN ----------");
                        for (int i = map.Count - 1; i >= 0; i--)
                        {
                            for (int j = 0; j < map[i].Count; j++)
                                Write(map[i][j] == ' ' ? '.' : map[i][j]);
                            WriteLine();
                        }
                        WriteLine("[ENTER] to accept seat selection OR enter a new seating (One alphabet for row and 2 digits for seat in the row):");
                        Write("> ");
                        input = ReadLine();
                        quit = string.IsNullOrEmpty(input);
                        if (!quit)
                            ;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Parse the input seat request string to it's corresponding row: [0, 25], cols: [1, min(_seatsPerRow , 50)]
    /// </summary>
    /// <param name="seat"></param>
    /// <returns></returns>
    private (int, int) ParseSeat(string seat, int rows, int seats)
    {
        int row = -1, col = -1; // _rows: [0, 25], _cols: [1, min(_seatsPerRow , 50)]
        MatchCollection matches = _regex.Matches(seat);
        _logger.LogDebug($"{matches.Count} matches");
        if (matches.Count > 0)
        {
            /* Report on each match.
             * If a match is found, information about this part of the matching string can be retrieved from the second Group object in the GroupCollection object returned by the Match.Groups property. 
             * The first element in the collection represents the entire match.
             */
            int _row = matches[0].Groups[1].Value.ToLower()[0] - 'a';
            if (Int32.TryParse(matches[0].Groups[2].Value, out int _col))
            {
                _logger.LogDebug($"{nameof(ParseSeat)} row: {_row}/{rows}, col: {_col}/{int.Min(seats, 50)}");
                if (_row >= 0 && _row < int.Min(rows, 26) && _col >= 1 && _col <= int.Min(seats, 50))
                {
                    row = _row;
                    col = _col;
                }
            }
        }
        return (row, col > 0 ? col - 1 : col);
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
