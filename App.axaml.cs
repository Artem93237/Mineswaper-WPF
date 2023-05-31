using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Minesweeper.ViewModels;
using Minesweeper.Views;
using Splat;

namespace Minesweeper
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow
                {
                    // Locator(.Current) == BAD !!!
                    DataContext = Locator.Current.GetService<MainWindowViewModel>() ??
                                  throw new Exception("boi you didn't register it")
                };


            base.OnFrameworkInitializationCompleted();
        }
    }
}