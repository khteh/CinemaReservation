using CinemaReservation.Strategies;
using Microsoft.Extensions.Logging;
namespace CinemaReservation;

public class SeatRow : IDisposable
{
    /// <summary>
    /// < 0 : This row is full
    /// The center-most seat to reserve from this row.
    /// Adjusted after every successful reservation.
    /// </summary>
    private int _index = 0;
    private readonly ISeatAllocationStrategy _strategy;
    private readonly ILogger<SeatRow> _logger;
    public List<char> Seats { get; private set; }
    public SeatRow(ILogger<SeatRow> logger, ISeatAllocationStrategy strategy, int seats)
    {
        _logger = logger;
        _strategy = strategy;
        Seats = new List<char>();
        for (int i = 0; i < seats; i++)
            Seats.Add(' ');
    }
    public int AvailableSeats() => _index >= 0 ? Seats.Where(s => s.Equals(' ')).Count() : 0;
    /// <summary>
    /// Reserve available seats out of the requested number of tickets in this seat row.
    /// </summary>
    /// <param name="tickets"></param>
    /// <returns>List of seats successfully reserved out of the requested number of tickets in this seat row.</returns>
    public List<int> Reserve(int tickets)
    {
        int index = _index;
        List<int> seats = new List<int>();
        if (AvailableSeats() == 0)
        {
            _logger.LogError($"{nameof(Reserve)}: No available seats in this row!");
            return seats;
        }
        //_index = _strategy.Allocate(_index, tickets, Seats, seats); Pending confirmation
        List<char> _seats = new(Seats);
        _strategy.Allocate(index, tickets, _seats, seats);
        return seats;
    }
    /// <summary>
    /// Reserve available seats out of the requested number of tickets from the requested seat position to the right of the row.
    /// </summary>
    /// <param name="seat">[0, Seats.Count - 1]</param>
    /// <param name="tickets"></param>
    /// <returns>List of seats successfully reserved out of the requested number of tickets in this seat row.</returns>
    public List<int> Reserve(int seat, int tickets)
    {
        int index = _index;
        List<int> seats = new List<int>();
        if (AvailableSeats() == 0)
        {
            _logger.LogError($"{nameof(Reserve)}: No available seats in this row!");
            return seats;
        }
        //_index = _strategy.Allocate(seat, _index, tickets, Seats, seats); pending confirmation
        List<char> _seats = new(Seats);
        _strategy.Allocate(seat, index, tickets, _seats, seats);
        return seats;
    }
    public bool Confirm(List<int> seats)
    {
        foreach (int seat in seats)
            Seats[seat] = 'x';
        _index = seats.Last() + 1;
        _index = _strategy.AdjustIndex(_index, Seats);
        return true;
    }
    public void Dispose()
    {
        Seats.Clear();
        GC.SuppressFinalize(this);
    }
}
