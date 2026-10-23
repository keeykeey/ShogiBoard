using System;
namespace ShogiBoard.Models;

// promote: 7g7f+
// move from piece stand: P*5e
public struct Move
{
    private IPosition MoveFrom { get; set; }
    private IPosition MoveTo { get; set; }
    private bool DoPromote { get; set; } = false;

    public Move(IPosition moveFrom, IPosition moveTo)
    {
        MoveFrom = moveFrom;
        MoveTo = moveTo;
    }

    public Move(string position)
    {
        if (position.Length < 4) throw new ArgumentException("Unexpected Position for Move");

        string from = position[..2];
        string to = position.Substring(2, 2);

        if (position.Length == 4)
        {
            if (char.IsDigit(position[0]))
            {
                MoveFrom = new Tile(from);
                MoveTo = new Tile(to);
            }
            else
            {
                MoveFrom = new PieceStandPosition(from);
                MoveTo = new Tile(to);
            }
        }
        else if (position.EndsWith('+') && position.Length == 5)
        {
            MoveFrom = new Tile(from);
            MoveTo = new Tile(to);
            DoPromote = true;
        }
        else
        {
            throw new ArgumentException("Unexpected position for Move");
        }
    }

    public override readonly string ToString() =>  $"{MoveFrom.MovePosition()}{MoveTo.MovePosition()}{DoPromoteChar}";
    private readonly string DoPromoteChar
    {
        get => DoPromote ? "+" : "";
    }
}
