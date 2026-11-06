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
    public Board Board = new();
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

public class BoardRank : INotifyPropertyChanged
{
    private string One { get; set; } = "";  // File-1 in a Rank
    private string Two { get; set; } = "";  // File-2 in a Rank
    private string Three { get; set; } = ""; // File-3 in a Rank
    private string Four { get; set; } = "";  // File-4 in a Rank
    private string Five { get; set; } = "";  // File-5 in a Rank
    private string Six { get; set; } = "";   // File-6 in a Rank
    private string Seven { get; set; } = ""; // File-7 in a Rank
    private string Eight { get; set; } = ""; // File-8 in a Rank
    private string Nine { get; set; } = "";  // File-9 in a Rank

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string Get(int file) => file switch
    {
        1 => One,
        2 => Two,
        3 => Three,
        4 => Four,
        5 => Five,
        6 => Six,
        7 => Seven,
        8 => Eight,
        9 => Nine,
        _ => throw new System.Exception($"Got unexpected file({file})")
    };

    public void Set(int file, string piece)
    {
        switch (file)
        {
            case 1:
                One = piece;
                NotifyPropertyChanged(nameof(One));
                break;
            case 2:
                Two = piece;
                NotifyPropertyChanged(nameof(Two));
                break;
            case 3:
                Three = piece;
                NotifyPropertyChanged(nameof(Three));
                break;
            case 4:
                Four = piece;
                NotifyPropertyChanged(nameof(Four));
                break;
            case 5:
                Five = piece;
                NotifyPropertyChanged(nameof(Five));
                break;
            case 6:
                Six = piece;
                NotifyPropertyChanged(nameof(Six));
                break;
            case 7:
                Seven = piece;
                NotifyPropertyChanged(nameof(Seven));
                break;
            case 8:
                Eight = piece;
                NotifyPropertyChanged(nameof(Eight));
                break;
            case 9:
                Nine = piece;
                NotifyPropertyChanged(nameof(Nine));
                break;
            default:
                throw new System.Exception($"Got unexpected file({file})");
        }
    }
}

public class Board
{
    private BoardRank A { get; set; } = new();
    private BoardRank B { get; set; } = new();
    private BoardRank C { get; set; } = new();
    private BoardRank D { get; set; } = new();
    private BoardRank E { get; set; } = new();
    private BoardRank F { get; set; } = new();
    private BoardRank G { get; set; } = new();
    private BoardRank H { get; set; } = new();
    private BoardRank I { get; set; } = new();

    public string Get(int rank, int file) => rank switch
    {
        1 => A.Get(file),
        2 => B.Get(file),
        3 => C.Get(file),
        4 => D.Get(file),
        5 => E.Get(file),
        6 => F.Get(file),
        7 => G.Get(file),
        8 => H.Get(file),
        9 => I.Get(file),
        _ => throw new System.Exception($"Got unexpected rank({rank})")
    };

    public void Set(int rank, int file, string piece)
    {
        switch (rank)
        {
            case 1:
                A.Set(file, piece);
                break;
            case 2:
                B.Set(file, piece);
                break;
            case 3:
                C.Set(file, piece);
                break;
            case 4:
                D.Set(file, piece);
                break;
            case 5:
                E.Set(file, piece);
                break;
            case 6:
                F.Set(file, piece);
                break;
            case 7:
                G.Set(file, piece);
                break;
            case 8:
                H.Set(file, piece);
                break;
            case 9:
                I.Set(file, piece);
                break;
            default:
                throw new System.Exception($"Got unexpected file({file})");
        }
    }
}

public class PieceOnBoard(string piece) : INotifyPropertyChanged
{
    public string Piece { get; set; } = piece;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public void Set(string piece)
    {
        Piece = piece;
        NotifyPropertyChanged(nameof(Piece));
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
