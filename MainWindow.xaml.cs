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