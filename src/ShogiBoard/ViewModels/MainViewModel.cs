using CommunityToolkit.Mvvm.ComponentModel;

namespace ShogiBoard.ViewModels;

public partial class MainViewModel(GameViewModel gameViewModel) : ViewModelBase
{
    public GameViewModel GameViewModel { get; } = gameViewModel;
}
