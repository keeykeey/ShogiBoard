using System.Collections.Generic;

namespace ShogiBoard.Models;

public class GameState(
    int  boardLength = 9,
    bool inAGame = false,
    bool playerIsBlack = true,
    bool blackCanMovePiece = true)
{
    public int BoardLength = boardLength;
    public string[,] BoardData = new string[boardLength, boardLength];
    public PieceStand BlackPieceStand = new();
    public PieceStand WhitePieceStand = new();
    public List<Move> Moves { get; set; } = [];
    public bool InAGame { get; set; } = inAGame;
    public bool PlayerIsBlack { get; set; } = playerIsBlack;
    public bool BlackCanMovePiece { get; set; } = blackCanMovePiece;
    public bool PlayerCanMovePiece
    {
        get => !InAGame || (PlayerIsBlack == BlackCanMovePiece);
    }
}

public class PieceStand
{
    public List<PieceOnStand> Data = [];
    public void Add(string piece)
    {
        if (piece == string.Empty) return;

        PieceOnStand? found = Data.Find(v => v.Piece.Equals(piece));
        if (found == null)
        {
            PieceOnStand pos = new(piece, 1);
            Data.Add(pos);
        }
        else
        {
            found.Increment();
        }
    }

    public void Remove(string piece)
    {
        if (piece == string.Empty) return;

        PieceOnStand? found = Data.Find(v => v.Piece.Equals(piece));
        found?.Decrement();

        if (found?.Count == 0)
        {
            Data.Remove(found);
        }
    }
}

public class PieceOnStand(string piece, int count)
{
    public string Piece = piece;
    public int Count = count;

    public void Set(string piece, int count)
    {
        if (count < 0)
        {
            throw new System.ArgumentException("count should be a positive number.");
        }
        Piece = piece;
        Count = count;
    }

    public void Increment()
    {
        if (Count == int.MaxValue) return;
        Count += 1;
    }

    public void Decrement()
    {
        if (Count < 1) return;
        Count -= 1;
    }
}
