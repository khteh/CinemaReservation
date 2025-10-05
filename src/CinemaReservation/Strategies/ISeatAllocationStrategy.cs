namespace CinemaReservation.Strategies;

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
    public int Allocate(int index, int tickets, List<char> seats, List<int> allocations);
    /// <summary>
    /// Specific seat allocation using the required strategy.
    /// </summary>
    /// <param name="seat"></param>
    /// <param name="index"></param>
    /// <param name="tickets"></param>
    /// <param name="seats"></param>
    /// <param name="allocations"></param>
    /// <returns>Index of next available seat. -1 if none.</returns>
    public int Allocate(int seat, int index, int tickets, List<char> seats, List<int> allocations);
}