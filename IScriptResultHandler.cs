namespace ManualWebScraper;

public interface IScriptResultHandler
{
    string Key { get; }
    void Handle(string jsonPayload);
}
