using MudBlazor;

namespace Uploadify.Client.Application.Utilities.Constants;

public static class Themes
{
    public static readonly MudTheme DefaultTheme = new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = Colors.Shades.White,
            PrimaryContrastText = Colors.Shades.White,
            TextPrimary = Colors.Shades.White,
            
            AppbarText = Colors.Shades.White,
            AppbarBackground = Colors.Grey.Darken4,
            
            DrawerText = Colors.Shades.White,
            DrawerIcon = Colors.Shades.White
        },
        Typography = new()
        {
            H1 = new() { FontSize = "2rem", FontWeight = 700 },
            H2 = new() { FontSize = "1.8rem", FontWeight = 700 },
            H3 = new() { FontSize = "1.6rem", FontWeight = 500 },
            H4 = new() { FontSize = "1.4rem", FontWeight = 500 },
            H5 = new() { FontSize = "1.2rem", FontWeight = 400 },
            H6 = new() { FontSize = "1.2rem", FontWeight = 300 },
            Body1 = new() { FontSize = "1rem" },
            Body2 = new() { FontSize = "1.2rem" },
            Button = new() { FontSize = "1.2rem", FontWeight = 700 },
            Caption = new() { FontSize = ".8rem" },
            Subtitle1 = new() { FontSize = "1.4rem", FontWeight = 700 },
            Subtitle2 = new() { FontSize = "1.4rem", FontWeight = 700 },
            Default = new() { FontSize = "1.2rem" }
        }
    };
}
