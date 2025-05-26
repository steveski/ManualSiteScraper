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

        // 1) prepare interceptor to only capture, not actually download
        _interceptor.CaptureOnly = true;
        _interceptor.LastDownloadUrl = null;
        _interceptor.LastDownloadFilename = null;

        // 2) run the user's JS
        var eval = await _browserVm.Browser.EvaluateScriptAsync(ScriptText);
        if (!eval.Success)
        {
            // JS threw or couldn’t run
            ScriptResult = JsonSerializer.Serialize(
                new { error = eval.Message },
                new JsonSerializerOptions { WriteIndented = true });
            return;
        }

        // 3) give the interceptor a moment to fire OnBeforeDownload
        await Task.Delay(500);

        // 4) take the raw JS result (usually a Dictionary<string,object>)
        //    and merge in galleryUrl/galleryFilename
        var combined = new Dictionary<string, object>();

        if (eval.Result is IDictionary<string, object> dict)
        {
            foreach (var kv in dict)
                combined[kv.Key] = kv.Value!;
        }
        else
        {
            // in case the JS returned something else (string/number/array)
            combined["jsResult"] = eval.Result!;
        }

        //combined["galleryUrl"] = _interceptor.LastDownloadUrl;
        //combined["galleryFilename"] = _interceptor.LastDownloadFilename;

        // 5) re-serialize and display in the same ScriptResult box
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
