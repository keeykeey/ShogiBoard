using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShogiBoard.Models;
using ShogiBoard.Services;
using System.Threading.Tasks;

namespace ShogiBoard.ViewModels;

public partial class GameViewModel : ViewModelBase
{
    public PlaybackViewModel PlaybackViewModel { get; }
    private IEngineCommunicationService EngineCommunicator { get; }
    private IGameService GameService { get; }
    private IDialogService DialogService { get; }
    private ITimerService TimerService { get; }
    private GameState GameState { get; set; }
    private GameRecordStore GameRecordStore { get; set; }
    public ObservableCollection<BoardRank> Board { get; set; }
    public TimerStore TimerStore { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TopPieceStand), nameof(BottomPieceStand))]
    public partial bool IsFlipped { get; set; } = false;

    public ObservableCollection<PieceOnStand> TopPieceStand    => IsFlipped ? GameState.BlackPieceStand.Data : GameState.WhitePieceStand.Data;
    public ObservableCollection<PieceOnStand> BottomPieceStand => IsFlipped ? GameState.WhitePieceStand.Data : GameState.BlackPieceStand.Data;

    public GameViewModel(PlaybackViewModel playbackViewModel, IEngineCommunicationService ecs, IGameService gs, IDialogService ds, ITimerService timerService, GameState gameState, GameRecordStore gameRecordStore, TimerStore timerStore)
    {
        PlaybackViewModel = playbackViewModel;
        EngineCommunicator = ecs;
        GameService  = gs;
        DialogService  = ds;
        TimerService = timerService;
        GameState = gameState;
        GameRecordStore = gameRecordStore;
        GameService.Init9DimBoardData(GameState);
        Board b = GameState.Board;
        Board = [b.A, b.B, b.C, b.D, b.E, b.F, b.G, b.H, b.I];
        TimerStore = timerStore;
    }

    private string EnginePath { get; set; } = string.Empty;

    private IPosition? MoveFrom { get; set; } = null;

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
            await EngineCommunicator.StartAsync(EnginePath);
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
        TimerStore.Reset();
        
        GameRecordStore.Reset();

        EngineCommunicator.NewGame();
        TimerService.RestartTimer(timerStore: TimerStore, turnIsBlack: GameState.BlackCanMovePiece, TimeLose);

        if (!playerIsBlack)
        {
            await HandleBestMoveFromEngine();
        }
    }

    [RelayCommand]
    private void GameEnd()
    {
        TimerService.StopTimer(timerStore: TimerStore);
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
        TimerService.StopTimer(timerStore: TimerStore);
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

    public void KillEngine()
    {
        TimerService.StopTimer(timerStore: TimerStore);
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
        int? score = UsiParser.ExtractScoreOfMove(EngineCommunicator.PopAllResponseStack(), m.ToString());
        GameRecord gr = new(
            move: m,
            pieceTaked: pieceToTake,
            score: score,
            byoyomiBlack: TimerStore.BlackTimeLeft.ByoyomiSec,
            timeControlMinBlack: TimerStore.BlackTimeLeft.TimeControlMin,
            timeControlSecBlack: TimerStore.BlackTimeLeft.TimeControlSec,
            byoyomiWhite: TimerStore.WhiteTimeLeft.ByoyomiSec,
            timeControlMinWhite: TimerStore.WhiteTimeLeft.TimeControlMin,
            timeControlSecWhite: TimerStore.WhiteTimeLeft.TimeControlSec);

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
            TimerService.RestartTimer(timerStore: TimerStore, turnIsBlack: GameState.BlackCanMovePiece, TimeLose);
        }
    }

    private async Task HandleBestMoveFromEngine()
    {
        try
        {
            IEnumerable<Move> moves = GameRecordStore.Moves();
            string bestMoveResponse = await EngineCommunicator.SendSfen(
                moves,
                TimerStore.BlackTimeLeft,
                TimerStore.WhiteTimeLeft,
                GameState.PlayerIsBlack ? TimerStore.WhiteTimeLeft.ByoyomiMiliSec : TimerStore.BlackTimeLeft.ByoyomiMiliSec);
            string bestMove = UsiParser.ParseBestmove(bestMoveResponse);

            if (bestMove == UsiMessage.RESIGN)
            {
                GameState.InAGame = false;
                TimerService.StopTimer(TimerStore);
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
                    TimerService.RestartTimer(TimerStore, GameState.BlackCanMovePiece, TimeLose);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"HandleBestMoveFromEngine failed {e.Message}");
            GameState.InAGame = false;
        }
    }

    private async Task TimeLose()
    {
        GameEnd();
        await DialogService.ShowMessageAsync(title: "Game End", message: "You lose", onCloseText: "ok", h: 120, w: 200);
    }
}
