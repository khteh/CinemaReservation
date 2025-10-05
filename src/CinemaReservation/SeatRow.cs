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
    public List<char> Seats { get; private set; }
    public SeatRow(int seats)
    {
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
        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x		<= (10 - 4) / 2 = 3

        0 1 2 3 4 5 6 7 8 9
              x x x			<= (10 - 3) / 2 = 3.5 (floor)

        0 1 2 3 4 5 6 7 8 9
                x x			<= (10 - 2) / 2 = 4

        0 1 2 3 4 5 6 7 8 9 10
              x x x x		<= (11 - 4) / 2 = 3.5 (floor)

        0 1 2 3 4 5 6 7 8 9 10
                x x x		<= (11 - 3) / 2 = 4

        0 1 2 3 4 5 6 7 8 9 10
                x x			<= (11 - 2) / 2 = 4.5 (floor)     
        */
        List<int> seats = new List<int>();
        if (AvailableSeats() == 0)
        {
            WriteLine($"{nameof(Reserve)}: No available seats in this row!");
            return seats;
        }
        int _tickets = Tickets(tickets);
        int offset = Offset(tickets);
        int i = offset;
        for (; i < offset + _tickets; i++)
        {
            Seats[i] = 'x';
            seats.Add(i);
        }
        _index = offset + _tickets;
        AdjustOffset();
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
        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x		<= (10 - 4) / 2 = 3

        0 1 2 3 4 5 6 7 8 9
              x x x			<= (10 - 3) / 2 = 3.5 (floor)

        0 1 2 3 4 5 6 7 8 9
                x x			<= (10 - 2) / 2 = 4

        0 1 2 3 4 5 6 7 8 9 10
              x x x x		<= (11 - 4) / 2 = 3.5 (floor)

        0 1 2 3 4 5 6 7 8 9 10
                x x x		<= (11 - 3) / 2 = 4

        0 1 2 3 4 5 6 7 8 9 10
                x x			<= (11 - 2) / 2 = 4.5 (floor)     
        */
        List<int> seats = new List<int>();
        if (AvailableSeats() == 0)
        {
            WriteLine($"{nameof(Reserve)}: No available seats in this row!");
            return seats;
        }
        int i = seat;
        for (; i < Seats.Count && seats.Count < tickets; i++)
            if (Seats[i] == ' ')
            {
                Seats[i] = 'x';
                seats.Add(i);
            }
        for (; i < Seats.Count && Seats[i] == 'x'; i++) ;
        _index = i;
        AdjustOffset();
        return seats;
    }
    /// <summary>
    /// Adjust _index for the next reservation request. < 0 : This row is full
    /// </summary>
    private void AdjustOffset()
    {
        if (_index == Seats.Count || Seats[_index] == 'x')
            // Check and reset _index to the right-most empty seat
            for (--_index; _index >= 0 && Seats[_index] == 'x'; _index--) ;
    }
    /// <summary>
    /// Calculate the center-most offset of the beginning of seat assignment for the number of required tickets in this row
    /// </summary>
    /// <param name="tickets"></param>
    /// <returns>offset</returns>
    private int Offset(int tickets)
    {
        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x		<= (10 - 4) / 2 = 3

        0 1 2 3 4 5 6 7 8 9
              x x x			<= (10 - 3) / 2 = 3.5 (floor)

        0 1 2 3 4 5 6 7 8 9
                x x			<= (10 - 2) / 2 = 4

        0 1 2 3 4 5 6 7 8 9 10
              x x x x		<= (11 - 4) / 2 = 3.5 (floor)

        0 1 2 3 4 5 6 7 8 9 10
                x x x		<= (11 - 3) / 2 = 4

        0 1 2 3 4 5 6 7 8 9 10
                x x			<= (11 - 2) / 2 = 4.5 (floor)     
        */
        int size = EmptySeatsToTheRight(_index);
        if (size > 1)
        {
            int _tickets = int.Min(size, tickets);
            int oddEven = (_tickets % 2) ^ (size % 2);
            return _index + ((oddEven == 0) ? ((size - _tickets) / 2) : (int)Math.Floor((size - _tickets) / 2.0));
        }
        else // size == 1
        {
            /*
            0 1 2 3 4 5 6 7 8 9
                  x x x x x x

            0 1 2 3 4 5 6 7 8 9
                x x x x x x x x
             */
            if (size == tickets)
                return _index;
            /* Available seats: [0, _index]
             * Return the offset which could accomodate the min(_index + 1, tickets)
             */
            return tickets >= _index + 1 ? 0 : (_index + 1) - tickets;
        }
    }
    /// <summary>
    /// Calculate how many of the tickets requested which can be fulfilled in this row
    /// </summary>
    /// <param name="tickets"></param>
    /// <returns>#tickets</returns>
    private int Tickets(int tickets)
    {
        int size = EmptySeatsToTheRight(_index);
        if (size == 1 && tickets > size)
        {
            int i = _index;
            // Look for empty seats from _index to the left
            for (; i >= 0 && Seats[i] == ' '; i--) ;
            if (i < 0)
                i = 0;
            size = _index - i + 1;
        }
        return int.Min(size, tickets);
    }
    /// <summary>
    /// Check if there is any empty seat to the right of input parameter offset.
    /// </summary>
    /// <param name="offset"></param>
    /// <returns># empty seats to the right of offset</returns>
    private int EmptySeatsToTheRight(int offset)
    {
        int i = _index;
        // Check if there is any empty seat to the left of _index
        for (; i < Seats.Count && Seats[i] == ' '; i++) ;
        return i - _index; // size = 1 means there is no empty seat to the left of _index
    }

    public void Dispose()
    {
        Seats.Clear();
        GC.SuppressFinalize(this);
    }
}
