namespace ManualWebScraper.Models;

public record SaveSceneDetailsRequest(string Title, string Description, int[]? Performers, string[]? Categories, string Uri, string SceneDate, string? Channel, Link[]? Links);

public record Link(string Href, string? Label, string? Details);
