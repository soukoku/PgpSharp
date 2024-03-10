using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaApp.ViewModels;
using AvaloniaApp.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddGpg();
        collection.AddSingleton<MainWindowViewModel>();

        var services = collection.BuildServiceProvider();
        var vm = services.GetRequiredService<MainWindowViewModel>();

        foreach (var type in Enum.GetValues<Environment.SpecialFolder>())
        {
            System.Diagnostics.Debug.WriteLine($"{type}={Environment.GetFolderPath(type)}");
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}