
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
        var path = await _fileDialog
            .ShowSelectFolderDialogAsync(
                title: "Enter a scene file to save",
                initialDirectory: appState.LastSceneFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        var sceneFolder = Path.Combine(path, body.SceneDate);
        if (!Directory.Exists(sceneFolder))
        {
            Directory.CreateDirectory(sceneFolder);
        }

        await File.WriteAllTextAsync(
            Path.Combine(sceneFolder, "sceneInfo.json"),
            JsonSerializer.Serialize(body, new JsonSerializerOptions() {  WriteIndented = true})
            );
        
        appState.LastSceneFolder = path;

    }
}
