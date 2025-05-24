using ManualWebScraper;
using System.Text.Json;

public class ExampleHandler : IScriptResultHandler
{
    public string Key => "example";

    public void Handle(string jsonPayload)
    {
        // Deserialize into a known type
        var data = JsonSerializer.Deserialize<ExampleData>(jsonPayload);
        Console.WriteLine($"ExampleHandler received: {data?.Value}");
        // Perform backend logic here...
    }
}

public record ExampleData(int Value);
