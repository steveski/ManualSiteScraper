
using ManualWebScraper.Models;

namespace ManualWebScraper.ScriptHandlers;

public class SaveSceneDetailsFlow : IScriptExecutionFlow<SaveSceneDetailsRequest>
{
    public async Task Execute(SaveSceneDetailsRequest body, CancellationToken ct = default)
    {
        // Example flow step: log details
        Console.WriteLine($"Flow: Saving scene '{body.Title}' with {body.ActorIds} links");
        await Task.CompletedTask;

    }
}
