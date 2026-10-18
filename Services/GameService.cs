using System;
using System.Diagnostics;
using ShogiBoard.Models;

namespace ShogiBoard.Services;

public interface IGameService
{
    void ClickBoard(Point2D point);
}

public class GameService : IGameService
{
    public void ClickBoard(Point2D point)
    {
        try
        {
            if (point.X < 0 || point.X > 8 || point.Y < 0 || point.Y > 8) return;
            int file = 9 - point.X;
            int rank = 1 + point.Y;
            var pos = new Position(file, rank);
            Console.WriteLine($"ClickBoard: col={pos.Files}, row={pos.Ranks}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}