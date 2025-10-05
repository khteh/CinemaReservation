using CinemaReservation.Strategies;
using static System.Console;
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
    public List<char> Seats { get; private set; }
    public SeatRow(ISeatAllocationStrategy strategy, int seats)
    {
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
        List<int> seats = new List<int>();
        if (AvailableSeats() == 0)
        {
            WriteLine($"{nameof(Reserve)}: No available seats in this row!");
            return seats;
        }
        _index = _strategy.Allocate(_index, tickets, Seats, seats);
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
        List<int> seats = new List<int>();
        if (AvailableSeats() == 0)
        {
            WriteLine($"{nameof(Reserve)}: No available seats in this row!");
            return seats;
        }
        _index = _strategy.Allocate(seat, _index, tickets, Seats, seats);
        return seats;
    }
    public void Dispose()
    {
        Seats.Clear();
        GC.SuppressFinalize(this);
    }
}
