using System;
using System.Diagnostics;
using ShogiBoard.Models;

namespace ShogiBoard.Services;

public interface IGameService
{
    bool IsValidMove(Move m);
}

public class GameService : IGameService
{
    public bool IsValidMove(Move m)
    {
        // TODO
        return true;
    }
}