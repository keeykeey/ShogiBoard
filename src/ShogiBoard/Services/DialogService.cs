using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ShogiBoard.ViewModels;
using ShogiBoard.Views.Dialog;

namespace ShogiBoard.Services;

public interface IDialogService
{
    Task<bool> ShowConfirmAsync(string title, string message, string yes, string no, int h, int w);
    Task<bool> ShowAskPromoteAsync(string piece);
    Task<bool> ShowErrorAsync(string title, string message, string onCloseText, int h, int w);
}

public class DialogService : IDialogService
{
    public async Task<bool> ShowConfirmAsync(
        string title,
        string message,
        string yes = "yes",
        string no = "no",
        int h = 200,
        int w = 200
    ) {
        Window? ownerWindow = null;
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ownerWindow = desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;
        }

        if (ownerWindow == null) return false;

        var dialog = new Confirm();
        var vm = new ConfirmViewModel(
            dialog:dialog,
            title:title,
            message:message,
            yesText: yes,
            noText: no,
            height: h,
            width: w
        );
        dialog.DataContext = vm;
        bool result = await dialog.ShowDialog<bool>(ownerWindow);
        return result;
    }

    public async Task<bool> ShowAskPromoteAsync(string piece)
    {
        Window? ownerWindow = null;
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ownerWindow = desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;
        }

        if (ownerWindow == null) return false;

        var dialog = new AskPromote();
        var vm = new AskPromoteViewModel(
            dialog:dialog,
            piece: piece
        );
        dialog.DataContext = vm;
        bool result = await dialog.ShowDialog<bool>(ownerWindow);
        return result;
    }

    public async Task<bool> ShowErrorAsync(
        string title,
        string message,
        string onCloseText = "close",
        int h = 200,
        int w = 200
    ) {
        Window? ownerWindow = null;
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ownerWindow = desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;
        }

        if (ownerWindow == null) return false;

        var dialog = new ShowError();
        var vm = new ShowErrorViewModel(
            dialog:dialog,
            title:title,
            message:message,
            onCloseText: onCloseText,
            height: h,
            width: w
        );
        dialog.DataContext = vm;
        bool result = await dialog.ShowDialog<bool>(ownerWindow);
        return result;
    }
}
