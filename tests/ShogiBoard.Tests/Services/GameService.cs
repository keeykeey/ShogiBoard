
using ShogiBoard.Services;
using ShogiBoard.Models;
namespace ShogiBoard.Tests.Services;

public class GameServiceTest
{
    [Theory]
    [InlineData("l1n6/+s1g6/b1r6/9/9/9/9/9/9")]
    [InlineData("9/9/9/9/9/9/5l1nK/5+s2g/5b1+Pr")]
    [InlineData("lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL")]
    public void SetBoardDataTest_Positive(string text)
    {
        GameState gs = new();
        GameService service = new();
        service.SetBoardData(gs, text);

        string[] data = text.Split("/");
        for (int rank = 1; rank <= gs.Board.NRank; ++rank)
        {
            string rankText = data[rank - 1];
            int files = 9;
            for (int j = 0; j < rankText.Length; ++j)
            {
                string s = rankText.Substring(j, 1);
                if (s == "+")
                {
                    s = rankText.Substring(j, 2);
                    Assert.Equal(s, gs.Board.Get(file: files, rank: rank));
                    j += 1;
                }
                else
                {
                    if (int.TryParse(s, out var n))
                    {
                        for (int k = 0; k < n; ++k)
                        {
                            Assert.Equal(string.Empty, gs.Board.Get(file: files - k, rank: rank));
                        }
                        files -= n;
                        continue;
                    }
                    else
                    {
                        Assert.Equal(s, gs.Board.Get(file: files, rank: rank));
                    }
                }
                files -= 1;
            }
        }
    }

    [Theory]
    [InlineData(1, 9, 1, 1, true, "+L",
        "9/9/9/9/9/9/9/9/8L", true)]
    [InlineData(9, 1, 9, 9, true, "+l",
        "l8/9/9/9/9/9/9/9/9", false)]
    [InlineData(9, 9, 9, 1, true, "+L",
        "9/9/9/9/9/9/9/9/L8", true)]
    [InlineData(1, 1, 1, 9, true, "+l",
        "8l/9/9/9/9/9/9/9/9", false)]
    [InlineData(1, 3, 2, 4, false, "S",
        "9/9/8S/9/9/9/9/9/9", true)]
    [InlineData(9, 9, 1, 1, false, "+b",
        "9/9/9/9/9/9/9/9/+b8", false)]
    public void BoardDataNext_MovePieceTest_PositiveBoundary(
        int ffile, int frank,
        int tofile,int torank,  bool promote, string expectPieceOnTo,
        string sfen, bool blackCanMovePiece)
    {
        GameService service = new();
        Tile from = new (ffile, frank);
        Tile to = new(tofile, torank);

        GameState gs = new(
            inAGame: true,
            playerIsBlack: true,
            blackCanMovePiece:blackCanMovePiece
        );

        service.SetBoardData(gs, sfen);
        service.BoardDataNext(from, to, promote, gs);
        Assert.Equal("", gs.Board.Get(file: ffile, rank: frank));
        Assert.Equal(expectPieceOnTo, gs.Board.Get(file: tofile, rank: torank));
    }

    [Fact]
    public void BoardDataNext_BlackTakePieceTest_Positive()
    {
        List<(string sfen, int fromFile, int fromRank, int toFile, int toRank, string expect)> moves = [
            ("9/9/9/9/ppppppppp/PPPPPPPPP/9/9/9", 1, 6, 1, 5, "P"),
            ("9/9/9/9/pppppppp+p/PPPPPPPPP/9/9/9", 1, 6, 1, 5, "P"),
            ("9/9/9/9/ppppppppp/PPPPPPPPP/9/9/9", 1, 6, 1, 5, "P"),
        ];

        GameService service = new();
        GameState gs = new(
            inAGame: true,
            playerIsBlack: true,
            blackCanMovePiece: true
        );

        foreach(var (sfen, fromFile, fromRank, toFile, toRank, expect) in moves)
        {
            Tile from = new (fromFile, fromRank);
            Tile to = new(toFile, toRank);
            service.SetBoardData(gs, sfen);
            service.BoardDataNext(from, to, false, gs);
            Assert.Equal("", gs.Board.Get(rank: fromRank, file: fromFile));
            Assert.Equal(expect, gs.Board.Get(rank: toRank, file: toFile));
        }

        PieceOnStand? pos = gs.BlackPieceStand.Data.FirstOrDefault(p => p.Piece == "P");
        Assert.NotNull(pos);
        Assert.Equal(3, pos.Count);
    }

