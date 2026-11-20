using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShogiBoard.Models;
using ShogiBoard.Services;
using ShogiBoard.Helper;
using System.Threading.Tasks;
using System.Threading;

namespace ShogiBoard.ViewModels;

public partial class GameViewModel : ViewModelBase
{
    public IEngineCommunicationService EngineCommunicator { get; }
    public IGameService GameService { get; }
    public IDialogService DialogService { get; }
    private GameState GameState { get; set; }
    public ObservableCollection<BoardRank> Board { get; set; }
    [ObservableProperty] public partial GameTime TimeSetting { get; set; }
    [ObservableProperty] public partial GameTime BlackTimeLeft { get; set; }
    [ObservableProperty] public partial GameTime WhiteTimeLeft { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TopPieceStand), nameof(BottomPieceStand))]
    public partial bool IsFlipped { get; set; } = false;

    public ObservableCollection<PieceOnStand> TopPieceStand    => IsFlipped ? GameState.BlackPieceStand.Data : GameState.WhitePieceStand.Data;
    public ObservableCollection<PieceOnStand> BottomPieceStand => IsFlipped ? GameState.WhitePieceStand.Data : GameState.BlackPieceStand.Data;

    public GameViewModel(IEngineCommunicationService ecs, IGameService gs, IDialogService ds)
    {
        EngineCommunicator = ecs;
        GameService  = gs;
        DialogService  = ds;
        GameState = new(byoyomi: 10, timeControlMin: 0, timeControlSec: 0);
        GameService.Init9DimBoardData(GameState);
        Board b = GameState.Board;
        Board = [b.A, b.B, b.C, b.D, b.E, b.F, b.G, b.H, b.I];
        TimeSetting = GameState.TimeSetting;
        BlackTimeLeft = GameState.BlackTimeLeft;
        WhiteTimeLeft = GameState.WhiteTimeLeft;
    }

    private string EnginePath { get; set; } = string.Empty;
    private CancellationTokenSource Cts { get; set; } = new();

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
            await DialogService.ShowMessageAsync(title: "Engine is not selected", message: "Please select engine path", onCloseText: "ok", h: 120, w: 240);
            return;
        }

        try
        {
            await EngineCommunicator.StartAsync(EnginePath, ResponseReceived);
        }
        catch (TimeoutException e)
        {
            Response = e.Message;
            await DialogService.ShowMessageAsync(title: "Start engine timeout", message: $"Please check if engine is valid.\nError message: {Response}", onCloseText: "ok", h: 120, w: 240);
        }
        catch (Exception e)
        {
            Response = e.Message;
            await DialogService.ShowMessageAsync(title: "Start engine failed", message: $"Error message: {Response}", onCloseText: "ok", h: 120, w: 240);
        }
    }

    [RelayCommand]
    private async Task NewGame()
    {
        if (GameState.InAGame)
        {
            bool ok = await DialogService.ShowConfirmAsync(title: "Start new game", "Are you sure you quit current game and start new?", yes: "yes", no: "no", h: 120, w: 240);
            if (!ok) return;

            EngineCommunicator.GameDraw();
        }

        bool playerIsBlack = await DialogService.ShowConfirmAsync(title: "black or white", "Choose black or white", yes: "black", no: "white", h: 120, w: 240);

        IsFlipped = !playerIsBlack;
        GameService.InitGameState(gameState: GameState, inAGame: true, playerIsBlack: playerIsBlack, blackCanMovePiece: true);
        BlackTimeLeft.Set(TimeSetting.ByoyomiSec, TimeSetting.TimeControlMin, TimeSetting.TimeControlSec);
        WhiteTimeLeft.Set(TimeSetting.ByoyomiSec, TimeSetting.TimeControlMin, TimeSetting.TimeControlSec);

        EngineCommunicator.NewGame();
        CountGameTime(Cts.Token).Background();

        if (!playerIsBlack)
        {
            await HandleBestMoveFromEngine();
        }
    }

    [RelayCommand]
    private void GameEnd()
    {
        Cts.Cancel();
        GameState.InAGame = false;
        EngineCommunicator.GameDraw();
    }

    [RelayCommand]
    private void FlipBoard()
    {
        IsFlipped = !IsFlipped;
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
        GameState.InAGame = false;
    }

    [RelayCommand]
    private async Task InitializeBoard()
    {
        if (GameState.InAGame)
        {
            await DialogService.ShowMessageAsync(title: "Error", "You can't initialize board while you are in a game", onCloseText: "ok", h: 120, w: 240);
            return;
        }
        GameService.InitGameState(gameState: GameState, inAGame: false, playerIsBlack: true, blackCanMovePiece: true);
    }

    private void ResponseReceived(string m)
    {
        Dispatcher.UIThread.Post(() => { Response = m; Console.WriteLine(m); });
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

        if (MoveFrom is null)
        {
            if (GameState.Board.Get(file: pos.Files, rank: pos.Ranks) != string.Empty)
            {
                MoveFrom = pos;
            }
            return;
        }

        if (GameService.IsSamePlayersPiece(MoveFrom, pos, GameState))
        {
            Console.WriteLine("same players piece");
            MoveFrom = pos;
            return;
        }

        if (!GameService.IsValidMove(MoveFrom, pos, GameState))
        {
            Console.WriteLine("not valid move");
            ClearMoveFrom();
            return;
        }


        bool playerDoPromote = false;
        if (MoveFrom is Tile from)
        {
            if (GameService.CanPromote(from, pos, GameState))
            {
                playerDoPromote = await DialogService.ShowConfirmAsync(
                    title: "Do you promote ?", message: "Do you promote ?",
                    yes: "yes", no: "no", h: 120, w: 240
                );
            }
        }
        else if (MoveFrom is PieceStandPosition) { /* Nothing need to do */ }
        else
        {
            throw new Exception($"Unexpected type of MoveFrom: {MoveFrom.GetType()}");
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
        var move = new Move(from, to, playerDoPromote);

        GameState.Moves.Add(move);
        GameService.BoardDataNext(move.MoveFrom, move.MoveTo, move.DoPromote, GameState);
        GameService.NextTurn(GameState);

        if (GameState.InAGame)
        {
            Cts.Cancel();
            Cts = new();
            CountGameTime(Cts.Token).Background();
        }
    }

    private async Task HandleBestMoveFromEngine()
    {
        try
        {
            string bestMoveResponse = await EngineCommunicator.SendSfen(
                GameState.Moves,
                GameState.BlackCanMovePiece ? BlackTimeLeft : WhiteTimeLeft);
            string bestMove = UsiParser.ParseBestmove(bestMoveResponse);

            if (bestMove == UsiMessage.RESIGN)
            {
                GameState.InAGame = false;
                Cts.Cancel();
                await DialogService.ShowMessageAsync(title: "Game end", message: "You win !", onCloseText: "ok", h: 120, w: 200);
            }
            else
            {
                Move bm = new(bestMove, GameState.BlackCanMovePiece);
                GameState.Moves.Add(bm);
                GameService.BoardDataNext(bm.MoveFrom, bm.MoveTo, bm.DoPromote, GameState);
                GameService.NextTurn(GameState);
                if (GameState.InAGame)
                {
                    Cts.Cancel();
                    Cts = new();
                    CountGameTime(Cts.Token).Background();
                }
            }
        }
        catch (Exception e)
        {
            Response = e.Message;
            GameState.InAGame = false;
        }
    }

    private async Task CountGameTime(CancellationToken cts)
    {
        GameTime timeLeft = GameState.BlackCanMovePiece ? BlackTimeLeft : WhiteTimeLeft;
        timeLeft.Set(
            byoyomi:        TimeSetting.ByoyomiSec,
            timeControlMin: timeLeft.TimeControlMin,
            timeControlSec: timeLeft.TimeControlSec
        );

        if (timeLeft.NoTimeLeft)
        {
            bool ok = await CountByoyomi(timeLeft, cts);
            if (!ok)
            {
                GameEnd();
                // TODO: create more appropriate dialog
                await DialogService.ShowMessageAsync(title: "Game End", message: "You lose", onCloseText: "ok", h: 120, w: 200);
            }
        }
        else
        {
            await CountTimeControl(timeLeft, cts);
            if (timeLeft.NoTimeLeft)
            {
                bool ok = await CountByoyomi(timeLeft, cts);
                if (!ok)
                {
                    GameEnd();
                    // TODO: create more appropriate dialog
                    await DialogService.ShowMessageAsync(title: "Game End", message: "You lose", onCloseText: "ok", h: 120, w: 200);
                }
            }
            else
            {
                throw new Exception("Unexpected behavior of CountGameTime");
            }
        }
    }

    private async Task<bool> CountByoyomi(GameTime timeLeft, CancellationToken cancellationToken)
    {
        int c = TimeSetting.ByoyomiSec;
        for (int i = 0; i < c; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            timeLeft.Set(
                byoyomi: timeLeft.ByoyomiSec - 1,
                timeControlMin: null,
                timeControlSec: null
            );
            await Task.Delay(1000, cancellationToken);
        }

        return false;
    }

    private async Task CountTimeControl(GameTime timeLeft, CancellationToken cancellationToken)
    {
        int c = timeLeft.TimeControlMin * 60 + timeLeft.TimeControlSec;
        for (int i = 0; i < c; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int byou = timeLeft.TimeControlSec == 0 ? 59 : timeLeft.TimeControlSec - 1;
            int min = byou == 59 ? timeLeft.TimeControlMin - 1 : timeLeft.TimeControlMin;
            timeLeft.Set(
                byoyomi: timeLeft.ByoyomiSec,
                timeControlMin: min,
                timeControlSec: byou
            );
            await Task.Delay(1000, cancellationToken);
        }
    }
}
