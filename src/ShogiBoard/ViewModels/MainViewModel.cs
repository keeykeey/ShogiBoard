namespace ShogiBoard.ViewModels;

public partial class MainViewModel(SettingViewModel settingViewModel, GameViewModel gameViewModel) : ViewModelBase
{
    public SettingViewModel SettingViewModel { get; } = settingViewModel;
    public GameViewModel GameViewModel { get; } = gameViewModel;
}
