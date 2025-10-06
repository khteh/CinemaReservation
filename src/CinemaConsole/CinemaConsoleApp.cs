using CinemaReservation;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using static System.Console;
namespace CinemaConsole;

public class CinemaConsoleApp
{
    private Cinema _cinema;
    private readonly ILogger<CinemaConsoleApp> _logger;
    private readonly Regex _regex = new Regex(@"^([a-zA-Z]{1})([0-9]{2})$");
    private int _rows = -1, _seats = -1;
    public CinemaConsoleApp(ILogger<CinemaConsoleApp> logger, Cinema cinema)
    {
        _logger = logger;
        _cinema = cinema;
    }
    public void Run(string[] args)
    {
        bool leave = false;
        string title = string.Empty, title_lower = string.Empty;
        bool movieCreated = false;
        while (!leave)
        {
            if (string.IsNullOrEmpty(title) || _rows < 0 || _seats < 0 || !movieCreated)
            {
                WriteLine($"Please define movie title and seat map in [Title] [Rows] [SeatsPerRow] format:");
                Write("> ");
                string input = ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    string[] values = input.Split(' ');
                    title = values[0].Trim();
                    title_lower = title.ToLower();
                    _rows = int.Parse(values[1].Trim());
                    _seats = int.Parse(values[2].Trim());
                }
                if (!string.IsNullOrEmpty(title) && _rows > 0 && _rows <= 26 && _seats > 0 && _seats <= 50)
                {
                    _cinema.CreateMovie(title_lower, _rows, _seats);
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
                            HandleCheckReservation(title_lower);
                            break;
                        case "3":
                            WriteLine($"Thank you for visiting GIC Cinema. Ciao!");
                            leave = true;
                            break;
                    }
                }
            }
        }
    }
    private void HandleCheckReservation(string title)
    {
        for (bool quit = false; !quit;)
        {
            WriteLine("Enter your reservation Id. [ENTER to return to main menu]:");
            Write("> ");
            string input = ReadLine();
            quit = string.IsNullOrEmpty(input);
            if (!quit)
            {
                _logger.LogInformation($"{nameof(HandleCheckReservation)} input: {input}");
                List<List<char>> map = new List<List<char>>();
                _cinema.ShowMap(title, input.Trim(), map);
                WriteLine($"Reservation Id: {input.Trim()}");
                WriteLine($"Selected seats:");
                WriteLine();
                WriteLine("---------- SCREEN ----------");
                for (int i = map.Count - 1; i >= 0; i--)
                {
                    for (int j = 0; j < map[i].Count; j++)
                        Write(map[i][j] == ' ' ? '.' : map[i][j]);
                    WriteLine();
                }
            }
        }
    }
    private void HandleSeatReservation(string title)
    {
        int tickets = 0;
        Reservation reservation = null;
        for (bool quit = false; !quit;)
        {
            WriteLine("Enter #tickets to purchase. [ENTER to return to main menu]:");
            Write("> ");
            string input = ReadLine();
            _logger.LogInformation($"{nameof(HandleSeatReservation)} input: {input}");
            quit = string.IsNullOrEmpty(input);
            if (!quit)
            {
                tickets = int.Parse(input.Trim());
                if (tickets < 0 || tickets > _cinema.SeatsAvailable(title))
                    WriteLine($"Only {_cinema.SeatsAvailable(title)} tickets available for '{title}'");
                else
                    quit = true;
            }
        }
        for (bool quit = false; !quit;)
        {
            if (tickets > 0 && tickets <= _cinema.SeatsAvailable(title))
            {
                reservation = _cinema.Reserve(string.Empty, title, tickets);
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
                    string input = ReadLine();
                    if (string.IsNullOrEmpty(input))
                    {
                        if (_cinema.Confirm(title, reservation.Id))
                        {
                            WriteLine($"Your reservation {reservation.Id} is confirmed. Enjoy the movie!");
                            quit = true;
                        }
                    }
                    else
                    {
                        (int row, int seat) = ParseSeat(input.Trim());
                        if (row >= 0 && seat >= 0)
                        {
                            _logger.LogInformation($"{nameof(HandleSeatReservation)} Reserving {tickets} from seat {input.Trim()} -> ({row},{seat})");
                            reservation = _cinema.Reserve(reservation.Id, title, tickets, row, seat);
                        }
                        else
                            WriteLine($"Invalid seat selection {input.Trim()}. Rows start from 'A' and ends at {(char)('A' + _rows - 1)}, seats starts from 1 and ends at {_seats}");
                    }
                }
            }
            else
                WriteLine($"Only {_cinema.SeatsAvailable(title)} tickets available for '{title}'");
        }
    }
    /// <summary>
    /// Parse the input seat request string to it's corresponding row: [0, 25], cols: [1, min(_seatsPerRow , 50)]
    /// </summary>
    /// <param name="seat"></param>
    /// <returns></returns>
    private (int, int) ParseSeat(string seat)
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
                _logger.LogDebug($"{nameof(ParseSeat)} row: {_row}/{_rows}, col: {_col}/{int.Min(_seats, 50)}");
                if (_row >= 0 && _row < int.Min(_rows, 26) && _col >= 1 && _col <= int.Min(_seats, 50))
                {
                    row = _row;
                    col = _col;
                }
            }
        }
        return (row, col > 0 ? col - 1 : col);
    }
}
