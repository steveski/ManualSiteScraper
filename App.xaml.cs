using CefSharp;
using System.Windows;
using CefSharp.Wpf;
using System.IO;
using Microsoft.Extensions.Hosting;
using ManualWebScraper.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ManualWebScraper.Models;
using ManualWebScraper.ScriptHandlers;

namespace ManualWebScraper;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost _host;

    protected override void OnStartup(StartupEventArgs e)
    {
        if (!Cef.IsInitialized.HasValue || !Cef.IsInitialized.Value)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var settings = new CefSettings
            {
                CachePath = Path.Combine(
                    folder,
                    "ManualWebScraperWPF",
                    "cef_cache")
            };
            Cef.Initialize(settings);
        }

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                // Load & register AppState
                var appState = AppStateViewModel.Load();
                services.AddSingleton(appState);

                // Register flows
                services.AddTransient<IScriptExecutionFlow<SaveSceneDetailsRequest>, SaveSceneDetailsFlow>();
                services.AddTransient<IScriptExecutionFlow<SaveSceneDetailsRequest>, DoSomethingElseFlow>();

                // Services
                services.AddSingleton<IScriptResultsDispatcher, ScriptResultsDispatcher>();

                services.AddSingleton<IScriptResultHandler, SaveSceneDetailsHandler>();
                services.AddSingleton<IScriptResultHandler, ExampleHandler>();

                // ViewModels
                services.AddTransient<BrowserViewModel>();

                // MainWindow
                services.AddTransient<MainWindow>();
            })
            .Build();

        var main = _host.Services.GetRequiredService<MainWindow>();
        main.Show();

        base.OnStartup(e);
    }
    protected override async void OnExit(ExitEventArgs e)
    {
        // Persist state on exit
        var appState = _host?.Services.GetRequiredService<AppStateViewModel>();
        appState?.Save();

        if (_host is not null)
            await _host.StopAsync(TimeSpan.FromSeconds(5));

        base.OnExit(e);
    }

}

