namespace ShogiBoard.Models;

public class Position
{
    public int Files { get; set; } // columns of the board
    public int Ranks { get; set; } // rows of the board

    public Position(int file, int rank)
    {
        if (file > 9 || rank > 9 || file < 1 || rank < 1)
        {
            throw new System.Exception($"Unexpected Position (file: {file}, rank: {rank})");
        }
        Files = file;
        Ranks = rank;
    }
}