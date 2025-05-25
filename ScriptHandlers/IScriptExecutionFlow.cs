namespace ManualWebScraper.ScriptHandlers;

public interface IScriptExecutionFlow<in TBody>
{
    Task Execute(TBody body, CancellationToken ct = default);
}
