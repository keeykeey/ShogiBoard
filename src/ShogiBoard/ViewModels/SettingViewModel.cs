using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ShogiBoard.Models;

namespace ShogiBoard.ViewModels;

public partial class SettingViewModel(EngineSettingStore engineSettingStore, TimerStore timerStore) : ViewModelBase
{
    public TimerStore TimerStore { get; set; } = timerStore;
    private EngineSettingStore EngineSettingStore { get; set; } = engineSettingStore;
    public ObservableCollection<int> Byoyomi { get; } = [5, 10, 30, 60];
    public ObservableCollection<int> TimeMinutes { get; } = [5, 10, 15, 30, 60, 120, 180];

    [ObservableProperty]
    public partial int ByoyomiSelected { get; set;} = timerStore.TimeSetting.ByoyomiSec;

    [ObservableProperty]
    public partial int TimeMinutesSelected { get; set;} = timerStore.TimeSetting.TimeControlMin;

    public void SetEnginePath(string path)
    {
        EngineSettingStore.EnginePath = path;
    }
}