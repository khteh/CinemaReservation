namespace CinemaReservation.Strategies;

public class BackRowAllocationStrategy : IRowAllocationStrategy
{
    /// <summary>
    /// Default seats reservation. Start from the back row.
    /// </summary>
    /// <param name="tickets"></param>
    /// <param name="rows"></param>
    /// <param name="seats"></param>
    /// <returns>#tickets reserved</returns>
    public int Allocate(int row, int tickets, List<SeatRow> rows, Dictionary<int, List<int>> seats)
    {
        for (; row < rows.Count && tickets > 0; row++)
            if (rows[row].AvailableSeats() > 0)
            {
                List<int> reserved = rows[row].Reserve(tickets);
                if (reserved.Any())
                {
                    tickets -= reserved.Count;
                    seats[row] = reserved;
                }
            }
        return tickets;
    }
}
