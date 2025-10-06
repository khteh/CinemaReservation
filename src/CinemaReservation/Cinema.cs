using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace CinemaReservation;

public class Cinema
{
    private Dictionary<string, SeatMap> _seatMap;
    private readonly ILogger<Cinema> _logger;
    private readonly IServiceProvider _serviceProvider;
    public Cinema(IServiceProvider serviceProvider, ILogger<Cinema> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _seatMap = new Dictionary<string, SeatMap>();
    }
    public int CreateMovie(string title, int rows, int seats)
    {
        title = title.Trim().ToLower();
        if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(nameof(title));
        if (rows < 0) throw new ArgumentOutOfRangeException(nameof(rows));
        if (seats < 0) throw new ArgumentOutOfRangeException(nameof(seats));
        if (!_seatMap.ContainsKey(title))
            _seatMap.Add(title, (SeatMap)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(SeatMap), new object[] { title, rows, seats }));
        return _seatMap[title].SeatsAvailable();
    }
    public int SeatsAvailable(string title) => _seatMap.ContainsKey(title) ? _seatMap[title].SeatsAvailable() : 0;
    /// <summary>
    /// Reserve tickets for movie "title", starting from (row, seat) using 0-based index.
    /// </summary>
    /// <param name="id">Existing reservation id to change seats</param>
    /// <param name="title"></param>
    /// <param name="tickets"></param>
    /// <param name="row"></param>
    /// <param name="seat"></param>
    /// <returns>Reservation</returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Reservation Reserve(string id, string title, int tickets, int row = -1, int seat = -1)
    {
        title = title.Trim();
        string title_lower = title.ToLower();
        if (!_seatMap.ContainsKey(title_lower))
            throw new InvalidOperationException($"Invalid movie title! {title}");
        if (tickets <= 0) throw new ArgumentOutOfRangeException(nameof(tickets));
        if (tickets > _seatMap[title_lower].SeatsAvailable())
        {
            _logger.LogError($"{nameof(Reserve)}: Not enough seats available!");
            return null;
        }
        return _seatMap[title_lower].Reserve(id, tickets, row, seat);
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
    public bool ShowMap(string title, string id, List<List<char>> map)
    {
        string title_lower = title.Trim().ToLower();
        id = id.Trim();
        if (string.IsNullOrEmpty(title_lower)) throw new ArgumentNullException(nameof(title));
        return _seatMap.ContainsKey(title_lower) && _seatMap[title_lower].HasReservation(id) && _seatMap[title_lower].ShowMap(id, map);
    }
}
