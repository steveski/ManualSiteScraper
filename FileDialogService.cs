using Ookii.Dialogs.Wpf;

namespace ManualWebScraper;

public class FileDialogService : IFileDialogService
{
    public Task<string[]> ShowOpenFileDialog(string title,
        string initialDirectory = null,
        (string filterName, string filterPattern)[] filters = null,
        bool multiSelect = false)
    {
        var dlg = new VistaOpenFileDialog
        {
            Title = title,
            InitialDirectory = initialDirectory,
            Multiselect = multiSelect,
            Filter = BuildFilter(filters)
        };
        return Task.FromResult(dlg.ShowDialog() == true ? dlg.FileNames : Array.Empty<string>());
    }

    public Task<string?> ShowSaveFileDialog(string title,
        string initialDirectory = null,
        string defaultFileName = null,
        (string filterName, string filterPattern)[] filters = null)
    {
        var dlg = new VistaSaveFileDialog
        {
            Title = title,
            InitialDirectory = initialDirectory,
            FileName = defaultFileName,
            Filter = BuildFilter(filters),
            OverwritePrompt = true
        };
        bool? result = dlg.ShowDialog();
        return Task.FromResult(result == true ? dlg.FileName : null as string);
    }

    public Task<string?> ShowSelectFolderDialogAsync(
        string title,
        string initialDirectory = null)
    {
        var dlg = new VistaFolderBrowserDialog
        {
            Description = title,
            UseDescriptionForTitle = true,
            SelectedPath = initialDirectory
        };
        bool? result = dlg.ShowDialog();
        return Task.FromResult(result == true ? dlg.SelectedPath : null);
    }

    private static string BuildFilter((string filterName, string filterPattern)[] filters)
    {
        if (filters == null || filters.Length == 0)
            return "All files (*.*)|*.*";

        return string.Join("|",
            filters.Select(f => $"{f.filterName} ({f.filterPattern})|{f.filterPattern}"));
    }
}