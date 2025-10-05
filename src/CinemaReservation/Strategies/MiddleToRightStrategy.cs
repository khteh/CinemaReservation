namespace CinemaReservation.Strategies;

public class MiddleToRightStrategy : ISeatAllocationStrategy
{
    /// <summary>
    /// Allocate seats starting from the middle-most seat and moving to the right.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="seats"></param>
    /// <param name="allocations"></param>
    /// <returns>Index of next available seat. -1 if none.</returns>
    public int Allocate(int index, int tickets, List<char> seats, List<int> allocations)
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
        if (index < 0) // This row is full
            return index;
        int _tickets = Tickets(index, tickets, seats);
        int _index = Index(index, tickets, seats);
        int i = _index;
        for (; i < _index + _tickets; i++)
        {
            seats[i] = 'x';
            allocations.Add(i);
        }
        index = _index + _tickets;
        return AdjustIndex(index, seats);
    }
    /// <summary>
    /// Allocate seats starting from the requested seat and moving to the right.
    /// </summary>
    /// <param name="seat"></param>
    /// <param name="index"></param>
    /// <param name="tickets"></param>
    /// <param name="seats"></param>
    /// <param name="allocations"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public int Allocate(int seat, int index, int tickets, List<char> seats, List<int> allocations)
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
        if (index < 0) // This row is full
            return index;
        int i = seat;
        for (; i < seats.Count && allocations.Count < tickets; i++)
            if (seats[i] == ' ')
            {
                seats[i] = 'x';
                allocations.Add(i);
            }
        for (; i < seats.Count && seats[i] == 'x'; i++) ;
        index = i;
        return AdjustIndex(index, seats);
    }
    /// <summary>
    /// Adjust index for the next reservation request. < 0 : This row is full
    /// </summary>
    private int AdjustIndex(int index, List<char> seats)
    {
        if (index == seats.Count || seats[index] == 'x')
            // Check and reset index to the right-most empty seat
            for (--index; index >= 0 && seats[index] == 'x'; index--) ;
        return index;
    }
    /// <summary>
    /// Calculate the center-most index of the beginning of seat assignment for the number of required tickets in this row
    /// </summary>
    /// <param name="tickets"></param>
    /// <returns>index</returns>
    private int Index(int index, int tickets, List<char> seats)
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
        int size = EmptySeatsToTheRight(index, seats);
        if (size > 1)
        {
            int _tickets = int.Min(size, tickets);
            int oddEven = (_tickets % 2) ^ (size % 2);
            return index + ((oddEven == 0) ? ((size - _tickets) / 2) : (int)Math.Floor((size - _tickets) / 2.0));
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
                return index;
            /* Available seats: [0, index]
             * Return the index which could accomodate the min(index + 1, tickets)
             */
            return tickets >= index + 1 ? 0 : (index + 1) - tickets;
        }
    }
    /// <summary>
    /// Calculate how many of the tickets requested which can be fulfilled in this row
    /// </summary>
    /// <param name="tickets"></param>
    /// <returns>#tickets</returns>
    private int Tickets(int index, int tickets, List<char> seats)
    {
        int size = EmptySeatsToTheRight(index, seats);
        if (size == 1 && tickets > size)
        {
            int i = index;
            // Look for empty seats from index to the left
            for (; i >= 0 && seats[i] == ' '; i--) ;
            if (i < 0)
                i = 0;
            size = index - i + 1;
        }
        return int.Min(size, tickets);
    }
    /// <summary>
    /// Check if there is any empty seat to the right of input parameter index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns># empty seats to the right of index</returns>
    private int EmptySeatsToTheRight(int index, in List<char> seats)
    {
        int i = index;
        // Check if there is any empty seat to the left of index
        for (; i < seats.Count && seats[i] == ' '; i++) ;
        return i - index; // size = 1 means there is no empty seat to the left of index
    }
}
