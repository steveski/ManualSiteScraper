using System.Windows;
using CefSharp;
using ManualWebScraper.ViewModels;

namespace ManualWebScraper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly BrowserViewModel _vm;

    public MainWindow(BrowserViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;
        vm.AttachBrowser(Browser);

    }

}