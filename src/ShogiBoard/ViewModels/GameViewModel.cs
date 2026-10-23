using System;
using System.IO;
using System.Collections.Generic;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShogiBoard.Models;
using ShogiBoard.Services;
using System.Threading.Tasks;

namespace ShogiBoard.ViewModels;

public partial class GameViewModel(IEngineCommunicationService ecs, IGameService gs) : ViewModelBase
{
    public IEngineCommunicationService EngineCommunicator { get; } = ecs;
    public IGameService GameService { get; } = gs;

    private List<Move> Moves { get; set; } = [];
    private bool InAGame = false;
    private bool PlayerIsBlack { get; set; } = true;
    private bool BlackCanMovePiece { get; set; } = true;
    private bool PlayerCanMovePiece
    {
        get => !InAGame || (PlayerIsBlack == BlackCanMovePiece);
    }

    [ObservableProperty]
    public partial IPosition? MoveFrom { get; set; } = null;

    [ObservableProperty]
    public partial string Response { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Message { get; set; } = string.Empty;

    [RelayCommand]
    private async Task StartEngine()
    {
        string executablePath = Path.Combine(AppContext.BaseDirectory, "engine", "Lesserkai");
        try
        {
            await EngineCommunicator.StartAsync(executablePath, ResponseReceived);
        }
        catch (Exception e)
        {
            Response = e.Message;
        }
    }

    [RelayCommand]
    private void NewGame()
    {
        if (InAGame)
        {
            EngineCommunicator.GameDraw();
        }
        else
        {
            InAGame = true;
        }
        PlayerIsBlack = true;
        BlackCanMovePiece = true;

        EngineCommunicator.NewGame();
        ClearMoves();
    }

    [RelayCommand]
    private void GameEnd()
    {
        InAGame = false;
        EngineCommunicator.GameDraw();
        ClearMoves();
    }

    [RelayCommand]
    private void SendMessage()
    {
        EngineCommunicator.Send(Message);
    }

    [RelayCommand]
    private void QuitEngine()
    {
        EngineCommunicator.Quit();
        ClearMoves();
    }

    [RelayCommand]
    private void InitializeBoard()
    {
        if (InAGame) return;
        ClearMoves();
    }

    private void ResponseReceived(string m)
    {
        Dispatcher.UIThread.Post(() => { Response = m; });
    }

    public void KillEngine()
    {
        EngineCommunicator.Kill();
    }

    public async Task ClickBoard(int col, int row)
    {
        if (!PlayerCanMovePiece) return;
        if (col < 0 || col > 8 || row < 0 || row > 8) return;
        int file = 9 - col;
        int rank = 1 + row;
        var pos = new Tile(file, rank);

        if (MoveFrom is Tile from)
        {
            if (from.Equals(pos)) return;

            var move = new Move(from, pos);

            if (!GameService.IsValidMove(move))
            {
                ClearPositions();
                return;
            }

            Moves.Add(move);

            if (InAGame)
            {
                BlackCanMovePiece = !BlackCanMovePiece;
                try
                {
                    string bestMoveResponse = await EngineCommunicator.SendSfen(Moves);
                    string bestMove = UsiParser.ParseBestmove(bestMoveResponse);

                    if (bestMove == UsiMessage.RESIGN)
                    {
                        InAGame = false;
                    }
                    else
                    {
                        Move bm = new(bestMove);
                        Moves.Add(bm);
                        BlackCanMovePiece = !BlackCanMovePiece;
                        Console.WriteLine(bm);
                    }
                }
                catch (Exception e)
                {
                    Response = e.Message;
                    InAGame = false;
                }
            }

            MoveFrom = null;
        } else
        {
            MoveFrom = pos;
        }
    }

    private void ClearMoves()
    {
        Moves = [];
        ClearPositions();
    }

    private void ClearPositions()
    {
        MoveFrom = null;
    }
}

