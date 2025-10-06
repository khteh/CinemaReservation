using CinemaReservation.Strategies;

namespace CinemaReservation.Tests;

public class MiddleToRightStrategyTests
{
    private readonly ISeatAllocationStrategy _strategy = new MiddleToRightStrategy();
    [Fact]
    public void ReserveWholeRowShouldPassTests()
    {
        int index = 0;
        List<char> seats = new List<char>();
        List<int> allocations = new List<int>();
        for (int i = 0; i < 10; i++)
            seats.Add(' ');
        _strategy.Allocate(index, 10, seats, allocations);
        Assert.Equal(10, allocations.Count);
        Assert.Equal([0, 1, 2, 3, 4, 5, 6, 7, 8, 9], allocations);

        allocations.Clear();
        _strategy.Allocate(index, 1, seats, allocations);
        Assert.Empty(allocations);
    }
    [Fact]
    public void ReserveSectionsShouldPassTests()
    {
        int index = 0;
        List<char> seats = new List<char>();
        List<int> allocations = new List<int>();
        for (int i = 0; i < 10; i++)
            seats.Add(' ');
        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x		<= (10 - 4) / 2 = 3
        */
        _strategy.Allocate(index, 4, seats, allocations);
        Assert.Equal(4, allocations.Count);
        Assert.Equal([3, 4, 5, 6], allocations);
        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x x x
         */
        allocations.Clear();
        index = 7;
        seats = [' ', ' ', ' ', 'x', 'x', 'x', 'x', ' ', ' ', ' '];
        _strategy.Allocate(index, 2, seats, allocations);
        Assert.Equal(2, allocations.Count);
        Assert.Equal([7, 8], allocations);

        /*
        0 1 2 3 4 5 6 7 8 9
              x x x x x x x
         */
        allocations.Clear();
        index = 9;
        seats = [' ', ' ', ' ', 'x', 'x', 'x', 'x', 'x', 'x', ' '];
        _strategy.Allocate(index, 1, seats, allocations);
        Assert.Single(allocations);
        Assert.Equal([9], allocations);

        /*
        0 1 2 3 4 5 6 7 8 9
            x x x x x x x x
         */
        allocations.Clear();
        index = 2;
        seats = [' ', ' ', ' ', 'x', 'x', 'x', 'x', 'x', 'x', 'x'];
        _strategy.Allocate(index, 1, seats, allocations);
        Assert.Single(allocations);
        Assert.Equal([2], allocations);

        /*
        0 1 2 3 4 5 6 7 8 9
        x x x x x x x x x x
         */
        allocations.Clear();
        index = 1;
        seats = [' ', ' ', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x'];
        _strategy.Allocate(index, 2, seats, allocations);
        Assert.Equal(2, allocations.Count);
        Assert.Equal([0, 1], allocations);

        allocations.Clear();
        index = 1;
        seats = ['x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x'];
        _strategy.Allocate(index, 1, seats, allocations);
        Assert.Empty(allocations);
    }
    [Fact]
    public void ReserveBiggerThanARowShouldPassTests()
    {
        int index = 0;
        List<char> seats = new List<char>();
        List<int> allocations = new List<int>();
        for (int i = 0; i < 10; i++)
            seats.Add(' ');

        _strategy.Allocate(index, 11, seats, allocations);
        Assert.Equal(10, allocations.Count);
        Assert.Equal([0, 1, 2, 3, 4, 5, 6, 7, 8, 9], allocations);

        allocations.Clear();
        _strategy.Allocate(index, 1, seats, allocations);
        Assert.Empty(allocations);
    }
    [Fact]
    public void ReserveSpecificSeatShouldPassTests()
    {
        int index = 0;
        List<char> seats = new List<char>();
        List<int> allocations = new List<int>();
        for (int i = 0; i < 10; i++)
            seats.Add(' ');
        /*
        0 1 2 3 4 5 6 7 8 9
            x x x x
         */
        _strategy.Allocate(2, index, 4, seats, allocations);
        Assert.Equal(4, allocations.Count);
        Assert.Equal([2, 3, 4, 5], allocations);

        /*
        0 1 2 3 4 5 6 7 8 9
            x x x x x x x
         */
        allocations.Clear();
        _strategy.Allocate(4, index, 3, seats, allocations);
        Assert.Equal(3, allocations.Count);
        Assert.Equal([6, 7, 8], allocations);

        /*
        0 1 2 3 4 5 6 7 8 9
          x x x x x x x x
         */
        allocations.Clear();
        _strategy.Allocate(1, index, 1, seats, allocations);
        Assert.Single(allocations);
        Assert.Equal([1], allocations);

        /*
        0 1 2 3 4 5 6 7 8 9
        x x x x x x x x x x
         */
        allocations.Clear();
        _strategy.Allocate(0, index, 3, seats, allocations);
        Assert.Equal(2, allocations.Count);
        Assert.Equal([0, 9], allocations);
    }
}
