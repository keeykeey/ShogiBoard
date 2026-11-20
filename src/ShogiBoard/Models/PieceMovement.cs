using System;
using System.Collections.Generic;

namespace ShogiBoard.Models;

public interface IMovement
{
    IEnumerable<Tile?> Dest(Tile from);
    IEnumerable<Tile?> Dest(PieceStandPosition from);
}

public class PornMovement(bool isBlack = true) : IMovement
{
    private readonly bool IsBlack = isBlack;
    public IEnumerable<Tile?> Dest(Tile from)
    {

        if (
            (from.Ranks <= 1 && IsBlack)
            || (from.Ranks >= 9 && !IsBlack)
            || (from.Ranks < 1)
            || (from.Ranks > 9)
        ) {
            yield return null;
        } else {
            if (IsBlack)
            {
                yield return new Tile(file: from.Files, rank: from.Ranks - 1);
            }
            else
            {
                yield return new Tile(file: from.Files, rank: from.Ranks + 1);
            }
        }
    }

    public IEnumerable<Tile?> Dest(PieceStandPosition from)
    {
        int rstart = from.IsBlack ? 2 : 1;
        int rend = from.IsBlack ? 9 : 8;

        for (int f = 1; f <= 9; ++f)
        {
            for (int r = rstart; r <= rend; ++r)
            {
                yield return new Tile(file: f, rank: r);
            }
        }
    }
}

public class LanceMovement(bool isBlack = true) : IMovement
{
    private readonly bool IsBlack = isBlack;
    public IEnumerable<Tile?> Dest(Tile from)
    {
        if (
            (from.Ranks <= 1 && IsBlack)
            || (from.Ranks >= 9 && !IsBlack)
            || (from.Ranks < 1)
            || (from.Ranks > 9)
        ) {
            yield return null;
        } else {
            if (IsBlack)
            {
                for (int r = from.Ranks - 1; r >= 1; --r)
                {
                    yield return new Tile(file: from.Files, rank: r);
                }
            }
            else
            {
                for (int r = from.Ranks + 1; r <= 9; ++r)
                {
                    yield return new Tile(file: from.Files, rank: r);
                }
            }
        }
    }

    public IEnumerable<Tile?> Dest(PieceStandPosition from)
    {
        int rstart = from.IsBlack ? 2 : 1;
        int rend = from.IsBlack ? 9 : 8;

        for (int f = 1; f <= 9; ++f)
        {
            for (int r = rstart; r <= rend; ++r)
            {
                yield return new Tile(file: f, rank: r);
            }
        }
    }
}

public class KnightMovement(bool isBlack = true) : IMovement
{
    private readonly bool IsBlack = isBlack;
    public IEnumerable<Tile?> Dest(Tile from)
    {
        if (
            (from.Ranks <= 2 && IsBlack)
            || (from.Ranks >= 8 && !IsBlack)
            || (from.Ranks < 1)
            || (from.Ranks > 9)
        ) {
            yield return null;
        } else {
            if (IsBlack)
            {
                if (from.Files >= 2) yield return new Tile(file: from.Files - 1, rank: from.Ranks - 2);
                if (from.Files <= 8) yield return new Tile(file: from.Files + 1, rank: from.Ranks - 2);
            }
            else
            {
                if (from.Files >= 2) yield return new Tile(file: from.Files - 1, rank: from.Ranks + 2);
                if (from.Files <= 8) yield return new Tile(file: from.Files + 1, rank: from.Ranks + 2);
            }
        }
    }

    public IEnumerable<Tile?> Dest(PieceStandPosition from)
    {
        int rstart = from.IsBlack ? 3 : 1;
        int rend = from.IsBlack ? 9 : 7;

        for (int f = 1; f <= 9; ++f)
        {
            for (int r = rstart; r <= rend; ++r)
            {
                yield return new Tile(file: f, rank: r);
            }
        }
    }
}

