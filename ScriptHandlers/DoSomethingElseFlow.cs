
using ManualWebScraper.Models;

namespace ManualWebScraper.ScriptHandlers;

public class DoSomethingElseFlow : IScriptExecutionFlow<SaveSceneDetailsRequest>
{
    public async Task Execute(SaveSceneDetailsRequest body, CancellationToken ct = default)
    {
        // Example flow step: log details
        Console.WriteLine($"Flow: Doing someting else {body}");
        await Task.CompletedTask;

    }
}
