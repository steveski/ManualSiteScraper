namespace ManualWebScraper;

public interface IScriptResultsDispatcher
{
    void Dispatch(string key, string jsonPayload);
}
