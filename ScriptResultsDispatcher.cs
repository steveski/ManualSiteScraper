namespace ManualWebScraper;

public class ScriptResultsDispatcher : IScriptResultsDispatcher
{
    private readonly IEnumerable<IScriptResultHandler> _handlers;

    public ScriptResultsDispatcher(IEnumerable<IScriptResultHandler> handlers)
    {
        _handlers = handlers;
    }

    public Task Dispatch(string key, string jsonPayload, CancellationToken ct = default)
    {
        var handler = _handlers.FirstOrDefault(h => h.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        if (handler == null)
            throw new InvalidOperationException($"No handler registered for script key '{key}'");

        handler.Handle(jsonPayload);

        return Task.CompletedTask;
    }
}