public class SilverMovement(bool isBlack = true) : IMovement
{
    private readonly bool IsBlack = isBlack;
    public IEnumerable<Tile?> Dest(Tile from)
    {
        if (
            (from.Ranks < 1)
            || (from.Ranks > 9)
        ) {
            yield return null;
        } else {
            if (IsBlack)
            {
                if (from.Ranks >= 2 && from.Files <= 8) yield return new Tile(rank: from.Ranks - 1, file: from.Files + 1);
                if (from.Ranks >= 2) yield return new Tile(rank: from.Ranks - 1, file: from.Files);
                if (from.Ranks >= 2 && from.Files >= 2) yield return new Tile(rank: from.Ranks - 1, file: from.Files - 1);
                if (from.Ranks <= 8 && from.Files <= 8) yield return new Tile(rank: from.Ranks + 1, file: from.Files + 1);
                if (from.Ranks <= 8 && from.Files >= 2) yield return new Tile(rank: from.Ranks + 1, file: from.Files - 1);
            }
            else
            {
                if (from.Ranks <= 8 && from.Files <= 8) yield return new Tile(rank: from.Ranks + 1, file: from.Files + 1);
                if (from.Ranks <= 8) yield return new Tile(rank: from.Ranks + 1, file: from.Files);
                if (from.Ranks <= 8 && from.Files >= 2) yield return new Tile(rank: from.Ranks + 1, file: from.Files - 1);
                if (from.Ranks >= 2 && from.Files <= 8) yield return new Tile(rank: from.Ranks - 1, file: from.Files + 1);
                if (from.Ranks >= 2 && from.Files >= 2) yield return new Tile(rank: from.Ranks - 1, file: from.Files - 1);
            }
        }
    }

    public IEnumerable<Tile?> Dest(PieceStandPosition from)
    {
        for (int f = 1; f <= 9; ++f)
        {
            for (int r = 1; r <= 9; ++r)
            {
                yield return new Tile(file: f, rank: r);
            }
        }
    }
}

public class GoldMovement(bool isBlack = true) : IMovement
{
    private readonly bool IsBlack = isBlack;
    public IEnumerable<Tile?> Dest(Tile from)
    {
        if (
            (from.Ranks < 1)
            || (from.Ranks > 9)
        ) {
            yield return null;
        } else {
            if (IsBlack)
            {
                if (from.Ranks >= 2 && from.Files <= 8) yield return new Tile(rank: from.Ranks - 1, file: from.Files + 1);
                if (from.Ranks >= 2) yield return new Tile(rank: from.Ranks - 1, file: from.Files);
                if (from.Ranks >= 2 && from.Files >= 2) yield return new Tile(rank: from.Ranks - 1, file: from.Files - 1);
                if (from.Files <= 8) yield return new Tile(rank: from.Ranks, file: from.Files + 1);
                if (from.Files >= 2) yield return new Tile(rank: from.Ranks, file: from.Files - 1);
                if (from.Ranks <= 8) yield return new Tile(rank: from.Ranks + 1, file: from.Files);
            }
            else
            {
                if (from.Ranks <= 8 && from.Files <= 8) yield return new Tile(rank: from.Ranks + 1, file: from.Files + 1);
                if (from.Ranks <= 8) yield return new Tile(rank: from.Ranks + 1, file: from.Files);
                if (from.Ranks <= 8 && from.Files >= 2) yield return new Tile(rank: from.Ranks + 1, file: from.Files - 1);
                if (from.Files <= 8) yield return new Tile(rank: from.Ranks, file: from.Files + 1);
                if (from.Files >= 2) yield return new Tile(rank: from.Ranks, file: from.Files - 1);
                if (from.Ranks >= 2) yield return new Tile(rank: from.Ranks - 1, file: from.Files);
            }
        }
    }

    public IEnumerable<Tile?> Dest(PieceStandPosition from)
    {
        for (int f = 1; f <= 9; ++f)
        {
            for (int r = 1; r <= 9; ++r)
            {
                yield return new Tile(file: f, rank: r);
            }
        }
    }
}

public class KingMovement : IMovement
{
    public IEnumerable<Tile?> Dest(Tile from)
    {
        if (
            (from.Ranks < 1)
            || (from.Ranks > 9)
        ) {
            yield return null;
        } else {
            if (from.Ranks >= 2 && from.Files <= 8) yield return new Tile(rank: from.Ranks - 1, file: from.Files + 1);
            if (from.Ranks >= 2) yield return new Tile(rank: from.Ranks - 1, file: from.Files);
            if (from.Ranks >= 2 && from.Files >= 2) yield return new Tile(rank: from.Ranks - 1, file: from.Files - 1);
            if (from.Files <= 8) yield return new Tile(rank: from.Ranks, file: from.Files + 1);
            if (from.Files >= 2) yield return new Tile(rank: from.Ranks, file: from.Files - 1);
            if (from.Ranks <= 8 && from.Files <= 8) yield return new Tile(rank: from.Ranks + 1, file: from.Files + 1);
            if (from.Ranks <= 8) yield return new Tile(rank: from.Ranks + 1, file: from.Files);
            if (from.Ranks <= 8 && from.Files >= 2) yield return new Tile(rank: from.Ranks + 1, file: from.Files - 1);
        }
    }

