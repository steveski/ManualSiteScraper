using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Windows;

namespace ManualWebScraper.ViewModels;

public partial class AppStateViewModel : BaseViewModel
{
    [ObservableProperty]
    private string? _lastVisitedUrl;

    [ObservableProperty]
    private double _windowX = 50;

    [ObservableProperty]
    private double _windowY = 100;

    [ObservableProperty]
    private double _windowWidth = 300;

    [ObservableProperty]
    private double _windowHeight = 200;

    private static string SavePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ManualWebScraper", "appstate.json");

    // Load persisted state or return defaults
    public static AppStateViewModel Load()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                var json = File.ReadAllText(SavePath);
                return JsonSerializer.Deserialize<AppStateViewModel>(json)
                       ?? new AppStateViewModel();
            }
            catch
            {
                // If deserialization fails, return new defaults
                return new AppStateViewModel();
            }
        }
        return new AppStateViewModel();
    }

    // Save current state to disk
    public void Save()
    {
        var dir = Path.GetDirectoryName(SavePath);
        if (!Directory.Exists(dir!))
            Directory.CreateDirectory(dir!);

        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SavePath, json);
    }

}
