using ShogiBoard.Services;
using ShogiBoard.Models;
using System;
using CommunityToolkit.Mvvm.Input;

namespace ShogiBoard.ViewModels;

public partial class PlaybackViewModel : ViewModelBase
{
    private IGameService GameService { get; }
    private GameState GameState { get; set; }
    private GameRecordStore GameRecordStore { get; set; }

    public PlaybackViewModel(IGameService gs, GameState gameState, GameRecordStore gameRecordStore)
    {
        GameService  = gs;
        GameState = gameState;
        GameRecordStore = gameRecordStore;
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
}