    [Fact]
    public void BoardDataNext_WhiteTakePieceTest_Positive()
    {
        List<(string sfen, int fromFile, int fromRank, int toFile, int toRank, string expect)> moves = [
            ("9/p8/N8/9/9/9/9/9/9", 9, 2, 9, 3, "p"),
            ("9/p8/+N8/9/9/9/9/9/9", 9, 2, 9, 3, "p"),
            ("9/p8/N8/9/9/9/9/9/9", 9, 2, 9, 3, "p"),
        ];

        GameService service = new();
        GameState gs = new(
            inAGame: true,
            playerIsBlack: true,
            blackCanMovePiece: false
        );

        foreach (var (sfen, fromFile, fromRank, toFile, toRank, expect) in moves)
        {
            Tile from = new(fromFile, fromRank);
            Tile to = new(toFile, toRank);
            service.SetBoardData(gs, sfen);
            service.BoardDataNext(from, to, false, gs);
            Assert.Equal("", gs.Board.Get(rank: fromRank, file: fromFile));
            Assert.Equal(expect, gs.Board.Get(rank: toRank, file: toFile));
        }

        PieceOnStand? pos = gs.WhitePieceStand.Data.FirstOrDefault(p => p.Piece == "n");
        Assert.NotNull(pos);
        Assert.Equal(3, pos.Count);
    }

    [Fact]
    public void BoardDataNext_BlackPutPieceFromStand_Positive()
    {
        GameService service = new();
        GameState gs = new(
            inAGame: true,
            playerIsBlack: true,
            blackCanMovePiece: true
        );
        gs.BlackPieceStand.Add("R");
        gs.BlackPieceStand.Add("R");
        gs.BlackPieceStand.Add("G");
        gs.BlackPieceStand.Add("G");
        gs.BlackPieceStand.Add("G");
        gs.BlackPieceStand.Add("G");
        service.SetBoardData(gs, "9/9/9/9/9/9/9/9/9");
        service.BoardDataNext(new PieceStandPosition("R*"), new Tile(1, 1), false, gs); // top - right
        service.BoardDataNext(new PieceStandPosition("R*"), new Tile(2, 2), false, gs);
        service.BoardDataNext(new PieceStandPosition("G*"), new Tile(3, 3), false, gs);
        service.BoardDataNext(new PieceStandPosition("G*"), new Tile(4, 4), false, gs);
        service.BoardDataNext(new PieceStandPosition("G*"), new Tile(9, 9), false, gs); // bottom -left

        Assert.Equal("R", gs.Board.Get(1, 1));
        Assert.Equal("R", gs.Board.Get(2, 2));
        Assert.Equal("G", gs.Board.Get(3, 3));
        Assert.Equal("G", gs.Board.Get(4, 4));
        Assert.Equal("G", gs.Board.Get(9, 9));

        PieceOnStand? rook = gs.BlackPieceStand.Data.FirstOrDefault(p => p.Piece.Equals("R"));
        Assert.Null(rook);

        PieceOnStand? gold = gs.BlackPieceStand.Data.FirstOrDefault(p => p.Piece.Equals("G"));
        Assert.Equal(1, gold?.Count);
    }

