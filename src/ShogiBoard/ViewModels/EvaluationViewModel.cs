using System;
using System.Collections.ObjectModel;
using ShogiBoard.Models;

namespace ShogiBoard.ViewModels;

public partial class EvaluationViewModel(GameRecordStore grs) : ViewModelBase
{
    public ObservableCollection<int?> Scores { get; } = grs.Scores;
    public Func<double, string> MoveLabeler { get; } =
        v => v % 1 == 0 && v >= 0 ? (v + 1).ToString("0") : "";
}