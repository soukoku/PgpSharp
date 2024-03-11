using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            var collection = new ServiceCollection();
            collection.AddGpg();
            collection.AddSingleton<MainWindowViewModel>();
            collection.AddSingleton<IStorageProvider>(svc => desktop.MainWindow.StorageProvider);

            var services = collection.BuildServiceProvider();
            var vm = services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow.DataContext = vm;
        }

        base.OnFrameworkInitializationCompleted();
    }
}