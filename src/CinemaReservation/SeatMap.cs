using System.Text.RegularExpressions;
using static System.Console;
namespace CinemaReservation;

public class SeatMap
{
    private readonly Regex _regex = new Regex(@"^([a-zA-Z]{1})([0-9]{2})$");
    private readonly string _title;
    private List<SeatRow> _rows;
    public string Title { get => _title; }
    public int Seats { get => _rows.Any() ? _rows.First().Seats.Count : 0; }
    public int RowCount { get => _rows.Count; }
    public SeatMap(string title, int rows, int seats)
    {
        if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(nameof(title));
        if (rows < 1 || rows > 26) throw new ArgumentOutOfRangeException(nameof(rows));
        if (seats < 1 || seats > 50) throw new ArgumentOutOfRangeException(nameof(seats));
        _title = title;
        _rows = new List<SeatRow>(); // _rows: [0, 25], _cols: [1, 50]
        for (int i = 0; i < rows; i++)
            _rows.Add(new SeatRow(seats));
    }
    public int SeatsAvailable() => _rows.AsParallel().Sum(r => r.AvailableSeats());
    public string Reserve(int tickets, string seat, out Dictionary<int, List<int>> seats)
    {
        seats = null;
        if (tickets > SeatsAvailable())
        {
            WriteLine($"Not enough seats available!");
            return null;
        }
        if (string.IsNullOrEmpty(seat))
        {
            /* Default seats reservation.
             * Back row, middle seats.
             */
            for (int i = 0; i < _rows.Count && tickets > 0; i++)
            {
            }
        }
        return null;
    }
    private (int, int) ParseSeat(string seat)
    {
        int row = -1, col = -1; // _rows: [0, 25], _cols: [1, min(_seatMap._seats , 50)]
        MatchCollection matches = _regex.Matches(seat);
        WriteLine($"{matches.Count} matches");
        if (matches.Count > 0)
        {
            /* Report on each match.
             * If a match is found, information about this part of the matching string can be retrieved from the second Group object in the GroupCollection object returned by the Match.Groups property. 
             * The first element in the collection represents the entire match.
             */
            int _row = matches[0].Groups[1].Value.ToLower()[0] - 'a';
            if (Int32.TryParse(matches[0].Groups[2].Value, out int _col))
            {
                WriteLine($"{nameof(ParseSeat)} row: {_row}/{RowCount}, col: {_col}/{int.Min(Seats, 50)}");
                if (_row >= 0 && _row < int.Min(RowCount, 26) && _col >= 1 && _col <= int.Min(Seats, 50))
                {
                    row = _row;
                    col = _col;
                }
            }
        }
        return (row, col);
    }
}
