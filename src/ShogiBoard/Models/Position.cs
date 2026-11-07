using System;
using System.Text.RegularExpressions;
namespace ShogiBoard.Models;

public interface IPosition
{
    string MovePosition();
}

public partial struct Tile : IPosition
{
    public int Files { get; set; } // columns of the board
    public int Ranks { get; set; } // rows of the board

    [GeneratedRegex(@"^(?<file>[1-9])(?<rank>[a-i])$")]
    public static partial Regex TilePatternRegex();

    public Tile(string position)
    {
        Match match = TilePatternRegex().Match(position);
        if (match.Success)
        {
            Files = int.Parse(match.Groups["file"].Value);
            Ranks = CharToRank(match.Groups["rank"].Value);
        }
        else
        {
            throw new ArgumentException($"Unexpected tile position ({position})");
        }

    }

    public Tile(int file, int rank)
    {
        if (file > 9 || rank > 9 || file < 1 || rank < 1)
        {
            throw new ArgumentException($"Unexpected tile position (file: {file}, rank: {rank})");
        }
        Files = file;
        Ranks = rank;
    }

    public readonly string MovePosition()
    {
        string pos = $"{Files}" + RankToChar(Ranks);
        return pos;
    }

    private static string RankToChar(int rank) => rank switch
    {
        1 => "a",
        2 => "b",
        3 => "c",
        4 => "d",
        5 => "e",
        6 => "f",
        7 => "g",
        8 => "h",
        9 => "i",
        _ => throw new System.ArgumentException("Unexpected Rank"),
    };

    private static int CharToRank(string rank) => rank switch
    {
        "a" => 1,
        "b" => 2,
        "c" => 3,
        "d" => 4,
        "e" => 5,
        "f" => 6,
        "g" => 7,
        "h" => 8,
        "i" => 9,
        _ => throw new System.ArgumentException("Unexpected Rank"),
    };
}

public partial struct PieceStandPosition : IPosition
{
    public bool IsBlack { get; set; }
    public string PieceChar { get; set; }

    [GeneratedRegex(@"^(?<piece>[plnsgkrbPLNSGKRB])\*$")]
    public static partial Regex PieceStandPositionRegex();

    public PieceStandPosition(string position)
    {
        Match match = PieceStandPositionRegex().Match(position);
        if (match.Success)
        {
            PieceChar = match.Groups["piece"].Value;
            IsBlack = char.IsUpper(PieceChar[0]);
        }
        else
        {
            throw new ArgumentException($"Unexpected piece stand position {position}");
        }

    }

    public readonly string MovePosition()
    {
        string pos = $"{PieceChar.ToUpper()}*";
        return pos;
    }
}
