using CinemaReservation.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace CinemaReservation;

public class SeatMap : IDisposable
{
    private readonly string _title;
    private int _runningCount = 0;
    private List<SeatRow> _rows;
    private Dictionary<string, Reservation> _reservations;
    private readonly IRowAllocationStrategy _strategy;
    private readonly ILogger<SeatMap> _logger;
    public string Title { get => _title; }
    public SeatMap(IServiceProvider serviceProvider, ILogger<SeatMap> logger, IRowAllocationStrategy strategy, string title, int rows = 26, int seats = 50)
    {
        _logger = logger;
        _strategy = strategy;
        if (string.IsNullOrEmpty(title.Trim())) throw new ArgumentNullException(nameof(title));
        if (rows < 1 || rows > 26) throw new ArgumentOutOfRangeException(nameof(rows));
        if (seats < 1 || seats > 50) throw new ArgumentOutOfRangeException(nameof(seats));
        _title = title;
        _rows = new List<SeatRow>(); // _rows: [0, 25], _cols: [1, 50]
        _reservations = new Dictionary<string, Reservation>();
        for (int i = 0; i < rows; i++)
            _rows.Add((SeatRow)ActivatorUtilities.CreateInstance(serviceProvider, typeof(SeatRow), new object[] { seats }));
    }
    public int SeatsAvailable() => _rows.AsParallel().Sum(r => r.AvailableSeats());
    /// <summary>
    /// Reserve tickets starting from (row, seat) using 0-based index.
    /// </summary>
    /// <param name="id">Existing reservation id to change seats</param>
    /// <param name="tickets"></param>
    /// <param name="row"></param>
    /// <param name="seat"></param>
    /// <returns>Reservation</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public Reservation Reserve(string id, int tickets, int row = -1, int seat = -1)
    {
        Reservation reservation = null;
        Dictionary<int, List<int>> seats = new Dictionary<int, List<int>>();
        if (tickets <= 0) throw new ArgumentOutOfRangeException(nameof(tickets));
        if (tickets > SeatsAvailable())
        {
            _logger.LogError($"{nameof(Reserve)}: Not enough seats available!");
            return reservation;
        }
        if (row >= 0 && seat >= 0)
        {
            _logger.LogDebug($"{nameof(Reserve)} Reserving {tickets} seats from ({row}, {seat})");
            List<int> reserved = _rows[row].Reserve(seat, tickets);
            if (reserved.Any())
            {
                tickets -= reserved.Count;
                seats[row] = reserved;
            }
            row++;
        }
        else
            row = 0;
        tickets = _strategy.Allocate(row, tickets, _rows, seats);
        if (tickets > 0)
            throw new InvalidOperationException($"{nameof(Reserve)} Failed to reserve {tickets} remaining seats!");
        if (string.IsNullOrEmpty(id) || !_reservations.ContainsKey(id))
        {
            id = $"GIC{_runningCount.ToString("D4")}";
            reservation = new Reservation(id, seats);
            _reservations.Add(id, reservation);
            Interlocked.Increment(ref _runningCount);
        }
        else
        {
            _reservations[id].UpdateSeats(seats);
            reservation = _reservations[id];
        }
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
    public void ShowMap(string id, List<List<char>> map)
    {
        if (string.IsNullOrEmpty(id) || !_reservations.ContainsKey(id)) throw new ArgumentOutOfRangeException(nameof(id));
        if (!_reservations.ContainsKey(id.Trim())) throw new ArgumentOutOfRangeException(nameof(id));
        Reservation reservation = _reservations[id.Trim()];
        for (int i = 0; i < _rows.Count; i++)
        {
            map.Add(new List<char>(_rows[i].Seats));
            for (int j = 0; j < map[i].Count; j++)
                if (reservation.Seats.ContainsKey(i) && reservation.Seats[i].Contains(j))
                    map[i][j] = '#';
        }
    }
    public void Dispose()
    {
        _rows.Clear();
        _reservations.Clear();
        GC.SuppressFinalize(this);
    }
}
