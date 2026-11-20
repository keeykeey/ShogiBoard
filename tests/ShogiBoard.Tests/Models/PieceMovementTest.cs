using ShogiBoard.Models;

namespace ShogiBoard.Tests.Models;

public class PornMovementTest
{
    [Fact]
    public void Test_BlackMovementFromTileIsAppropriate()
    {
        PornMovement m = new();
        Tile from = new(file: 9, rank: 9);
        List<Tile> results = [];
        Tile expect = new(file: 9, rank: 8);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Single(results);
        Assert.Equal(expect, results[0]);
    }

    [Fact]
    public void Test_WhiteMovementFromTileIsAppropriate()
    {
        PornMovement m = new(isBlack: false);
        Tile from = new(file: 1, rank: 1);
        List<Tile> results = [];
        Tile expect = new(file: 1, rank: 2);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Single(results);
        Assert.Equal(expect, results[0]);
    }

    [Fact]
    public void Test_BlackMovementFromTileIsAppropriateAtTheCorner()
    {
        PornMovement m = new();
        Tile from = new(file: 1, rank: 1);
        List<Tile> results = [];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Empty(results);
    }

    [Fact]
    public void Test_WhiteMovementFromTileIsAppropriateAtTheCorner()
    {
        PornMovement m = new(isBlack: false);
        Tile from = new(file: 9, rank: 9);
        List<Tile> results = [];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Empty(results);
    }

