using CommunityToolkit.Mvvm.ComponentModel;
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

}
