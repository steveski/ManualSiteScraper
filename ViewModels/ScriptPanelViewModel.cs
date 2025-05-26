using CefSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;

namespace ManualWebScraper.ViewModels;

public partial class ScriptPanelViewModel : ObservableObject
{
    private readonly ScriptPanelModel _model;
    private readonly BrowserViewModel _browserVm;
    private readonly IScriptResultsDispatcher _dispatcher;

    private readonly DownloadInterceptor _interceptor = null!;

    public ScriptPanelViewModel(ScriptPanelModel model, BrowserViewModel browserVm, IScriptResultsDispatcher dispatcher, DownloadInterceptor interceptor)
    {
        _model = model;
        _browserVm = browserVm;
        _dispatcher = dispatcher;
        _interceptor = interceptor;

        Name = model.Name;
        ScriptText = model.ScriptText;
    }

    [ObservableProperty]
    private string name;

    partial void OnNameChanged(string value)
    {
        _model.Name = value;
    }

    [ObservableProperty]
    private string scriptText;

    partial void OnScriptTextChanged(string value)
    {
        _model.ScriptText = value;
    }

    [ObservableProperty]
    private string scriptResult;

    [RelayCommand]
    private async Task RunScript()
    {
        if (string.IsNullOrWhiteSpace(ScriptText) || _browserVm.Browser == null)
            return;

        // 1) reset & enter capture-only mode
        _interceptor.PrepareCapture();

        // 2) run the user’s JS (which must click the download link)
        var eval = await _browserVm.Browser.EvaluateScriptAsync(ScriptText);
        if (!eval.Success)
        {
            ScriptResult = JsonSerializer.Serialize(
                new { error = eval.Message },
                new JsonSerializerOptions { WriteIndented = true });
            return;
        }

        // 3) await the interceptor seeing the download (up to 3 sec)
        bool sawDownload = await _interceptor.WaitForDownloadAsync(TimeSpan.FromSeconds(3));

        // 4) merge the original JS result
        var combined = new Dictionary<string, object>();
        if (eval.Result is IDictionary<string, object> dict)
            foreach (var kv in dict) combined[kv.Key] = kv.Value!;
        else
            combined["jsResult"] = eval.Result!;

        // 5) tack on the captured URL/filename (or an error)
        if (sawDownload)
        {
            combined["galleryUrl"] = _interceptor.LastDownloadUrl!;
            combined["galleryFilename"] = _interceptor.LastDownloadFilename!;
        }
        else
        {
            combined["galleryError"] = "download link wasn’t triggered or timed out";
        }

        // 6) show it
        ScriptResult = JsonSerializer.Serialize(
            combined,
            new JsonSerializerOptions { WriteIndented = true });
    }

    [RelayCommand]
    private void ExecuteBackend()
    {
        _dispatcher.Dispatch(Name, ScriptResult);
    }
}
