using CefSharp;
using CefSharp.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Security.Policy;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;

namespace ManualWebScraper.ViewModels
{
    public partial class BrowserViewModel : BaseViewModel
    {
        public required ChromiumWebBrowser Browser { get; set; }

        private readonly DispatcherTimer _navPollTimer;

        public BrowserViewModel()
        {
            LoadAppState();

            Url = AppState.LastVisitedUrl ?? Url;

            _navPollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _navPollTimer.Tick += (_, _) => RefreshNavCommands();
            _navPollTimer.Start();
        }


        [ObservableProperty]
        private string _url = "https://example.com";

        [ObservableProperty]
        public AppStateViewModel _appState = new();

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

        public void AttachBrowser(ChromiumWebBrowser browser)
        {
            Browser = browser;

            Browser.LoadingStateChanged += (_, e) =>
            {
                if (e.IsLoading == false)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Url = Browser.Address;
                        AppState.LastVisitedUrl = Browser.Address;
                    });
                }
            };
        }

        public void UpdateWindowState(Point position, Size size)
        {
            AppState.WindowPosition = position;
            AppState.WindowSize = size;
        }

        private static string SavePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ManualWebScraper", "appstate.json");

        public void SaveAppState()
        {
            var dir = Path.GetDirectoryName(SavePath)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(SavePath,
                JsonSerializer.Serialize(
                    AppState,
                    new JsonSerializerOptions { WriteIndented = true }
                    )
                );
        }

        public void LoadAppState()
        {
            if (File.Exists(SavePath))
            {
                var state = JsonSerializer.Deserialize<AppStateViewModel>(File.ReadAllText(SavePath));
                if (state != null)
                    AppState = state;
            }
        }

    }
}
