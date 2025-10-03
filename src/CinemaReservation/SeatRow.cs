using static System.Console;
namespace CinemaReservation;

public class SeatRow
{
    /// <summary>
    /// < 0 : This row is full
    /// The center-most seat to reserve from this row.
    /// Adjusted after every successful reservation.
    /// </summary>
    private int _offset = 0;
    private List<char> _seats;
    public SeatRow(int seats)
    {
        _seats = new List<char>();
        for (int i = 0; i < seats; i++)
            _seats.Add(' ');
    }
    public int AvailableSeats() => _offset >= 0 ? _seats.Where(s => s.Equals(' ')).Count() : 0;
    /// <summary>
    /// Reserve the number of tickets in this seat row
    /// </summary>
    /// <param name="tickets"></param>
    /// <returns>#tickets actually reserved in this row</returns>
    public int Reserve(int tickets)
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
        if (AvailableSeats() == 0)
        {
            WriteLine($"{nameof(Reserve)}: No available seats in this row!");
            return 0;
        }
        int _tickets = Tickets(tickets);
        int offset = Offset(tickets);
        int i = offset;
        for (; i < offset + _tickets; i++)
            _seats[i] = 'x';
        _offset = offset + _tickets;
        if (_offset == _seats.Count || _seats[_offset] == 'x')
            // Check and reset _offset to the right-most empty seat
            for (--_offset; _offset >= 0 && _seats[_offset] == 'x'; _offset--) ;
        return _tickets;
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
        int size = EmptySeatsToTheRight(_offset);
        if (size > 1)
        {
            int _tickets = int.Min(size, tickets);
            int oddEven = (_tickets % 2) ^ (size % 2);
            return _offset + ((oddEven == 0) ? ((size - _tickets) / 2) : (int)Math.Floor((size - _tickets) / 2.0));
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
                return _offset;
            int _tickets = int.Min(_offset + 1, tickets);
            return _tickets - tickets;
        }
    }
    /// <summary>
    /// Calculate how many of the tickets requested which can be fulfilled in this row
    /// </summary>
    /// <param name="tickets"></param>
    /// <returns>#tickets</returns>
    private int Tickets(int tickets)
    {
        int size = EmptySeatsToTheRight(_offset);
        if (size == 1 && tickets > size)
        {
            int i = _offset;
            // Look for empty seats from _offset to the left
            for (; i >= 0 && _seats[i] == ' '; i--) ;
            if (i < 0)
                i = 0;
            size = _offset - i + 1;
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
        int i = _offset;
        // Check if there is any empty seat to the left of _offset
        for (; i < _seats.Count && _seats[i] == ' '; i++) ;
        return i - _offset; // size = 1 means there is no empty seat to the left of _offset
    }
}
