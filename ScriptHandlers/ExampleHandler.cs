using ManualWebScraper;
using System.Text.Json;

public class ExampleHandler : IScriptResultHandler
{
    public string Key => "example";

    public Task Handle(string jsonPayload, CancellationToken ct = default)
    {
        // Deserialize into a known type
        var data = JsonSerializer.Deserialize<ExampleData>(jsonPayload);
        Console.WriteLine($"ExampleHandler received: {data?.Value}");
        // Perform backend logic here...

        return Task.CompletedTask;
    }
}

public record ExampleData(int Value);
