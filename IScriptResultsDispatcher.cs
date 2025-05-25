namespace ManualWebScraper;

public interface IScriptResultsDispatcher
{
    Task Dispatch(string key, string jsonPayload, CancellationToken ct = default);
}
