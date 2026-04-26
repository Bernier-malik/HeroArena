using Avalonia;
using HeroArena.Data;
using System;

namespace HeroArena;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        DatabaseInitializer.Initialize();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
