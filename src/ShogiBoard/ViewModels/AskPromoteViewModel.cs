
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;

namespace ShogiBoard.ViewModels;

public partial class AskPromoteViewModel(
    Window dialog,
    string piece
    ) : ViewModelBase
{
    public Window Dialog = dialog;
    public string Piece { get; } = piece;
    public string PiecePromote { get; } = $"+{piece}";

    [RelayCommand]
    private void ClickPromote() => Dialog.Close(true);

    [RelayCommand]
    private void ClickNotPromote() => Dialog.Close(false);

    // [RelayCommand]
    public void SelectDoPromote(string? s) {
        if (s == null)
        {
            throw new System.Exception("SelectDoPromote() got null string");
        }
        bool doPromote = s.Equals(PiecePromote);
        Dialog.Close(doPromote);
    }
}