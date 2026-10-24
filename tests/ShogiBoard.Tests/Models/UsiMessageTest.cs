using ShogiBoard.Models;

namespace ShogiBoard.Tests.Models;

public class SfenParserTest
{

    [Theory]
    [InlineData(
        "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL", "b", "-", "", "", 1,
        "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b - 1"
    )]
    [InlineData(
        "lnsgkgsnl/1r5b1/pppp1pppp/9/9/9/PP1P1PPPP/1B5R1/LNSGKGSNL", "w", "P3p2", "P3", "p2", 2,
        "lnsgkgsnl/1r5b1/pppp1pppp/9/9/9/PP1P1PPPP/1B5R1/LNSGKGSNL w P3p2 2"
    )]
    [InlineData(
        "4k4/9/9/9/9/9/9/9/8K", "b", "P18L4K4S4G4B2R2", "P18L4K4S4G4B2R2", "", 100,
        "4k4/9/9/9/9/9/9/9/8K b P18L4K4S4G4B2R2 100"
    )]
    public void ConstructSfen_PositiveTest(
        string board,
        string bw,
        string sidePiece,
        string blackSidePiece,
        string whiteSidePiece,
        int moveCount,
        string input)
    {
        Sfen sfen = new(input);
        Assert.Equal(board, sfen.Board);
        Assert.Equal(bw, sfen.BlackWhite);
        Assert.Equal(sidePiece, sfen.SidePiece);
        Assert.Equal(blackSidePiece, sfen.BlackSidePiece);
        Assert.Equal(whiteSidePiece, sfen.WhiteSidePiece);
        Assert.Equal(moveCount, sfen.MoveCount);
    }

    [Theory]
    [InlineData(
        "lnsgkgsnl/1r5b1/pppp1pppp/9/9/9/PP1P1PPPP/1B5R1/LNSGKGSNL",
        "lnsgkgsnl/1r5b1/pppp1pppp/9/9/9/PP1P1PPPP/1B5R1/LNSGKGSNL w P3p2 2"
    )]
    [InlineData(
        "9/9/9/9/9/9/9/9/9",
        "9/9/9/9/9/9/9/9/9 w P3p2 2"
    )]
    [InlineData(
        "ll7/9/9/9/9/9/9/9/9",
        "ll7/9/9/9/9/9/9/9/9 w P3p2 2"
    )]
    [InlineData(
        "3+Pp+l3/9/3+Sr+k3/9/9/9/9/9/9",
        "3+Pp+l3/9/3+Sr+k3/9/9/9/9/9/9 w P3p2 2"
    )]
    public void TakeBoard_PositiveTest(string expect, string sfen)
    {
        string? result = SfenParser.GetBoard(sfen);
        Assert.Equal(expect, result);
    }

    [Theory]
    [InlineData("w", "lnsgkgsnl/1r5b1/pppp1pppp/9/9/9/PP1P1PPPP/1B5R1/LNSGKGSNL w P3p2 2")]
    [InlineData("b", "lnsgkgsnl/1r5b1/pppp1pppp/9/9/9/PP1P1PPPP/1B5R1/LNSGKGSNL b P3p2 2")]
    public void TakeBlackWhite_PositiveTest(string expect, string sfen)
    {
        string? result = SfenParser.GetBlackWhite(sfen);
        Assert.Equal(expect, result);
    }

    [Theory]
    [InlineData("P3p2", "lnsgkgsnl/1r5b1/pppp1pppp/9/9/9/PP1P1PPPP/1B5R1/LNSGKGSNL w P3p2 2")]
    [InlineData("Gs2", "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL w Gs2 2")]
    [InlineData("L2k", "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL w L2k 2")]
    [InlineData("bbR", "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL w bbR 2")]
    [InlineData("-", "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL w - 2")]
    public void TakeSidePiece_PositiveTest(string expect, string sfen)
    {
        string? result = SfenParser.GetSidePiece(sfen);
        Assert.Equal(expect, result);
    }

    [Theory]
    [InlineData("0", "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL w P3p2 0")]
    [InlineData("1", "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL w P3p2 1")]
    [InlineData("100", "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL w P3p2 100")]
    [InlineData("9999", "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL w P3p2 9999")]
    public void TakeMoveCount_PositiveTest(string expect, string sfen)
    {
        string? result = SfenParser.GetMoveCount(sfen);
        Assert.Equal(expect, result);
    }
}
