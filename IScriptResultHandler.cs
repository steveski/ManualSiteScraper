namespace ManualWebScraper;

public interface IScriptResultHandler
{
    string Key { get; }
    Task Handle(string jsonPayload, CancellationToken ct = default);
}
