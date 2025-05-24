using CefSharp;
using CefSharp.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Security.Policy;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;

namespace ManualWebScraper.ViewModels;

public partial class BrowserViewModel : ObservableObject
{
    public ChromiumWebBrowser Browser { get; set; }

    private readonly DispatcherTimer _navPollTimer;

    public BrowserViewModel(AppStateViewModel? state = null)
    {
        AppState = state ?? new AppStateViewModel();
        Url = AppState.LastVisitedUrl ?? Url;

        _navPollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        _navPollTimer.Tick += (_, _) => RefreshNavCommands();
        _navPollTimer.Start();
    }


    [ObservableProperty] private string _url = "https://example.com";
    [ObservableProperty] private string _scriptKey;
    [ObservableProperty] private string _scriptText;
    [ObservableProperty] private string _scriptResult;

    public AppStateViewModel AppState { get; set; }



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

        if (!string.IsNullOrWhiteSpace(Url))
        {
            Browser.Load(Url);
        }
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

    public static AppStateViewModel? LoadAppStateFromDisk()
    {
        if (File.Exists(SavePath))
        {
            var state = JsonSerializer.Deserialize<AppStateViewModel>(File.ReadAllText(SavePath));
            return state;
        }

        return null;
    }

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
        {
            BackendDispatch(ScriptKey, ScriptResult);
        }
    }

    private void BackendDispatch(string key, string? json)
    {
        // Placeholder for backend handling based on key
    }

    private void RunWizard(WizardInput? input)
    {
        Console.WriteLine($"Running wizard with: {input?.Param1} / {input?.Param2}");
        // Add real logic or UI here
    }

}
