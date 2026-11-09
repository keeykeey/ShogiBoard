
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;

namespace ShogiBoard.ViewModels;

public partial class ConfirmViewModel(
    Window dialog,
    string title,
    string message,
    string yesText = "yes",
    string noText = "no",
    int height = 200,
    int width = 200
    ) : ViewModelBase
{
    public Window Dialog = dialog;
    public string Title { get; } = title;
    public string Message { get; } = message;
    public string YesText { get; } = yesText;
    public string NoText { get; } = noText;
    public int Height { get; } = height;
    public int Width { get; } = width;

    [RelayCommand]
    private void ClickYes() => Dialog.Close(true);

    [RelayCommand]
    private void ClickNo() => Dialog.Close(false);
}