namespace CinemaReservation;

public class Reservation : IDisposable
{
    private readonly string _id;
    private Dictionary<int, List<int>> _seats;
    public string Id { get { return _id; } }
    public Dictionary<int, List<int>> Seats { get { return _seats; } }
    public bool Confirmed { get; set; } = false;
    public Reservation(string id, Dictionary<int, List<int>> seats)
    {
        _id = id;
        _seats = seats;
    }
    public void Dispose()
    {
        _seats.Clear();
        GC.SuppressFinalize(this);
    }
}
