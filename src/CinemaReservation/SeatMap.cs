namespace CinemaReservation;

public class SeatMap
{
    private readonly int _rows, _seats; // _rows: [0, 25], _cols: [1, 50]
    private readonly string _title;
    private int _seatsAvailable;
    private char[,] _seatMap;
    public string Title { get => _title; }
    public int Rows { get => _rows; }
    public int Seats { get => _seats; }
    public int SeatsAvailable { get => _seatsAvailable; }
    public SeatMap(string title, int rows, int seats)
    {
        if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(nameof(title));
        if (rows < 1 || rows > 26) throw new ArgumentOutOfRangeException(nameof(rows));
        if (seats < 1 || seats > 50) throw new ArgumentOutOfRangeException(nameof(seats));
        _title = title;
        _rows = rows;
        _seats = seats;
        _seatsAvailable = _rows * _seats;
        _seatMap = new char[_rows, _seats]; // All elements will be 0
    }
    public bool Reserve(int tickets, string seat)
    {
#if false
        if (tickets > _seatsAvailable)
        {
            WriteLine($"Not enough seats available!");
            return false;
        }
        if (string.IsNullOrEmpty(seat))
        {
            /* Default seats reservation.
             * Back row, middle seats.
             */
            for (int i = _rows - 1; i >= 0; i--)
            {
            }
        }
#endif
        throw new NotImplementedException();
    }
}
