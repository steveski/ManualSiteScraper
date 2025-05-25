using ManualWebScraper.Models;
using System.Text.Json;

namespace ManualWebScraper.ScriptHandlers;

public class SaveSceneDetailsHandler : IScriptResultHandler
{
    public string Key => "save-scene-details";

    private readonly IEnumerable<IScriptExecutionFlow<SaveSceneDetailsRequest>> _flows;

    public SaveSceneDetailsHandler(IEnumerable<IScriptExecutionFlow<SaveSceneDetailsRequest>> flows)
    {
        // All flows for this scenario injected
        _flows = flows;
    }

    public async Task Handle(string jsonPayload, CancellationToken ct = default)
    {
        var body = JsonSerializer.Deserialize<SaveSceneDetailsRequest>(
            jsonPayload,
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
            )
                   ?? throw new InvalidOperationException("Invalid payload for save-scene-details");

        // Execute each flow in order
        foreach (var flow in _flows)
        {
            await flow.Execute(body, ct);
        }
    }

}
