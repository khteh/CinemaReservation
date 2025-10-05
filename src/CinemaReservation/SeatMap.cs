using CinemaReservation.Strategies;
using System.Text.RegularExpressions;
using static System.Console;
namespace CinemaReservation;

public class SeatMap : IDisposable
{
    private readonly Regex _regex = new Regex(@"^([a-zA-Z]{1})([0-9]{2})$");
    private readonly string _title;
    private int _seatsPerRow;
    private int _runningCount = 0;
    private List<SeatRow> _rows;
    private Dictionary<string, Reservation> _reservations;
    public string Title { get => _title; }
    public SeatMap(ISeatAllocationStrategy strategy, string title, int rows = 26, int seats = 50)
    {
        if (string.IsNullOrEmpty(title.Trim())) throw new ArgumentNullException(nameof(title));
        if (rows < 1 || rows > 26) throw new ArgumentOutOfRangeException(nameof(rows));
        if (seats < 1 || seats > 50) throw new ArgumentOutOfRangeException(nameof(seats));
        _title = title;
        _rows = new List<SeatRow>(); // _rows: [0, 25], _cols: [1, 50]
        _reservations = new Dictionary<string, Reservation>();
        for (int i = 0; i < rows; i++)
            _rows.Add(new SeatRow(strategy, seats));
        _seatsPerRow = _rows.Any() ? _rows.First().Seats.Count : 0;
    }
    public int SeatsAvailable() => _rows.AsParallel().Sum(r => r.AvailableSeats());
    public Reservation Reserve(int tickets, string seat)
    {
        Reservation reservation = null;
        Dictionary<int, List<int>> seats = new Dictionary<int, List<int>>();
        if (tickets <= 0) throw new ArgumentOutOfRangeException(nameof(tickets));
        if (tickets > SeatsAvailable())
        {
            WriteLine($"Not enough seats available!");
            return reservation;
        }
        int row = -1, col = -1;
        if (!string.IsNullOrEmpty(seat))
        {
            (row, col) = ParseSeat(seat);
            if (row < 0 || col < 1)
                throw new ArgumentOutOfRangeException(nameof(seat));
            WriteLine($"{nameof(Reserve)} Reserving {tickets} seats from ({row}, {col - 1})");
            List<int> reserved = _rows[row].Reserve(col - 1, tickets);
            if (reserved.Any())
            {
                tickets -= reserved.Count;
                seats[row] = reserved;
            }
            row++;
        }
        else
            row = 0;
        /* Default seats reservation.
         * Back row, middle seats.
         */
        for (int i = row; i < _rows.Count && tickets > 0; i++)
            if (_rows[i].AvailableSeats() > 0)
            {
                List<int> reserved = _rows[i].Reserve(tickets);
                if (reserved.Any())
                {
                    tickets -= reserved.Count;
                    seats[i] = reserved;
                }
            }
        if (tickets > 0)
            throw new InvalidOperationException($"{nameof(Reserve)} Failed to reserve {tickets} remaining seats!");
        string id = $"GIC{_runningCount.ToString("D4")}";
        reservation = new Reservation(id, seats);
        _reservations.Add(id, reservation);
        Interlocked.Increment(ref _runningCount);
        return reservation;
    }
    public bool ConfirmReservation(string id)
    {
        if (string.IsNullOrEmpty(id.Trim())) throw new ArgumentNullException(nameof(id));
        if (!_reservations.ContainsKey(id.Trim())) throw new ArgumentOutOfRangeException(nameof(id));
        Reservation reservation = _reservations[id.Trim()];
        if (!reservation.Confirmed)
        {
            foreach (KeyValuePair<int, List<int>> kv in reservation.Seats)
                _rows[kv.Key].Confirm(kv.Value);
            reservation.Confirmed = true;
        }
        return true;
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
                WriteLine($"{nameof(ParseSeat)} row: {_row}/{_rows.Count}, col: {_col}/{int.Min(_seatsPerRow, 50)}");
                if (_row >= 0 && _row < int.Min(_rows.Count, 26) && _col >= 1 && _col <= int.Min(_seatsPerRow, 50))
                {
                    row = _row;
                    col = _col;
                }
            }
        }
        return (row, col);
    }
    public void ShowMap(string id)
    {
        if (string.IsNullOrEmpty(id) || !_reservations.ContainsKey(id)) throw new ArgumentOutOfRangeException(nameof(id));
        WriteLine("\t\t{_title}");
        List<List<char>> rows = new List<List<char>>();
        for (int i = _rows.Count - 1; i >= 0; i--)
        {
            List<char> row = new List<char>();
            //for (int j = 0; j < _rows[i].Seats.Count; j++)
        }
    }
    public void Dispose()
    {
        _rows.Clear();
        _reservations.Clear();
        GC.SuppressFinalize(this);
    }
}
