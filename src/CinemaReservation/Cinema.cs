using CinemaReservation.Strategies;
using static System.Console;
namespace CinemaReservation;

public class Cinema
{
    private readonly ISeatAllocationStrategy _strategy;
    private Dictionary<string, SeatMap> _seatMap;
    public Cinema(ISeatAllocationStrategy strategy)
    {
        _strategy = strategy;
        _seatMap = new Dictionary<string, SeatMap>();
    }
    public int CreateMovie(string title, int rows, int seats)
    {
        title = title.Trim().ToLower();
        if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(nameof(title));
        if (rows < 0) throw new ArgumentOutOfRangeException(nameof(rows));
        if (seats < 0) throw new ArgumentOutOfRangeException(nameof(seats));
        if (!_seatMap.ContainsKey(title))
            _seatMap.Add(title, new SeatMap(_strategy, title, rows, seats));
        return _seatMap[title].SeatsAvailable();
    }
    public int SeatsAvailable(string title) => _seatMap.ContainsKey(title) ? _seatMap[title].SeatsAvailable() : 0;
    public Reservation Reserve(string title, int tickets, string seat)
    {
        title = title.Trim();
        string title_lower = title.ToLower();
        seat = seat.Trim().ToLower();
        if (!_seatMap.ContainsKey(title_lower))
            throw new InvalidOperationException($"Invalid movie title! {title}");
        if (tickets <= 0) throw new ArgumentOutOfRangeException(nameof(tickets));
        if (tickets > _seatMap[title_lower].SeatsAvailable())
        {
            WriteLine($"Not enough seats available!");
            return null;
        }
        return _seatMap[title_lower].Reserve(tickets, seat);
    }
    public bool Confirm(string title, string id)
    {
        string title_lower = title.Trim().ToLower();
        id = id.Trim();
        if (string.IsNullOrEmpty(title_lower)) throw new ArgumentNullException(nameof(title));
        if (!_seatMap.ContainsKey(title_lower))
            throw new InvalidOperationException($"Invalid movie title! {title}");
        return _seatMap[title_lower].ConfirmReservation(id);
    }
}
