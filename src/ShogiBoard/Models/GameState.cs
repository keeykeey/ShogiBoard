using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

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
    public ObservableCollection<PieceOnStand> Data = [];
    public void Add(string piece)
    {
        if (piece == string.Empty) return;

        PieceOnStand? found = Data.FirstOrDefault(v => v.Piece.Equals(piece));
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

        PieceOnStand? found = Data.FirstOrDefault(v => v.Piece.Equals(piece));
        found?.Decrement();

        if (found?.Count == 0)
        {
            Data.Remove(found);
        }
    }
}

public class PieceOnStand(string piece, int count): INotifyPropertyChanged
{
    public string Piece { get; set; } = piece;
    public int Count { get; set; } = count;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public void Set(string piece, int count)
    {
        if (count < 0)
        {
            throw new System.ArgumentException("count should be a positive number.");
        }
        Piece = piece;
        Count = count;
        NotifyPropertyChanged(nameof(Piece));
        NotifyPropertyChanged(nameof(Count));
    }

    public void Increment()
    {
        if (Count == int.MaxValue) return;
        Count += 1;
        NotifyPropertyChanged(nameof(Count));
    }

    public void Decrement()
    {
        if (Count < 1) return;
        Count -= 1;
        NotifyPropertyChanged(nameof(Count));
    }
}
