using System.Text.RegularExpressions;
namespace CinemaReservation.Tests;

public class SeatRegexTests : IClassFixture<TestFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly Regex _regex = new Regex(@"^([a-zA-Z]{1})([0-9]{2})$");
    private readonly IServiceProvider _serviceProvider;
    public SeatRegexTests(ITestOutputHelper output, TestFixture testFixture)
    {
        _output = output;
        _serviceProvider = testFixture.Host.Services;
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
}
