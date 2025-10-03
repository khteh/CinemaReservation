namespace CinemaReservation;

public class SeatMap
{
    private readonly int _rows, _seats; // _rows: [0, 25], _cols: [1, 50]
    private readonly string _title;
    private int _seatsAvailable;
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
    }
    public bool Reserve(int tickets, string seat)
    {
        throw new NotImplementedException();
    }
}
