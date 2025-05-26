using CefSharp;
using CefSharp.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;

namespace ManualWebScraper.ViewModels;

public partial class BrowserViewModel : ObservableObject
{
    public ChromiumWebBrowser Browser { get; set; }

    private readonly DispatcherTimer _navPollTimer;
    public AppStateViewModel AppState { get; set; }
    private readonly IScriptResultsDispatcher _dispatcher = null!;

    private readonly DownloadInterceptor _interceptor = null!;

    [ObservableProperty] private string _url = "https://example.com";
    [ObservableProperty] private string _scriptKey;
    [ObservableProperty] private string _scriptText;
    [ObservableProperty] private string _scriptResult;

    public ObservableCollection<ScriptPanelViewModel> Panels { get; } = new();


    public BrowserViewModel(IScriptResultsDispatcher dispatcher, DownloadInterceptor interceptor, AppStateViewModel? state = null)
    {
        _dispatcher = dispatcher;
        AppState = state ?? new AppStateViewModel();

        Url = AppState.LastVisitedUrl ?? Url;

        // Initialise panels from state
        foreach(var model in AppState.ScriptPanels)
        {
            Panels.Add(new ScriptPanelViewModel(model, this, _dispatcher, interceptor));
        }

        _navPollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        _navPollTimer.Tick += (_, _) => RefreshNavCommands();
        _navPollTimer.Start();

        _interceptor = interceptor;

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

    [RelayCommand]
    private void NewPanel()
    {
        var model = new ScriptPanelModel();
        AppState.ScriptPanels.Add(model);
        Panels.Add(new ScriptPanelViewModel(model, this, _dispatcher, _interceptor));
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
    private async Task ExecuteScript()
    {
        if (string.IsNullOrWhiteSpace(ScriptKey) || string.IsNullOrWhiteSpace(ScriptResult))
            return;

        await _dispatcher.Dispatch(ScriptKey, ScriptResult);
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
    //[RelayCommand]
    //private async Task RunGalleryCaptureAsync()
    //{
    //    // … your existing capture logic …
    //    _interceptor.CaptureOnly = true;
    //    _interceptor.LastDownloadUrl = null;
    //    _interceptor.LastDownloadFilename = null;

    //    await Browser.EvaluateScriptAsync(/* click gallery JS */);
    //    await Task.Delay(500);

    //    // 1) Parse the existing JSON result into a dictionary
    //    var baseData = new Dictionary<string, object>();
    //    if (!string.IsNullOrWhiteSpace(ScriptResult))
    //    {
    //        try
    //        {
    //            baseData = JsonSerializer.Deserialize<Dictionary<string, object>>(ScriptResult)
    //                       ?? new Dictionary<string, object>();
    //        }
    //        catch
    //        {
    //            // If it wasn’t JSON, stick it under a key
    //            baseData = new Dictionary<string, object> { ["rawResult"] = ScriptResult };
    //        }
    //    }

    //    // 2) Add your gallery info
    //    baseData["galleryUrl"] = _interceptor.LastDownloadUrl!;
    //    baseData["galleryFilename"] = _interceptor.LastDownloadFilename!;

    //    // 3) Re-serialize back into the same ScriptResultJson
    //    ScriptResult = JsonSerializer.Serialize(
    //        baseData,
    //        new JsonSerializerOptions { WriteIndented = true });
    //}


    //[RelayCommand]
    //private async Task RunActualDownloadAsync()
    //{
    //    _interceptor.CaptureOnly = false;  // now let it actually download
    //    await Browser.EvaluateScriptAsync(/* same gallery click JS */);
    //}

}
