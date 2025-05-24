using CefSharp;
using CefSharp.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;

namespace ManualWebScraper.ViewModels;

public partial class BrowserViewModel : ObservableObject
{
    public ChromiumWebBrowser Browser { get; set; }

    private readonly DispatcherTimer _navPollTimer;
    public AppStateViewModel AppState { get; set; }
    private readonly IScriptResultsDispatcher _dispatcher;

    [ObservableProperty] private string _url = "https://example.com";
    [ObservableProperty] private string _scriptKey;
    [ObservableProperty] private string _scriptText;
    [ObservableProperty] private string _scriptResult;

    public BrowserViewModel(IScriptResultsDispatcher dispatcher, AppStateViewModel? state = null)
    {
        _dispatcher = dispatcher;
        AppState = state ?? new AppStateViewModel();

        Url = AppState.LastVisitedUrl ?? Url;

        _navPollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        _navPollTimer.Tick += (_, _) => RefreshNavCommands();
        _navPollTimer.Start();
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

        if (!string.IsNullOrWhiteSpace(Url))
        {
            Browser.Load(Url);
        }
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void GoBack() => Browser?.Back();
    public bool CanGoBack() => Browser?.CanGoBack ?? false;

    [RelayCommand(CanExecute = nameof(CanGoForward))]
    private void GoForward() => Browser?.Forward();
    public bool CanGoForward() => Browser?.CanGoForward ?? false;

    [RelayCommand]
    private void Reload() => Browser?.Reload();

    [RelayCommand]
    private async Task RunScript()
    {
        if (string.IsNullOrWhiteSpace(ScriptText) || Browser == null)
            return;

        var result = await Browser.EvaluateScriptAsync(ScriptText);
        if (result.Success)
        {
            // Serialize the JS result to JSON for display
            ScriptResult = JsonSerializer.Serialize(result.Result, new JsonSerializerOptions { WriteIndented = true });
        }
        else
        {
            ScriptResult = $"Error: {result.Message}";
        }
    }

    [RelayCommand]
    private void ExecuteScript()
    {
        if (!string.IsNullOrWhiteSpace(ScriptKey))
            _dispatcher.Dispatch(ScriptKey, ScriptResult);
    }
    public void RefreshNavCommands()
    {
        GoBackCommand.NotifyCanExecuteChanged();
        GoForwardCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void Go()
    {
        if (!string.IsNullOrWhiteSpace(Url))
        {
            Browser?.Load(Url);
        }
    }

}
