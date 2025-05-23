using CefSharp;
using System.Configuration;
using System.Data;
using System.Windows;
using CefSharp.Wpf;

namespace ManualWebScraper;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        if (Cef.IsInitialized.HasValue && Cef.IsInitialized.Value == false)
        {
            var settings = new CefSettings();
            Cef.Initialize(settings);
        }

        base.OnStartup(e);
    }
}