    public IEnumerable<Tile?> Dest(PieceStandPosition from)
    {
        for (int f = 1; f <= 9; ++f)
        {
            for (int r = 1; r <= 9; ++r)
            {
                yield return new Tile(file: f, rank: r);
            }
        }
    }
}

public class BishopMovement : IMovement
{
    public IEnumerable<Tile?> Dest(Tile from)
    {
        if (
            (from.Ranks < 1)
            || (from.Ranks > 9)
        ) {
            yield return null;
        } else {
            for (int distance = 1; distance <= 8; ++distance)
            {
                int f = from.Files + distance;
                int r = from.Ranks + distance;
                if (f < 1 || f > 9 || r < 1 || r > 9) break;

                yield return new Tile(file: f, rank: r);
            }
            yield return null;

            for (int distance = 1; distance <= 8; ++distance)
            {
                int f = from.Files + distance;
                int r = from.Ranks - distance;
                if (f < 1 || f > 9 || r < 1 || r > 9) break;

                yield return new Tile(file: f, rank: r);
            }
            yield return null;

            for (int distance = 1; distance <= 8; ++distance)
            {
                int f = from.Files - distance;
                int r = from.Ranks + distance;
                if (f < 1 || f > 9 || r < 1 || r > 9) break;

                yield return new Tile(file: f, rank: r);
            }
            yield return null;

            for (int distance = 1; distance <= 8; ++distance)
            {
                int f = from.Files - distance;
                int r = from.Ranks - distance;
                if (f < 1 || f > 9 || r < 1 || r > 9) break;

                yield return new Tile(file: f, rank: r);
            }
            yield return null;
        }
    }

    public IEnumerable<Tile?> Dest(PieceStandPosition from)
    {
        for (int f = 1; f <= 9; ++f)
        {
            for (int r = 1; r <= 9; ++r)
            {
                yield return new Tile(file: f, rank: r);
            }
        }
    }
}

public class RookMovement : IMovement
{
    public IEnumerable<Tile?> Dest(Tile from)
    {
        if (
            (from.Ranks < 1)
            || (from.Ranks > 9)
        ) {
            yield return null;
        } else {
            for (int r = from.Ranks + 1; r <= 9; ++r)
            {
                yield return new Tile(file: from.Files, rank: r);
            }
            yield return null;

            for (int r = from.Ranks - 1; r >= 1; --r)
            {
                yield return new Tile(file: from.Files, rank: r);
            }
            yield return null;

            for (int f = from.Files + 1; f <= 9; ++f)
            {
                yield return new Tile(file: f, rank: from.Ranks);
            }
            yield return null;

            for (int f = from.Files - 1; f >= 1; --f)
            {
                yield return new Tile(file: f, rank: from.Ranks);
            }
            yield return null;
        }
    }

    public IEnumerable<Tile?> Dest(PieceStandPosition from)
    {
        for (int f = 1; f <= 9; ++f)
        {
            for (int r = 1; r <= 9; ++r)
            {
                yield return new Tile(file: f, rank: r);
            }
        }
    }
}

