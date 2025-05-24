using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace ManualWebScraper.ViewModels;

public partial class AppStateViewModel : BaseViewModel
{
    [ObservableProperty]
    private string? _lastVisitedUrl;

    [ObservableProperty]
    private Point _windowPosition;

    [ObservableProperty]
    private Size _windowSize;

}
