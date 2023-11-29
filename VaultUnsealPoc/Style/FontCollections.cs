using System;
using Avalonia.Media.Fonts;

namespace VaultUnsealPoc.Style;

public static class DefaultFontSettings
{
    public static string DefaultFontFamily = "fonts:DMSans#DM Sans";
    public static Uri Key { get; set; } = new("fonts:DMSans", UriKind.Absolute);

    public static Uri Source { get; set; } =
        new("avares://VaultUnsealPoc/Assets/Fonts/DMSans#DM Sans", UriKind.Absolute);
}

public sealed class DMSansFontCollection : EmbeddedFontCollection
{
    public DMSansFontCollection() : base(
        new Uri("fonts:DMSans", UriKind.Absolute),
        new Uri("avares://VaultUnsealPoc/Assets/Fonts/DMSans#DM Sans", UriKind.Absolute))
    {
    }
}

public sealed class SyneFontCollection : EmbeddedFontCollection
{
    public SyneFontCollection() : base(
        new Uri("fonts:Syne", UriKind.Absolute),
        new Uri("avares://VaultUnsealPoc/Assets/Fonts/Syne#Syne", UriKind.Absolute))
    {
    }
}