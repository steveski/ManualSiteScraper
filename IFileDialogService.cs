namespace ManualWebScraper;

public interface IFileDialogService
{
    /// <summary>
    /// Shows an Open File dialog; returns selected file paths (or empty array if cancelled).
    /// </summary>
    Task<string[]> ShowOpenFileDialog(string title,
                                           string? initialDirectory = null,
                                           (string filterName, string? filterPattern)[] filters = null,
                                           bool multiSelect = false, CancellationToken ct = default);

    Task<string?> ShowSaveFileDialog(string title,
        string initialDirectory = null,
        string defaultFileName = null,
        (string filterName, string filterPattern)[] filters = null, CancellationToken ct = default);

}
