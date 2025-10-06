namespace CinemaReservation.Strategies;

/// <summary>
/// Strategy pattern to allocate seats in a specific row.
/// Used by SeatRow.Reserve to determine the best seats to reserve #tickets for.
/// </summary>
public interface ISeatAllocationStrategy
{
    /// <summary>
    /// Default seat allocation using the required strategy.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="tickets"></param>
    /// <param name="seats"></param>
    /// <param name="allocations"></param>
    /// <returns>Index of next available seat. -1 if none.</returns>
    public void Allocate(int index, int tickets, List<char> seats, List<int> allocations);
    /// <summary>
    /// Specific seat allocation using the required strategy.
    /// </summary>
    /// <param name="seat"></param>
    /// <param name="index"></param>
    /// <param name="tickets"></param>
    /// <param name="seats"></param>
    /// <param name="allocations"></param>
    /// <returns>Index of next available seat. -1 if none.</returns>
    public void Allocate(int seat, int index, int tickets, List<char> seats, List<int> allocations);
    /// <summary>
    /// Adjust index for the next reservation request. < 0 : This row is full
    /// </summary>
    /// <param name="index"></param>
    /// <param name="seats"></param>
    /// <returns></returns>
    public int AdjustIndex(int index, List<char> seats);
}