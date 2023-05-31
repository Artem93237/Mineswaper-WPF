using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Models;
using Minesweeper.ViewModels;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Minesweeper
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            InitializeDependencyInjection();

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        private static void InitializeDependencyInjection()
        {
            ServiceCollection services = new();

            ConfigureServices(services);

            // https://www.reactiveui.net/docs/handbook/dependency-inversion/custom-dependency-inversion
            // https://dev.to/ingvarx/avaloniaui-dependency-injection-4aka
            services.UseMicrosoftDependencyResolver();

            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<BoardViewModel>();
            services.AddSingleton<InfoTextViewModel>();
            services.AddSingleton<ControlsViewModel>();
            services.AddSingleton<Global>();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        }
    }
}