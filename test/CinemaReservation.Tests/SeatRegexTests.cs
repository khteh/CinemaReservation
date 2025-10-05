using CinemaReservation.Strategies;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;
namespace CinemaReservation.Tests;

public class SeatRegexTests : IClassFixture<TestFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly ISeatAllocationStrategy _strategy;
    private readonly ILoggerFactory _logger;
    private readonly Regex _regex = new Regex(@"^([a-zA-Z]{1})([0-9]{2})$");
    private readonly SeatMap _seatMap;
    private FieldInfo _field = typeof(SeatMap).GetField("_seatsPerRow", BindingFlags.Instance | BindingFlags.NonPublic);
    private FieldInfo _rowsField = typeof(SeatMap).GetField("_rows", BindingFlags.Instance | BindingFlags.NonPublic);
    public SeatRegexTests(ITestOutputHelper output, TestFixture testFixture)
    {
        _output = output;
        //_logger = new Mock<ILogger<SeatMap>>().Object;
        _logger = testFixture.Host.Services.GetService<ILoggerFactory>();
        _strategy = testFixture.Strategy;
        _seatMap = new SeatMap(_logger, _strategy, "Test Movie", 10, 10);
    }
    [Theory]
    [InlineData(1, "A05", 0, 5)]
    [InlineData(0, "AB05", -1, -1)]
    [InlineData(0, "A123", -1, -1)]
    [InlineData(0, "AB123", -1, -1)]
    public void SeatStringValidationTests(int expected, string seat, int expectedRow, int expectedCol)
    {
        int row = -1, col = -1; // _rows: [0, 25], _cols: [1, min(_seatMap._seats, 50)]
        seat = seat.Trim().ToLower();
        MatchCollection matches = _regex.Matches(seat);
        _output.WriteLine($"{matches.Count} matches");
        Assert.Equal(expected, matches.Count);
        if (matches.Count > 0)
        {
            /* Report on each match.
             * If a match is found, information about this part of the matching string can be retrieved from the second Group object in the GroupCollection object returned by the Match.Groups property. 
             * The first element in the collection represents the entire match.
             */
            Assert.Equal(3, matches[0].Groups.Count);
            Assert.Equal(1, matches[0].Groups[1].Value.Length);
            row = matches[0].Groups[1].Value.ToLower()[0] - 'a';
            Assert.True(Int32.TryParse(matches[0].Groups[2].Value, out col));
        }
        Assert.Equal(expectedRow, row);
        Assert.Equal(expectedCol, col);
    }
    [Theory]
    [InlineData("A05", 0, 5)]
    [InlineData("J10", 9, 10)]
    [InlineData("K05", -1, -1)] // row should be <= J (10)
    [InlineData("E00", -1, -1)] // col should be >= 1
    [InlineData("E05", 4, 5)]
    [InlineData("E11", -1, -1)] // col should be <= 10
    public void RowSeatValidationTests(string seat, int expectedRow, int expectedSeat)
    {
        int row = -1, col = -1; // _rows: [0, 25], _cols: [1, min(_seatMap._seats, 50)]
        MatchCollection matches = _regex.Matches(seat);
        _output.WriteLine($"{matches.Count} matches");
        Assert.Single(matches);
        if (matches.Count > 0)
        {
            /* Report on each match.
             * If a match is found, information about this part of the matching string can be retrieved from the second Group object in the GroupCollection object returned by the Match.Groups property. 
             * The first element in the collection represents the entire match.
             */
            Assert.Equal(3, matches[0].Groups.Count);
            Assert.Equal(1, matches[0].Groups[1].Value.Length);
            int _row = matches[0].Groups[1].Value.ToLower()[0] - 'a';
            Assert.True(Int32.TryParse(matches[0].Groups[2].Value, out int _col));
            int seatsPerRow = (int)_field.GetValue(_seatMap);
            List<SeatRow> rows = (List<SeatRow>)_rowsField.GetValue(_seatMap);
            _output.WriteLine($"{nameof(RowSeatValidationTests)} row: {_row}/{rows.Count}, col: {_col}/{int.Min(seatsPerRow, 50)}");
            if (_row >= 0 && _row < int.Min(rows.Count, 26) && _col >= 1 && _col <= int.Min(seatsPerRow, 50))
            {
                row = _row;
                col = _col;
            }
        }
        Assert.Equal(expectedRow, row);
        Assert.Equal(expectedSeat, col);
    }
}
