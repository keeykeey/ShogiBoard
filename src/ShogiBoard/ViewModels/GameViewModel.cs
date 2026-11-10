using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShogiBoard.Models;
using ShogiBoard.Services;
using System.Threading.Tasks;

namespace ShogiBoard.ViewModels;

public partial class GameViewModel : ViewModelBase
{
    public IEngineCommunicationService EngineCommunicator { get; }
    public IGameService GameService { get; }
    public IDialogService DialogService { get; }
    private GameState GameState { get; set; }
    public ObservableCollection<BoardRank> Board { get; set; }
    public ObservableCollection<PieceOnStand> BlackPieceStand { get; set; }
    public ObservableCollection<PieceOnStand> WhitePieceStand { get; set; }

    public GameViewModel(IEngineCommunicationService ecs, IGameService gs, IDialogService ds)
    {
        EngineCommunicator = ecs;
        GameService  = gs;
        DialogService  = ds;
        GameState = new();
        GameService.Init9DimBoardData(GameState);
        Board b = GameState.Board;
        Board = [b.A, b.B, b.C, b.D, b.E, b.F, b.G, b.H, b.I];
        BlackPieceStand = GameState.BlackPieceStand.Data;
        WhitePieceStand = GameState.WhitePieceStand.Data;
    }

    private string EnginePath { get; set; } = string.Empty;

    [ObservableProperty]
    public partial IPosition? MoveFrom { get; set; } = null;

    [ObservableProperty]
    public partial string Response { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Message { get; set; } = string.Empty;

    [RelayCommand]
    private async Task StartEngine()
    {
        if (EnginePath == string.Empty)
        {
            await DialogService.ShowErrorAsync(title: "Engine is not selected", message: "Please select engine path", onCloseText: "ok", h: 120, w: 240);
            return;
        }

        try
        {
            await EngineCommunicator.StartAsync(EnginePath, ResponseReceived);
        }
        catch (TimeoutException e)
        {
            Response = e.Message;
            await DialogService.ShowErrorAsync(title: "Start engine timeout", message: $"Please check if engine is valid.\nError message: {Response}", onCloseText: "ok", h: 120, w: 240);
        }
        catch (Exception e)
        {
            Response = e.Message;
            await DialogService.ShowErrorAsync(title: "Start engine failed", message: $"Error message: {Response}", onCloseText: "ok", h: 120, w: 240);
        }
    }

    [RelayCommand]
    private void NewGame()
    {
        if (GameState.InAGame)
        {
            EngineCommunicator.GameDraw();
        }

        // TODO: let user to select black or white
        GameService.InitGameState(gameState: GameState, inAGame: true, playerIsBlack: true, blackCanMovePiece: true);

        EngineCommunicator.NewGame();
    }

    [RelayCommand]
    private void GameEnd()
    {
        GameState.InAGame = false;
        EngineCommunicator.GameDraw();
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
        ClearMoveFrom();
    }

    [RelayCommand]
    private void InitializeBoard()
    {
        if (GameState.InAGame) return;
        // TODO: let user to select black or white
        GameService.InitGameState(gameState: GameState, inAGame: false, playerIsBlack: true, blackCanMovePiece: true);
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
        if (!GameState.PlayerCanMovePiece) return;
        if (col < 0 || col > 8 || row < 0 || row > 8) return;
        int file = 9 - col;
        int rank = 1 + row;
        var pos = new Tile(file, rank);

        bool playerDoPromote = false;
        if (MoveFrom is Tile from)
        {
            if (from.Equals(pos)) return;

            if (GameService.CanPromote(from, pos, GameState))
            {
                playerDoPromote = await DialogService.ShowConfirmAsync(
                    title: "Do you promote ?", message: "Do you promote ?",
                    yes: "yes", no: "no", h: 120, w: 240
                );
            }
        }
        else if (MoveFrom is PieceStandPosition)
        {
            // need to do nothing.
        }
        else
        {
            MoveFrom = pos;
            return;
        }

        NextGameState(MoveFrom, pos, playerDoPromote);

        if (GameState.InAGame)
        {
            await HandleBestMoveFromEngine();
        }
        MoveFrom = null;
    }

    public void ClickPieceStand(string piece)
    {
        if (!GameState.PlayerCanMovePiece) return;
        PieceStandPosition psp = new(piece);
        MoveFrom = psp;
    }

    public void SetEnginePath(string path)
    {
        EnginePath = path;
    }

    private void ClearMoveFrom()
    {
        MoveFrom = null;
    }

    private void NextGameState(IPosition from, Tile to, bool playerDoPromote)
    {
        if (!GameService.IsValidMove(from, to))
        {
            ClearMoveFrom();
            return;
        }

        var move = new Move(from, to, playerDoPromote);

        GameState.Moves.Add(move);
        GameService.BoardDataNext(move.MoveFrom, move.MoveTo, move.DoPromote, GameState);
        GameService.NextTurn(GameState);
    }

    private async Task HandleBestMoveFromEngine()
    {
        try
        {
            string bestMoveResponse = await EngineCommunicator.SendSfen(GameState.Moves);
            string bestMove = UsiParser.ParseBestmove(bestMoveResponse);

            if (bestMove == UsiMessage.RESIGN)
            {
                GameState.InAGame = false;
            }
            else
            {
                Move bm = new(bestMove, GameState.BlackCanMovePiece);
                GameState.Moves.Add(bm);
                GameService.BoardDataNext(bm.MoveFrom, bm.MoveTo, bm.DoPromote, GameState);
                GameService.NextTurn(GameState);
            }
        }
        catch (Exception e)
        {
            Response = e.Message;
            GameState.InAGame = false;
        }
    }
}
