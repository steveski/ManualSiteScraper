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

    public ScriptPanelViewModel(ScriptPanelModel model, BrowserViewModel browserVm, IScriptResultsDispatcher dispatcher)
    {
        _model = model;
        _browserVm = browserVm;
        _dispatcher = dispatcher;

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
        var jsResult = await _browserVm.Browser.EvaluateScriptAsync(ScriptText);
        if (jsResult.Success)
            ScriptResult = JsonSerializer.Serialize(jsResult.Result, new JsonSerializerOptions { WriteIndented = true });
        else
            ScriptResult = $"Error: {jsResult.Message}";
    }

    [RelayCommand]
    private void ExecuteBackend()
    {
        _dispatcher.Dispatch(Name, ScriptResult);
    }
}
