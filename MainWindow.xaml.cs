using System.Windows;
using CefSharp;
using ManualWebScraper.ViewModels;

namespace ManualWebScraper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly BrowserViewModel vm;

    public MainWindow()
    {
        InitializeComponent();

        vm = new BrowserViewModel { Browser = Browser };
        DataContext = vm;

        vm.AttachBrowser(Browser);

        LocationChanged += (_, _) => vm.UpdateWindowState(new Point(Left, Top), vm.AppState.WindowSize);
        SizeChanged += (_, _) => vm.UpdateWindowState(vm.AppState.WindowPosition, new Size(ActualWidth, ActualHeight));
        Closing += (_, _) => vm.SaveAppState();

    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        // Manually apply saved window size and position
        if (vm.AppState.WindowSize.Width > 0 && vm.AppState.WindowSize.Height > 0)
        {
            Width = vm.AppState.WindowSize.Width;
            Height = vm.AppState.WindowSize.Height;
        }

        if (vm.AppState.WindowPosition.X >= 0 && vm.AppState.WindowPosition.Y >= 0)
        {
            Left = vm.AppState.WindowPosition.X;
            Top = vm.AppState.WindowPosition.Y;
        }
    }

    private async void ScrapeButton_Click(object sender, RoutedEventArgs e)
    {
        var script = @"
                (function() {
                    const links = Array.from(document.querySelectorAll('a'));
                    return links.map(link => ({ text: link.innerText.trim(), href: link.href }));
                })();"
        ;

        var result = await Browser.EvaluateScriptAsync(script);
        if (result.Success && result.Result is List<object> data)
        {
            string output = "";
            foreach (var item in data)
            {
                if (item is IDictionary<string, object> link)
                {
                    string text = link.TryGetValue("text", out var t) ? t?.ToString() : "";
                    string href = link.TryGetValue("href", out var h) ? h?.ToString() : "";
                    output += $"{text} => {href}";
                    }
            }

            MessageBox.Show(output, "Scraped Links", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Failed to evaluate script.");
        }
    }
}