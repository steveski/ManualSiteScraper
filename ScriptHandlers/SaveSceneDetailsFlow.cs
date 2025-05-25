
using ManualWebScraper.Models;
using ManualWebScraper.ViewModels;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace ManualWebScraper.ScriptHandlers;

public class SaveSceneDetailsFlow(IFileDialogService fileDialog, AppStateViewModel appState) : IScriptExecutionFlow<SaveSceneDetailsRequest>
{
    private readonly IFileDialogService _fileDialog = fileDialog;

    public async Task Execute(SaveSceneDetailsRequest body, CancellationToken ct = default)
    {
        var filePath = await _fileDialog
            .ShowSaveFileDialog(
                title: "Enter a scene file to save",
                initialDirectory: appState.LastSceneFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                defaultFileName: "sceneInfo.json",
                filters: [("JSON files", "*.json"), ("All files", "*")]);

        if (filePath is null || filePath.Length == 0)
        {
            return;
        }

        await File.WriteAllTextAsync(
            filePath,
            JsonSerializer.Serialize(body, new JsonSerializerOptions() {  WriteIndented = true}),
            ct);
        
        appState.LastSceneFolder = filePath;

    }
}
