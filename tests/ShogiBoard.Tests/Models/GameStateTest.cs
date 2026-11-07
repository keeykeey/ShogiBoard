using ShogiBoard.Models;

namespace ShogiBoard.Tests.Models;

public class PieceOnStandTest
{
    [Theory]
    [InlineData("p", 1, 2, 1)]
    [InlineData("P", int.MaxValue, int.MaxValue, int.MaxValue - 1)]
    [InlineData("R", 0, 1, 0)]
    public void PieceOnStand_IncreaseDecrease_BoundaryTest(string str, int count, int expect_count_1, int expect_count_2)
    {
        PieceOnStand piece = new(str, count);
        Assert.Equal(str, piece.Piece);
        Assert.Equal(count, piece.Count);
        piece.Increment();
        Assert.Equal(expect_count_1, piece.Count);
        piece.Decrement();
        Assert.Equal(expect_count_2, piece.Count);
    }

    [Theory]
    [InlineData("R", 0, 0, 1)]
    public void PieceOnStand_DecreaseIncrease_BoundaryTest(string str, int count, int expect_count_1, int expect_count_2)
    {
        PieceOnStand piece = new(str, count);
        Assert.Equal(str, piece.Piece);
        Assert.Equal(count, piece.Count);
        piece.Decrement();
        Assert.Equal(expect_count_1, piece.Count);
        piece.Increment();
        Assert.Equal(expect_count_2, piece.Count);
    }
}