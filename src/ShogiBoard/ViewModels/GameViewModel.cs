using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
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
    private GameRecordStore GameRecordStore { get; set; }
    public ObservableCollection<BoardRank> Board { get; set; }
    [ObservableProperty] public partial GameTime TimeSetting { get; set; }
    [ObservableProperty] public partial GameTime BlackTimeLeft { get; set; }
    [ObservableProperty] public partial GameTime WhiteTimeLeft { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TopPieceStand), nameof(BottomPieceStand))]
    public partial bool IsFlipped { get; set; } = false;

    public ObservableCollection<PieceOnStand> TopPieceStand    => IsFlipped ? GameState.BlackPieceStand.Data : GameState.WhitePieceStand.Data;
    public ObservableCollection<PieceOnStand> BottomPieceStand => IsFlipped ? GameState.WhitePieceStand.Data : GameState.BlackPieceStand.Data;

    public GameViewModel(IEngineCommunicationService ecs, IGameService gs, IDialogService ds, GameState gameState, GameRecordStore gameRecordStore)
    {
        EngineCommunicator = ecs;
        GameService  = gs;
        DialogService  = ds;
        GameState = gameState;
        GameRecordStore = gameRecordStore;
        GameService.Init9DimBoardData(GameState);
        Board b = GameState.Board;
        Board = [b.A, b.B, b.C, b.D, b.E, b.F, b.G, b.H, b.I];
        TimeSetting = GameState.TimeSetting;
        BlackTimeLeft = GameState.BlackTimeLeft;
        WhiteTimeLeft = GameState.WhiteTimeLeft;
    }

    private string EnginePath { get; set; } = string.Empty;
    private CancellationTokenSource? TimerCts { get; set; } = new();

    [ObservableProperty]
    public partial IPosition? MoveFrom { get; set; } = null;

    private readonly Stack<string> ResponseStacks = [];

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
            await DialogService.ShowMessageAsync(title: "Start engine timeout", message: $"Please check if engine is valid.\nError message: {e.Message}", onCloseText: "ok", h: 120, w: 240);
        }
        catch (Exception e)
        {
            await DialogService.ShowMessageAsync(title: "Start engine failed", message: $"Error message: {e.Message}", onCloseText: "ok", h: 120, w: 240);
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
        
        GameRecordStore.Reset();

        EngineCommunicator.NewGame();
        RestartTimer();

        if (!playerIsBlack)
        {
            await HandleBestMoveFromEngine();
        }
    }

    [RelayCommand]
    private void GameEnd()
    {
        StopTimer();
        GameState.InAGame = false;
        EngineCommunicator.GameDraw();
    }

    [RelayCommand]
    private void FlipBoard()
    {
        IsFlipped = !IsFlipped;
    }

    [RelayCommand]
    private void QuitEngine()
    {
        StopTimer();
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
        GameRecordStore.Reset();
    }

    [RelayCommand]
    private void OnPlayBackPrev()
    {
        if (GameRecordStore.GameRecordCurrent is GameRecordHead)
        {
            return;
        }
        else if (GameRecordStore.GameRecordCurrent is GameRecord gr)
        {
            GameService.BoardDataPrev(
                from: gr.Move.MoveTo,  // intentionally set Move.MoveTo to from argument because this process roll backs the state.
                to: gr.Move.MoveFrom,  // intentionally set Move.MoveFrom to to argument because this process roll backs the state.
                promoted: gr.Move.DoPromote,
                pieceTaked: gr.PieceTaked,
                state: GameState
            );

            IGameRecord? prev = gr.GetPrev();
            if (prev is not null)
            {
                GameRecordStore.GameRecordCurrent = prev;
            }
        }
        else if (GameRecordStore.GameRecordCurrent is GameRecordTail grt)
        {
            IGameRecord? prev = grt.GetPrev();
            if (prev is not null)
            {
                GameRecordStore.GameRecordCurrent = prev;
            }
            OnPlayBackPrev();
        }
    }

    [RelayCommand]
    private void OnPlayBackNext()
    {
        if (GameRecordStore.GameRecordCurrent.GetNext() is GameRecordHead _)
        {
            throw new Exception("Unexpected behavior that GameRecordCurrent.GetNext() returns GameRecordHead");
        }
        else if (GameRecordStore.GameRecordCurrent.GetNext() is GameRecord gr)
        {
            GameService.BoardDataNext(
                from: gr.Move.MoveFrom,  // intentionally set Move.MoveTo to from argument because this process roll backs the state.
                to: gr.Move.MoveTo,  // intentionally set Move.MoveFrom to to argument because this process roll backs the state.
                promote: gr.Move.DoPromote,
                state: GameState
            );

            GameRecordStore.GameRecordCurrent = gr;
        }
        else if (GameRecordStore.GameRecordCurrent.GetNext() is GameRecordTail _)
        {
            return;
        }
    }

    [RelayCommand]
    private void OnPlayBackStart()
    {
        int safeGuard = 500;
        while (GameRecordStore.GameRecordCurrent.GetPrev() is not null)
        {
            OnPlayBackPrev();
            if (--safeGuard <= 0) break;
        }
    }

    [RelayCommand]
    private void OnPlayBackLast()
    {
        int safeGuard = 500;
        while (GameRecordStore.GameRecordCurrent.GetNext() is not null)
        {
            OnPlayBackNext();
            if (--safeGuard <= 0) break;
        }
    }

    private void ResponseReceived(string m)
    {
        Dispatcher.UIThread.Post(() => {
            ResponseStacks.Push(m);
        });
    }

    public void KillEngine()
    {
        StopTimer();
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

    private void GameRecordSetNext(Move m, string pieceToTake)
    {
        int? score = UsiParser.ExtractScoreOfMove(ResponseStacks, m.ToString());
        GameRecord gr = new(
            move: m,
            pieceTaked: pieceToTake,
            score: score,
            byoyomiBlack: BlackTimeLeft.ByoyomiSec,
            timeControlMinBlack: BlackTimeLeft.TimeControlMin,
            timeControlSecBlack: BlackTimeLeft.TimeControlSec,
            byoyomiWhite: WhiteTimeLeft.ByoyomiSec,
            timeControlMinWhite: WhiteTimeLeft.TimeControlMin,
            timeControlSecWhite: WhiteTimeLeft.TimeControlSec);
        ResponseStacks.Clear();

        GameRecordStore.LinkNext(gr);
    }

    private void NextGameState(IPosition from, Tile to, bool playerDoPromote)
    {
        var move = new Move(from, to, playerDoPromote);
        string pieceToTakeOrEmpty = GameState.Board.Get(rank: to.Ranks, file: to.Files);
        GameRecordSetNext(move, pieceToTakeOrEmpty);

        GameService.BoardDataNext(move.MoveFrom, move.MoveTo, move.DoPromote, GameState);
        GameService.NextTurn(GameState);

        if (GameState.InAGame)
        {
            RestartTimer();
        }
    }

    private async Task HandleBestMoveFromEngine()
    {
        try
        {
            IEnumerable<Move> moves = GameRecordStore.Moves();
            string bestMoveResponse = await EngineCommunicator.SendSfen(
                moves,
                BlackTimeLeft,
                WhiteTimeLeft,
                GameState.PlayerIsBlack ? WhiteTimeLeft.ByoyomiMiliSec : BlackTimeLeft.ByoyomiMiliSec);
            string bestMove = UsiParser.ParseBestmove(bestMoveResponse);

            if (bestMove == UsiMessage.RESIGN)
            {
                GameState.InAGame = false;
                StopTimer();
                await DialogService.ShowMessageAsync(title: "Game end", message: "You win !", onCloseText: "ok", h: 120, w: 200);
            }
            else
            {
                Move bm = new(bestMove, GameState.BlackCanMovePiece);
                string pieceToTakeOrEmpty = GameState.Board.Get(rank: bm.MoveTo.Ranks, file: bm.MoveTo.Files);
                GameRecordSetNext(bm, pieceToTakeOrEmpty);
                GameService.BoardDataNext(bm.MoveFrom, bm.MoveTo, bm.DoPromote, GameState);
                GameService.NextTurn(GameState);
                if (GameState.InAGame)
                {
                    RestartTimer();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"HandleBestMoveFromEngine failed {e.Message}");
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

    private static async Task CountTimeControl(GameTime timeLeft, CancellationToken cancellationToken)
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

    private void StopTimer()
    {
        if (TimerCts is null) return;
        TimerCts.Cancel();
        TimerCts.Dispose();
        TimerCts = null;
    }

    private void RestartTimer()
    {
        StopTimer();
        TimerCts = new CancellationTokenSource();
        CountGameTime(TimerCts.Token).Background();
    }
}
