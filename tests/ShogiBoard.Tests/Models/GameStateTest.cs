using ShogiBoard.Models;

namespace ShogiBoard.Tests.Models;

public class PieceOnStandTest
{

    [Theory]
    [InlineData(1, "p")]
    [InlineData(2, "l")]
    [InlineData(3, "n")]
    [InlineData(4, "s")]
    [InlineData(5, "g")]
    [InlineData(6, "k")]
    [InlineData(7, "b")]
    [InlineData(8, "r")]
    [InlineData(9, "+p")]
    public void BoardRank_GetSet_PositiveTest(int file, string piece)
    {
        BoardRank br = new();
        Assert.Equal("", br.Get(file));
        br.Set(file, piece);
        Assert.Equal(piece, br.Get(file));
    }

    [Theory]
    [InlineData(1, 1, "L")]
    [InlineData(2, 2, "N")]
    [InlineData(3, 3, "S")]
    [InlineData(4, 4, "G")]
    [InlineData(5, 5, "K")]
    [InlineData(6, 6, "+l")]
    [InlineData(7, 7, "+N")]
    [InlineData(8, 8, "+s")]
    [InlineData(9, 9, "+R")]
    public void Board_Set_PositiveTest(int rank, int file, string piece)
    {
        Board b = new();
        b.Set(rank: rank, file: file, piece: piece);
        Assert.Equal(piece, b.Get(rank: rank, file: file));
    }

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