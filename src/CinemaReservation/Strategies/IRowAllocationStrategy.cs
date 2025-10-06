namespace CinemaReservation.Strategies;

/// <summary>
/// Strategy pattern to allocate a row.
/// Used by SeatMap.Reserve to determine which row to allocate seats in.
/// </summary>
public interface IRowAllocationStrategy
{
    public int Allocate(int row, int tickets, List<SeatRow> rows, Dictionary<int, List<int>> seats);
}