public class HorseMovement : IMovement
{
    public IEnumerable<Tile?> Dest(Tile from)
    {
        for (int distance = 1; distance <= 8; ++distance)
        {
            int f = from.Files + distance;
            int r = from.Ranks + distance;
            if (f < 1 || f > 9 || r < 1 || r > 9) break;

            yield return new Tile(file: f, rank: r);
        }
        yield return null;

        for (int distance = 1; distance <= 8; ++distance)
        {
            int f = from.Files + distance;
            int r = from.Ranks - distance;
            if (f < 1 || f > 9 || r < 1 || r > 9) break;

            yield return new Tile(file: f, rank: r);
        }
        yield return null;

        for (int distance = 1; distance <= 8; ++distance)
        {
            int f = from.Files - distance;
            int r = from.Ranks + distance;
            if (f < 1 || f > 9 || r < 1 || r > 9) break;

            yield return new Tile(file: f, rank: r);
        }
        yield return null;

        for (int distance = 1; distance <= 8; ++distance)
        {
            int f = from.Files - distance;
            int r = from.Ranks - distance;
            if (f < 1 || f > 9 || r < 1 || r > 9) break;

            yield return new Tile(file: f, rank: r);
        }
        yield return null;

        if (from.Ranks >= 2)
        {
            yield return new Tile(rank: from.Ranks - 1, file: from.Files);
            yield return null;
        }

        if (from.Files <= 8)
        {
            yield return new Tile(rank: from.Ranks, file: from.Files + 1);
            yield return null;
        }

        if (from.Files >= 2)
        {
            yield return new Tile(rank: from.Ranks, file: from.Files - 1);
            yield return null;
        }

        if (from.Ranks <= 8)
        {
            yield return new Tile(rank: from.Ranks + 1, file: from.Files);
            yield return null;
        }
    }

    public IEnumerable<Tile?> Dest(PieceStandPosition from)
    {
        for (int f = 1; f <= 9; ++f)
        {
            for (int r = 1; r <= 9; ++r)
            {
                yield return new Tile(file: f, rank: r);
            }
        }
    }
}

public class DragonMovement : IMovement
{
    public IEnumerable<Tile?> Dest(Tile from)
    {
        for (int r = from.Ranks + 1; r <= 9; ++r)
        {
            yield return new Tile(file: from.Files, rank: r);
        }
        yield return null;

        for (int r = from.Ranks - 1; r >= 1; --r)
        {
            yield return new Tile(file: from.Files, rank: r);
        }
        yield return null;

        for (int f = from.Files + 1; f <= 9; ++f)
        {
            yield return new Tile(file: f, rank: from.Ranks);
        }
        yield return null;

        for (int f = from.Files - 1; f >= 1; --f)
        {
            yield return new Tile(file: f, rank: from.Ranks);
        }
        yield return null;

        if (from.Ranks >= 2 && from.Files <= 8)
        {
            yield return new Tile(rank: from.Ranks - 1, file: from.Files + 1);
            yield return null;
        }

        if (from.Ranks >= 2 && from.Files >= 2)
        {
            yield return new Tile(rank: from.Ranks - 1, file: from.Files - 1);
            yield return null;
        }

        if (from.Ranks <= 8 && from.Files <= 8)
        {
            yield return new Tile(rank: from.Ranks + 1, file: from.Files + 1);
            yield return null;
        }

        if (from.Ranks <= 8 && from.Files >= 2)
        {
            yield return new Tile(rank: from.Ranks + 1, file: from.Files - 1);
            yield return null;
        }
    }

    public IEnumerable<Tile?> Dest(PieceStandPosition from)
    {
        for (int f = 1; f <= 9; ++f)
        {
            for (int r = 1; r <= 9; ++r)
            {
                yield return new Tile(file: f, rank: r);
            }
        }
    }
}

public class PieceMovementFactory()
{
    public static IMovement Get(string piece) => piece switch
    {
        "p" => new PornMovement(isBlack: false),
        "P" => new PornMovement(),
        "l" => new LanceMovement(isBlack: false),
        "L" => new LanceMovement(),
        "n" => new KnightMovement(isBlack: false),
        "N" => new KnightMovement(),
        "s" => new SilverMovement(isBlack: false),
        "S" => new SilverMovement(),
        "g" => new GoldMovement(isBlack: false),
        "G" => new GoldMovement(),
        "K" => new KingMovement(),
        "k" => new KingMovement(),
        "b" => new BishopMovement(),
        "B" => new BishopMovement(),
        "r" => new RookMovement(),
        "R" => new RookMovement(),
        "+p" => new GoldMovement(isBlack: false),
        "+P" => new GoldMovement(),
        "+l" => new GoldMovement(isBlack: false),
        "+L" => new GoldMovement(),
        "+n" => new GoldMovement(isBlack: false),
        "+N" => new GoldMovement(),
        "+s" => new GoldMovement(isBlack: false),
        "+S" => new GoldMovement(),
        "+b" => new HorseMovement(),
        "+B" => new HorseMovement(),
        "+r" => new DragonMovement(),
        "+R" => new DragonMovement(),
        _ => throw new ArgumentException("Unexpected piece string"),
    };
}