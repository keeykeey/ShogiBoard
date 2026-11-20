using ShogiBoard.Models;

namespace ShogiBoard.Tests.Models;

public class GameRecordTest
{
    [Fact]
    public void GameRecord_ConstructTest()
    {
        Move m = new("5a5b", false);
        GameRecord gr = new(
            move: m, score: 100,
            byoyomiBlack: 1, timeControlMinBlack: 2, timeControlSecBlack: 3,
            byoyomiWhite: 4, timeControlMinWhite: 5, timeControlSecWhite: 6);
        Assert.Equal(m, gr.Move);
        Assert.Equal(100, gr.Score);
        Assert.Equal(1, gr.ByoyomiBlack);
        Assert.Equal(2, gr.TimeControlMinBlack);
        Assert.Equal(3, gr.TimeControlSecBlack);
        Assert.Equal(4, gr.ByoyomiWhite);
        Assert.Equal(5, gr.TimeControlMinWhite);
        Assert.Equal(6, gr.TimeControlSecWhite);

        GameRecord gr2 = new(m);
        Assert.Equal(m, gr2.Move);
        Assert.Null(gr2.Score);
        Assert.Null(gr2.ByoyomiBlack);
        Assert.Null(gr2.TimeControlMinBlack);
        Assert.Null(gr2.TimeControlSecBlack);
        Assert.Null(gr2.ByoyomiWhite);
        Assert.Null(gr2.TimeControlMinWhite);
        Assert.Null(gr2.TimeControlSecWhite);
    }

    [Fact]
    public void GameRecord_GetterSetterTest()
    {
        Move m = new("5i5h", true);
        Move m2 = new("5a5b", false);
        Move m3 = new("4a4b", false);
        GameRecord gr = new(m);
        GameRecord gr2 = new(m2);
        GameRecord gr3 = new(m3);

        gr.SetNext(gr2);
        gr.SetBranchNext(gr3);

        Assert.Equal(gr2, gr.GetNext());
        Assert.Equal(gr, gr2.GetPrev());
        Assert.Equal(gr3, gr.GetBranchNext(branchIndex: 0));
    }

    [Fact]
    public void GameRecord_GetBranchNext_NegativeTest()
    {
        Move m = new("5i5h", true);
        Move m2 = new("5a5b", false);
        GameRecord gr = new(m);
        GameRecord gr2 = new(m2);
        Assert.Throws<ArgumentException>(() => gr.GetBranchNext(-1));
        Assert.Throws<ArgumentException>(() => gr.GetBranchNext(1));
        gr.SetBranchNext(gr2);
        Assert.Throws<ArgumentException>(() => gr.GetBranchNext(2));
    }

    [Fact]
    public void GameRecord_Moves_Test()
    {
        Move m = new("5i5h", true);
        Move m2 = new("5a5b", false);
        Move m3 = new("P*5e", true);
        Move m4 = new("2d2c+", false);
        GameRecord gr = new(m);
        GameRecord gr2 = new(m2);
        GameRecord gr3 = new(m3);
        GameRecord gr4 = new(m4);
        gr.SetNext(gr2);
        gr2.SetNext(gr3);
        gr3.SetNext(gr4);

        Assert.Equal("5i5h 5a5b P*5e 2d2c+", string.Join(" ", gr.Moves()));
    }

    [Fact]
    public void GameRecord_TimeControl_Test()
    {
        Move m = new("5i5h", true);
        Move m2 = new("5a5b", false);
        GameRecord gr = new(
            move: m,
            byoyomiBlack: 30, timeControlMinBlack: 10, timeControlSecBlack: 0,
            byoyomiWhite: 10, timeControlMinWhite: 8, timeControlSecWhite: 1);
        GameRecord gr2 = new(
            move: m2,
            byoyomiBlack: 15, timeControlMinBlack: 5, timeControlSecBlack: 3,
            byoyomiWhite: 9, timeControlMinWhite: 4, timeControlSecWhite: 2);

        Assert.Equal(30, gr.ByoyomiBlack);
        Assert.Equal(10, gr.TimeControlMinBlack);
        Assert.Equal(0, gr.TimeControlSecBlack);
        Assert.Equal(10, gr.ByoyomiWhite);
        Assert.Equal(8, gr.TimeControlMinWhite);
        Assert.Equal(1, gr.TimeControlSecWhite);

        Assert.Equal(15, gr2.ByoyomiBlack);
        Assert.Equal(5, gr2.TimeControlMinBlack);
        Assert.Equal(3, gr2.TimeControlSecBlack);
        Assert.Equal(9, gr2.ByoyomiWhite);
        Assert.Equal(4, gr2.TimeControlMinWhite);
        Assert.Equal(2, gr2.TimeControlSecWhite);
    }
}