    [Fact]
    public void Test_BlackMovementFromPieceStandPositionIsAppropriate()
    {
        PornMovement m = new();
        PieceStandPosition from = new("P*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 2);
        Tile twoOfExpects = new(file: 9, rank: 9);
        Tile oneOfNoExpects = new(file: 1, rank: 1);
        Tile twoOfNoExpects = new(file: 9, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(72, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
        Assert.DoesNotContain(oneOfNoExpects, results);
        Assert.DoesNotContain(twoOfNoExpects, results);
    }

    [Fact]
    public void Test_WhiteMovementFromPieceStandPositionIsAppropriate()
    {
        PornMovement m = new();
        PieceStandPosition from = new("p*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 8);
        Tile oneOfNoExpects = new(file: 1, rank: 9);
        Tile twoOfNoExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(72, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
        Assert.DoesNotContain(oneOfNoExpects, results);
        Assert.DoesNotContain(twoOfNoExpects, results);
    }
}

public class LanceMovementTest
{
    [Fact]
    public void Test_BlackMovementFromTileIsAppropriate()
    {
        LanceMovement m = new();
        Tile from = new(file: 9, rank: 9);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 9, rank: 8), new(file: 9, rank: 7), new(file: 9, rank: 6),
            new(file: 9, rank: 5), new(file: 9, rank: 4), new(file: 9, rank: 3),
            new(file: 9, rank: 2), new(file: 9, rank: 1)
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_WhiteMovementFromTileIsAppropriate()
    {
        LanceMovement m = new(isBlack: false);
        Tile from = new(file: 1, rank: 1);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 1, rank: 9), new(file: 1, rank: 8), new(file: 1, rank: 7),
            new(file: 1, rank: 6), new(file: 1, rank: 5), new(file: 1, rank: 4),
            new(file: 1, rank: 3), new(file: 1, rank: 2)
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_BlackMovementFromTileIsAppropriateAtTheCorner()
    {
        LanceMovement m = new();
        Tile from = new(file: 1, rank: 1);
        List<Tile> results = [];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Empty(results);
    }

    [Fact]
    public void Test_WhiteMovementFromTileIsAppropriateAtTheCorner()
    {
        LanceMovement m = new(isBlack: false);
        Tile from = new(file: 9, rank: 9);
        List<Tile> results = [];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Empty(results);
    }

    [Fact]
    public void Test_BlackMovementFromPieceStandPositionIsAppropriate()
    {
        LanceMovement m = new();
        PieceStandPosition from = new("L*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 2);
        Tile twoOfExpects = new(file: 9, rank: 9);
        Tile oneOfNoExpects = new(file: 1, rank: 1);
        Tile twoOfNoExpects = new(file: 9, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(72, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
        Assert.DoesNotContain(oneOfNoExpects, results);
        Assert.DoesNotContain(twoOfNoExpects, results);
    }

    [Fact]
    public void Test_WhiteMovementFromPieceStandPositionIsAppropriate()
    {
        LanceMovement m = new();
        PieceStandPosition from = new("l*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 8);
        Tile oneOfNoExpects = new(file: 1, rank: 9);
        Tile twoOfNoExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(72, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
        Assert.DoesNotContain(oneOfNoExpects, results);
        Assert.DoesNotContain(twoOfNoExpects, results);
    }
}

public class KnightMovementTest
{
    [Fact]
    public void Test_BlackMovementFromTileIsAppropriate()
    {
        KnightMovement m = new();
        Tile from = new(file: 5, rank: 9);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 7), new(file: 4, rank: 7)
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_WhiteMovementFromTileIsAppropriate()
    {
        KnightMovement m = new(isBlack: false);
        Tile from = new(file: 5, rank: 1);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 3), new(file: 4, rank: 3)
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_BlackMovementFromTileIsAppropriateAtTheCorner()
    {
        KnightMovement m = new();
        List<Tile> results = [];
        List<Tile> expects;

        Tile from = new(file: 5, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Empty(results);

        results = [];
        expects = [
            new(file: 2, rank: 7)
        ];
        from = new(file: 1, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 8, rank: 7)
        ];
        from = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_WhiteMovementFromTileIsAppropriateAtTheCorner()
    {
        KnightMovement m = new(isBlack: false);
        List<Tile> results = [];
        List<Tile> expects;

        Tile from = new(file: 5, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Empty(results);

        results = [];
        expects = [
            new(file: 2, rank: 3)
        ];
        from = new(file: 1, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 8, rank: 3)
        ];
        from = new(file: 9, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_BlackMovementFromPieceStandPositionIsAppropriate()
    {
        KnightMovement m = new();
        PieceStandPosition from = new("N*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 3);
        Tile twoOfExpects = new(file: 9, rank: 9);
        Tile oneOfNoExpects = new(file: 1, rank: 1);
        Tile twoOfNoExpects = new(file: 9, rank: 1);
        Tile threeOfNoExpects = new(file: 1, rank: 2);
        Tile fourOfNoExpects = new(file: 9, rank: 2);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(63, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
        Assert.DoesNotContain(oneOfNoExpects, results);
        Assert.DoesNotContain(twoOfNoExpects, results);
        Assert.DoesNotContain(threeOfNoExpects, results);
        Assert.DoesNotContain(fourOfNoExpects, results);
    }

    [Fact]
    public void Test_WhiteMovementFromPieceStandPositionIsAppropriate()
    {
        KnightMovement m = new();
        PieceStandPosition from = new("n*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 7);
        Tile oneOfNoExpects = new(file: 1, rank: 8);
        Tile twoOfNoExpects = new(file: 9, rank: 8);
        Tile threeOfNoExpects = new(file: 1, rank: 9);
        Tile fourOfNoExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(63, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
        Assert.DoesNotContain(oneOfNoExpects, results);
        Assert.DoesNotContain(twoOfNoExpects, results);
        Assert.DoesNotContain(threeOfNoExpects, results);
        Assert.DoesNotContain(fourOfNoExpects, results);
    }
}

public class SilverMovementTest
{
    [Fact]
    public void Test_BlackMovementFromTileIsAppropriate()
    {
        SilverMovement m = new();
        Tile from = new(file: 5, rank: 5);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 4), new(file: 5, rank: 4), new(file: 4, rank: 4),
            new(file: 6, rank: 6), new(file: 4, rank: 6)
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_WhiteMovementFromTileIsAppropriate()
    {
        SilverMovement m = new(isBlack: false);
        Tile from = new(file: 5, rank: 5);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 6), new(file: 5, rank: 6), new(file: 4, rank: 6),
            new(file: 6, rank: 4), new(file: 4, rank: 4)
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_BlackMovementFromTileIsAppropriateAtTheCorner()
    {
        SilverMovement m = new();

        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 2), new(file: 4, rank: 2),
        ];
        Tile from = new(file: 5, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 6, rank: 8), new(file: 5, rank: 8), new(file: 4, rank: 8),
        ];
        from = new(file: 5, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 2, rank: 4), new(file: 1, rank: 4), new(file: 2, rank: 6)
        ];
        from = new(file: 1, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 8, rank: 4), new(file: 9, rank: 4), new(file: 8, rank: 6)
        ];
        from = new(file: 9, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_WhiteMovementFromTileIsAppropriateAtTheCorner()
    {
        SilverMovement m = new(isBlack: false);

        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 8), new(file: 4, rank: 8),
        ];
        Tile from = new(file: 5, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 6, rank: 2), new(file: 5, rank: 2), new(file: 4, rank: 2),
        ];
        from = new(file: 5, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 2, rank: 4), new(file: 1, rank: 6), new(file: 2, rank: 6)
        ];
        from = new(file: 1, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 8, rank: 4), new(file: 9, rank: 6), new(file: 8, rank: 6)
        ];
        from = new(file: 9, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_BlackMovementFromPieceStandPositionIsAppropriate()
    {
        SilverMovement m = new();
        PieceStandPosition from = new("S*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(81, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
    }

    [Fact]
    public void Test_WhiteMovementFromPieceStandPositionIsAppropriate()
    {
        SilverMovement m = new();
        PieceStandPosition from = new("s*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(81, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
    }
}

public class GoldMovementTest
{
    [Fact]
    public void Test_BlackMovementFromTileIsAppropriate()
    {
        GoldMovement m = new();
        Tile from = new(file: 5, rank: 5);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 4), new(file: 5, rank: 4), new(file: 4, rank: 4),
            new(file: 6, rank: 5), new(file: 4, rank: 5),
            new(file: 5, rank: 6)
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_WhiteMovementFromTileIsAppropriate()
    {
        GoldMovement m = new(isBlack: false);
        Tile from = new(file: 5, rank: 5);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 6), new(file: 5, rank: 6), new(file: 4, rank: 6),
            new(file: 6, rank: 5), new(file: 4, rank: 5),
            new(file: 5, rank: 4)
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_BlackMovementFromTileIsAppropriateAtTheCorner()
    {
        GoldMovement m = new();

        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 1), new(file: 4, rank: 1), new(file: 5, rank: 2),
        ];
        Tile from = new(file: 5, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 6, rank: 8), new(file: 5, rank: 8), new(file: 4, rank: 8),
            new(file: 6, rank: 9), new(file: 4, rank: 9)
        ];
        from = new(file: 5, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 2, rank: 4), new(file: 1, rank: 4), new(file: 2, rank: 5), new(file: 1, rank: 6)
        ];
        from = new(file: 1, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 8, rank: 4), new(file: 9, rank: 4), new(file: 8, rank: 5), new(file: 9, rank: 6)
        ];
        from = new(file: 9, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_WhiteMovementFromTileIsAppropriateAtTheCorner()
    {
        GoldMovement m = new(isBlack: false);

        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 9), new(file: 4, rank: 9), new(file: 5, rank: 8),
        ];
        Tile from = new(file: 5, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 6, rank: 2), new(file: 5, rank: 2), new(file: 4, rank: 2),
            new(file: 6, rank: 1), new(file: 4, rank: 1)
        ];
        from = new(file: 5, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 2, rank: 6), new(file: 1, rank: 6), new(file: 2, rank: 5), new(file: 1, rank: 4)
        ];
        from = new(file: 1, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 8, rank: 6), new(file: 9, rank: 6), new(file: 8, rank: 5), new(file: 9, rank: 4)
        ];
        from = new(file: 9, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_BlackMovementFromPieceStandPositionIsAppropriate()
    {
        GoldMovement m = new();
        PieceStandPosition from = new("G*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(81, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
    }

    [Fact]
    public void Test_WhiteMovementFromPieceStandPositionIsAppropriate()
    {
        SilverMovement m = new();
        PieceStandPosition from = new("g*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(81, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
    }
}

public class KingMovementTest
{
    [Fact]
    public void Test_MovementFromTileIsAppropriate()
    {
        KingMovement m = new();
        Tile from = new(file: 5, rank: 5);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 4), new(file: 5, rank: 4), new(file: 4, rank: 4),
            new(file: 6, rank: 5), new(file: 4, rank: 5),
            new(file: 6, rank: 6), new(file: 5, rank: 6), new(file: 4, rank: 6)
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_MovementFromTileIsAppropriateAtTheCorner()
    {
        KingMovement m = new();

        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 1), new(file: 4, rank: 1),
            new(file: 6, rank: 2), new(file: 5, rank: 2), new(file: 4, rank: 2)
        ];
        Tile from = new(file: 5, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 6, rank: 9), new(file: 4, rank: 9),
            new(file: 6, rank: 8), new(file: 5, rank: 8), new(file: 4, rank: 8),
        ];
        from = new(file: 5, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 2, rank: 4), new(file: 1, rank: 4), new(file: 2, rank: 5), new(file: 2, rank: 6), new(file: 1, rank: 6)
        ];
        from = new(file: 1, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 8, rank: 4), new(file: 9, rank: 4), new(file: 8, rank: 5), new(file: 8, rank: 6), new(file: 9, rank: 6)
        ];
        from = new(file: 9, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_MovementFromBlackPieceStandPositionIsAppropriate()
    {
        GoldMovement m = new();
        PieceStandPosition from = new("K*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(81, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
    }

    [Fact]
    public void Test_MovementFromWhitePieceStandPositionIsAppropriate()
    {
        SilverMovement m = new();
        PieceStandPosition from = new("k*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(81, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
    }
}

public class BishopMovementTest
{
    [Fact]
    public void Test_MovementFromTileIsAppropriate()
    {
        BishopMovement m = new();
        Tile from = new(file: 5, rank: 5);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 6), new(file: 7, rank: 7), new(file: 8, rank: 8), new(file: 9, rank: 9),
            new(file: 4, rank: 4), new(file: 3, rank: 3), new(file: 2, rank: 2), new(file: 1, rank: 1),
            new(file: 6, rank: 4), new(file: 7, rank: 3), new(file: 8, rank: 2), new(file: 9, rank: 1),
            new(file: 4, rank: 6), new(file: 3, rank: 7), new(file: 2, rank: 8), new(file: 1, rank: 9),
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_MovementFromTileIsAppropriateAtTheCorner()
    {
        BishopMovement m = new();

        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 2), new(file: 7, rank: 3), new(file: 8, rank: 4), new(file: 9, rank: 5),
            new(file: 4, rank: 2), new(file: 3, rank: 3), new(file: 2, rank: 4), new(file: 1, rank: 5),
        ];
        Tile from = new(file: 5, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 6, rank: 8), new(file: 7, rank: 7), new(file: 8, rank: 6), new(file: 9, rank: 5),
            new(file: 4, rank: 8), new(file: 3, rank: 7), new(file: 2, rank: 6), new(file: 1, rank: 5),
        ];
        from = new(file: 5, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 2, rank: 4), new(file: 3, rank: 3), new(file: 4, rank: 2), new(file: 5, rank: 1),
            new(file: 2, rank: 6), new(file: 3, rank: 7), new(file: 4, rank: 8), new(file: 5, rank: 9),
        ];
        from = new(file: 1, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 8, rank: 4), new(file: 7, rank: 3), new(file: 6, rank: 2), new(file: 5, rank: 1),
            new(file: 8, rank: 6), new(file: 7, rank: 7), new(file: 6, rank: 8), new(file: 5, rank: 9),
        ];
        from = new(file: 9, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_MovementFromBlackPieceStandPositionIsAppropriate()
    {
        BishopMovement m = new();
        PieceStandPosition from = new("B*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(81, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
    }

    [Fact]
    public void Test_MovementFromWhitePieceStandPositionIsAppropriate()
    {
        BishopMovement m = new();
        PieceStandPosition from = new("b*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(81, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
    }
}

public class RookMovementTest
{
    [Fact]
    public void Test_MovementFromTileIsAppropriate()
    {
        RookMovement m = new();
        Tile from = new(file: 5, rank: 5);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 5, rank: 1), new(file: 5, rank: 2), new(file: 5, rank: 3),
            new(file: 5, rank: 4), new(file: 5, rank: 6),
            new(file: 5, rank: 7), new(file: 5, rank: 8), new(file: 5, rank: 9),
            new(file: 1, rank: 5), new(file: 2, rank: 5), new(file: 3, rank: 5),
            new(file: 4, rank: 5), new(file: 6, rank: 5),
            new(file: 7, rank: 5), new(file: 8, rank: 5), new(file: 9, rank: 5),
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_MovementFromTileIsAppropriateAtTheCorner()
    {
        RookMovement m = new();

        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 5, rank: 2), new(file: 5, rank: 3),
            new(file: 5, rank: 4), new(file: 5, rank: 5), new(file: 5, rank: 6),
            new(file: 5, rank: 7), new(file: 5, rank: 8), new(file: 5, rank: 9),
            new(file: 1, rank: 1), new(file: 2, rank: 1), new(file: 3, rank: 1),
            new(file: 4, rank: 1), new(file: 6, rank: 1),
            new(file: 7, rank: 1), new(file: 8, rank: 1), new(file: 9, rank: 1),
        ];
        Tile from = new(file: 5, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 5, rank: 1), new(file: 5, rank: 2), new(file: 5, rank: 3),
            new(file: 5, rank: 4), new(file: 5, rank: 5), new(file: 5, rank: 6),
            new(file: 5, rank: 7), new(file: 5, rank: 8),
            new(file: 1, rank: 9), new(file: 2, rank: 9), new(file: 3, rank: 9),
            new(file: 4, rank: 9), new(file: 6, rank: 9),
            new(file: 7, rank: 9), new(file: 8, rank: 9), new(file: 9, rank: 9),
        ];
        from = new(file: 5, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 1, rank: 1), new(file: 1, rank: 2), new(file: 1, rank: 3),
            new(file: 1, rank: 4), new(file: 1, rank: 6),
            new(file: 1, rank: 7), new(file: 1, rank: 8), new(file: 1, rank: 9),
            new(file: 2, rank: 5), new(file: 3, rank: 5),
            new(file: 4, rank: 5), new(file: 5, rank: 5), new(file: 6, rank: 5),
            new(file: 7, rank: 5), new(file: 8, rank: 5), new(file: 9, rank: 5),
        ];
        from = new(file: 1, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 9, rank: 1), new(file: 9, rank: 2), new(file: 9, rank: 3),
            new(file: 9, rank: 4), new(file: 9, rank: 6),
            new(file: 9, rank: 7), new(file: 9, rank: 8), new(file: 9, rank: 9),
            new(file: 1, rank: 5), new(file: 2, rank: 5), new(file: 3, rank: 5),
            new(file: 4, rank: 5), new(file: 5, rank: 5), new(file: 6, rank: 5),
            new(file: 7, rank: 5), new(file: 8, rank: 5),
        ];
        from = new(file: 9, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_MovementFromBlackPieceStandPositionIsAppropriate()
    {
        RookMovement m = new();
        PieceStandPosition from = new("R*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(81, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
    }

    [Fact]
    public void Test_MovementFromWhitePieceStandPositionIsAppropriate()
    {
        RookMovement m = new();
        PieceStandPosition from = new("r*");
        List<Tile> results = [];
        Tile oneOfExpects = new(file: 1, rank: 1);
        Tile twoOfExpects = new(file: 9, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(81, results.Count);
        Assert.Contains(oneOfExpects, results);
        Assert.Contains(twoOfExpects, results);
    }
}

public class HorseMovementTest
{
    [Fact]
    public void Test_MovementFromTileIsAppropriate()
    {
        HorseMovement m = new();
        Tile from = new(file: 5, rank: 5);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 6), new(file: 7, rank: 7), new(file: 8, rank: 8), new(file: 9, rank: 9),
            new(file: 4, rank: 4), new(file: 3, rank: 3), new(file: 2, rank: 2), new(file: 1, rank: 1),
            new(file: 6, rank: 4), new(file: 7, rank: 3), new(file: 8, rank: 2), new(file: 9, rank: 1),
            new(file: 4, rank: 6), new(file: 3, rank: 7), new(file: 2, rank: 8), new(file: 1, rank: 9),
            new(file: 5, rank: 4), new(file: 5, rank: 6), new(file: 6, rank: 5), new(file: 4, rank: 5)
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_MovementFromTileIsAppropriateAtTheCorner()
    {
        HorseMovement m = new();

        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 6, rank: 2), new(file: 7, rank: 3), new(file: 8, rank: 4), new(file: 9, rank: 5),
            new(file: 4, rank: 2), new(file: 3, rank: 3), new(file: 2, rank: 4), new(file: 1, rank: 5),
            new(file: 5, rank: 2), new(file: 6, rank: 1), new(file: 4, rank: 1)
        ];
        Tile from = new(file: 5, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 6, rank: 8), new(file: 7, rank: 7), new(file: 8, rank: 6), new(file: 9, rank: 5),
            new(file: 4, rank: 8), new(file: 3, rank: 7), new(file: 2, rank: 6), new(file: 1, rank: 5),
            new(file: 5, rank: 8), new(file: 6, rank: 9), new(file: 4, rank: 9)
        ];
        from = new(file: 5, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 2, rank: 4), new(file: 3, rank: 3), new(file: 4, rank: 2), new(file: 5, rank: 1),
            new(file: 2, rank: 6), new(file: 3, rank: 7), new(file: 4, rank: 8), new(file: 5, rank: 9),
            new(file: 1, rank: 4), new(file: 1, rank: 6), new(file: 2, rank: 5)
        ];
        from = new(file: 1, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 8, rank: 4), new(file: 7, rank: 3), new(file: 6, rank: 2), new(file: 5, rank: 1),
            new(file: 8, rank: 6), new(file: 7, rank: 7), new(file: 6, rank: 8), new(file: 5, rank: 9),
            new(file: 9, rank: 4), new(file: 9, rank: 6), new(file: 8, rank: 5)
        ];
        from = new(file: 9, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);
    }
}

public class DragonMovementTest
{
    [Fact]
    public void Test_MovementFromTileIsAppropriate()
    {
        DragonMovement m = new();
        Tile from = new(file: 5, rank: 5);
        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 5, rank: 1), new(file: 5, rank: 2), new(file: 5, rank: 3),
            new(file: 5, rank: 4), new(file: 5, rank: 6),
            new(file: 5, rank: 7), new(file: 5, rank: 8), new(file: 5, rank: 9),
            new(file: 1, rank: 5), new(file: 2, rank: 5), new(file: 3, rank: 5),
            new(file: 4, rank: 5), new(file: 6, rank: 5),
            new(file: 7, rank: 5), new(file: 8, rank: 5), new(file: 9, rank: 5),
            new(file: 4, rank: 4), new(file: 6, rank: 6),
            new(file: 4, rank: 6), new(file: 6, rank: 4),
        ];
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile)
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }

        Assert.Equal(expects.Count, results.Count);
    }

    [Fact]
    public void Test_MovementFromTileIsAppropriateAtTheCorner()
    {
        DragonMovement m = new();

        List<Tile> results = [];
        List<Tile> expects = [
            new(file: 5, rank: 2), new(file: 5, rank: 3),
            new(file: 5, rank: 4), new(file: 5, rank: 5), new(file: 5, rank: 6),
            new(file: 5, rank: 7), new(file: 5, rank: 8), new(file: 5, rank: 9),
            new(file: 1, rank: 1), new(file: 2, rank: 1), new(file: 3, rank: 1),
            new(file: 4, rank: 1), new(file: 6, rank: 1),
            new(file: 7, rank: 1), new(file: 8, rank: 1), new(file: 9, rank: 1),
            new(file: 4, rank: 2), new(file: 6, rank: 2),
        ];
        Tile from = new(file: 5, rank: 1);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) results.Add(tile);
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 5, rank: 1), new(file: 5, rank: 2), new(file: 5, rank: 3),
            new(file: 5, rank: 4), new(file: 5, rank: 5), new(file: 5, rank: 6),
            new(file: 5, rank: 7), new(file: 5, rank: 8),
            new(file: 1, rank: 9), new(file: 2, rank: 9), new(file: 3, rank: 9),
            new(file: 4, rank: 9), new(file: 6, rank: 9),
            new(file: 7, rank: 9), new(file: 8, rank: 9), new(file: 9, rank: 9),
            new(file: 4, rank: 8), new(file: 6, rank: 8),
        ];
        from = new(file: 5, rank: 9);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 1, rank: 1), new(file: 1, rank: 2), new(file: 1, rank: 3),
            new(file: 1, rank: 4), new(file: 1, rank: 6),
            new(file: 1, rank: 7), new(file: 1, rank: 8), new(file: 1, rank: 9),
            new(file: 2, rank: 5), new(file: 3, rank: 5),
            new(file: 4, rank: 5), new(file: 5, rank: 5), new(file: 6, rank: 5),
            new(file: 7, rank: 5), new(file: 8, rank: 5), new(file: 9, rank: 5),
            new(file: 2, rank: 4), new(file: 2, rank: 6),
        ];
        from = new(file: 1, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);

        results = [];
        expects = [
            new(file: 9, rank: 1), new(file: 9, rank: 2), new(file: 9, rank: 3),
            new(file: 9, rank: 4), new(file: 9, rank: 6),
            new(file: 9, rank: 7), new(file: 9, rank: 8), new(file: 9, rank: 9),
            new(file: 1, rank: 5), new(file: 2, rank: 5), new(file: 3, rank: 5),
            new(file: 4, rank: 5), new(file: 5, rank: 5), new(file: 6, rank: 5),
            new(file: 7, rank: 5), new(file: 8, rank: 5),
            new(file: 8, rank: 4), new(file: 8, rank: 6),
        ];
        from = new(file: 9, rank: 5);
        foreach (var t in m.Dest(from))
        {
            if (t is Tile tile) 
            {
                results.Add(tile);
                Assert.Contains(tile, expects);
            }
        }
        Assert.Equal(expects.Count, results.Count);
    }
}
