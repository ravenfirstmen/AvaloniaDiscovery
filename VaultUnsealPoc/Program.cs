using System;
using Avalonia;
using VaultUnsealPoc.Style;

namespace VaultUnsealPoc;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        var app = AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            // .With(new FontManagerOptions()
            // {
            //     DefaultFamilyName = DefaultFontSettings.DefaultFontFamily
            //     //DefaultFamilyName = "fonts:DM Sans"
            // })
            .ConfigureFonts(manager =>
            {
                manager.AddFontCollection(new DMSansFontCollection());
                manager.AddFontCollection(new SyneFontCollection());
            })
            .LogToTrace();

        return app;
    }
}