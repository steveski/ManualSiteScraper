using CefSharp;
using CefSharp.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Security.Policy;
using System.Windows.Threading;

namespace ManualWebScraper.ViewModels
{
    public partial class BrowserViewModel : BaseViewModel
    {
        public required ChromiumWebBrowser Browser { get; set; }

        private readonly DispatcherTimer _navPollTimer;

        public BrowserViewModel()
        {
            _navPollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _navPollTimer.Tick += (_, _) => RefreshNavCommands();
            _navPollTimer.Start();
        }

        [ObservableProperty]
        private string url = "https://example.com";

        [RelayCommand]
        private void Go()
        {
            if (!string.IsNullOrWhiteSpace(Url))
            {
                Browser?.Load(Url);
            }
        }

        [RelayCommand(CanExecute = nameof(CanGoBack))]
        private void GoBack() => Browser?.Back();

        [RelayCommand(CanExecute = nameof(CanGoForward))]
        private void GoForward() => Browser?.Forward();

        [RelayCommand]
        private void Reload() => Browser?.Reload();

        public bool CanGoBack() => Browser?.CanGoBack ?? false;
        public bool CanGoForward() => Browser?.CanGoForward ?? false;

        public void RefreshNavCommands()
        {
            GoBackCommand.NotifyCanExecuteChanged();
            GoForwardCommand.NotifyCanExecuteChanged();
        }

    }
}
