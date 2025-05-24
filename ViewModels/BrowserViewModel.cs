using CefSharp;
using CefSharp.Wpf;
using CommunityToolkit.Mvvm.Input;

namespace ManualWebScraper.ViewModels
{
    public partial class BrowserViewModel : BaseViewModel
    {
        public ChromiumWebBrowser Browser { get; set; }

        [RelayCommand(CanExecute = nameof(CanGoBack))]
        private void GoBack()
        {
            System.Diagnostics.Debug.WriteLine("Back pressed");
            Browser?.Back();
        }

        [RelayCommand(CanExecute = nameof(CanGoForward))]
        private void GoForward() => Browser?.Forward();

        [RelayCommand]
        private void Reload() => Browser?.Reload();

        public void RefreshNavCommands()
        {
            GoBackCommand.NotifyCanExecuteChanged();
            GoForwardCommand.NotifyCanExecuteChanged();
        }

        public bool CanGoBack() => Browser?.CanGoBack ?? false;
        public bool CanGoForward() => Browser?.CanGoForward ?? false;

    }
}