    [Fact]
    public void BoardDataNext_WhitePutPieceFromStand_Positive()
    {
        GameService service = new();
        GameState gs = new(
            inAGame: true,
            playerIsBlack: true,
            blackCanMovePiece: false
        );
        gs.WhitePieceStand.Add("b");
        gs.WhitePieceStand.Add("b");
        gs.WhitePieceStand.Add("s");
        gs.WhitePieceStand.Add("s");
        gs.WhitePieceStand.Add("s");
        gs.WhitePieceStand.Add("s");
        service.SetBoardData(gs, "9/9/9/9/9/9/9/9/9");
        service.BoardDataNext(new PieceStandPosition("b*"), new Tile(1, 1), false, gs); // top - right
        service.BoardDataNext(new PieceStandPosition("b*"), new Tile(2, 2), false, gs);
        service.BoardDataNext(new PieceStandPosition("s*"), new Tile(3, 3), false, gs);
        service.BoardDataNext(new PieceStandPosition("s*"), new Tile(4, 4), false, gs);
        service.BoardDataNext(new PieceStandPosition("s*"), new Tile(9, 9), false, gs); // bottom -left

        Assert.Equal("b", gs.Board.Get(1, 1));
        Assert.Equal("b", gs.Board.Get(2, 2));
        Assert.Equal("s", gs.Board.Get(3, 3));
        Assert.Equal("s", gs.Board.Get(4, 4));
        Assert.Equal("s", gs.Board.Get(9, 9));

        PieceOnStand? bishop = gs.WhitePieceStand.Data.FirstOrDefault(p => p.Piece.Equals("b"));
        Assert.Null(bishop);

        PieceOnStand? silver = gs.WhitePieceStand.Data.FirstOrDefault(p => p.Piece.Equals("s"));
        Assert.Equal(1, silver?.Count);
    }

    [Fact]
    public void BoardDataNext_TakePieceScenarioTest()
    {
        List<(string sfen, int fromFile, int fromRank, int toFile, int toRank, string expect)> moves = [
            ("8l/9/p8/P7p/9/7SP/9/9/8L", 1, 6, 1, 5, "P"),
            ("8l/9/p8/P7p/8P/7S1/9/9/8L", 1, 4, 1, 5, "p"),
            ("8l/9/p8/P8/8p/7S1/9/9/8L", 2, 6, 1, 5, "S"),
            ("8l/9/p8/P7p/8S/9/9/9/8L", 1, 1, 1, 5, "l"),
            ("9/9/p8/P7p/8l/9/9/9/8L", 1, 9, 1, 5, "L"),
            ("9/9/p8/P7p/8S/9/9/9/8L", 9, 3, 9, 4, "p"),
        ];

        GameService service = new();
        GameState gs = new(
            inAGame: true,
            playerIsBlack: true,
            blackCanMovePiece: true
        );

        foreach(var (sfen, fromFile, fromRank, toFile, toRank, expect) in moves)
        {
            Tile from = new (fromFile, fromRank);
            Tile to = new(toFile, toRank);
            service.SetBoardData(gs, sfen);
            service.BoardDataNext(from, to, false, gs);
            Assert.Equal("", gs.Board.Get(file: fromFile, rank: fromRank));
            Assert.Equal(expect, gs.Board.Get(file: toFile, rank: toRank));
            gs.BlackCanMovePiece = !gs.BlackCanMovePiece;
        }

        PieceOnStand? pornBlackTook = gs.BlackPieceStand.Data.FirstOrDefault(p => p.Piece == "P");
        PieceOnStand? pornWhiteTook = gs.WhitePieceStand.Data.FirstOrDefault(p => p.Piece == "p");
        PieceOnStand? lanceBlackTook = gs.BlackPieceStand.Data.FirstOrDefault(p => p.Piece == "L");
        PieceOnStand? lanceWhiteTook = gs.WhitePieceStand.Data.FirstOrDefault(p => p.Piece == "l");
        PieceOnStand? silverBlackTook = gs.BlackPieceStand.Data.FirstOrDefault(p => p.Piece == "S");
        PieceOnStand? silverWhiteTook = gs.WhitePieceStand.Data.FirstOrDefault(p => p.Piece == "s");

        Assert.Equal(1, pornBlackTook?.Count);
        Assert.Equal(2, pornWhiteTook?.Count);
        Assert.Equal(1, lanceBlackTook?.Count);
        Assert.Null(lanceWhiteTook);
        Assert.Equal(1, silverWhiteTook?.Count);
        Assert.Null(silverBlackTook);
    }
}
