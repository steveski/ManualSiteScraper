namespace ManualWebScraper.Models;

public record SaveSceneDetailsRequest(string Title, string Description, int[]? ActorIds, string[]? Links);
