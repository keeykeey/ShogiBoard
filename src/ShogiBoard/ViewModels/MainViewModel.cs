namespace ShogiBoard.ViewModels;

public partial class MainViewModel(GameViewModel gameViewModel, EvaluationViewModel evm) : ViewModelBase
{
    public GameViewModel GameViewModel { get; } = gameViewModel;
    public EvaluationViewModel EvaluationViewModel { get; } = evm;
}
