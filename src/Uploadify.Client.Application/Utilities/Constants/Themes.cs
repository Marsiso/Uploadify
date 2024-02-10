using MudBlazor;

namespace Uploadify.Client.Application.Utilities.Constants;

public static class Themes
{
    public static readonly MudTheme DefaultTheme = new()
    {
        Palette = new PaletteLight()
        {
            Primary = Colors.Shades.Black,
            PrimaryContrastText = Colors.Shades.White,
            Secondary = Colors.Grey.Lighten1,
            SecondaryContrastText = Colors.Shades.White,
            Tertiary = Colors.Pink.Darken4,
            TertiaryContrastText = Colors.Shades.White,

            Error = Colors.Red.Accent4,
            ErrorContrastText = Colors.Shades.White,

            Dark = Colors.Shades.Black,
            DarkContrastText = Colors.Shades.White,

            TextPrimary = Colors.Shades.Black,
            TextSecondary = Colors.Grey.Lighten1,
            TextDisabled = Colors.Grey.Lighten1,

            Background = Colors.Shades.White,
            BackgroundGrey = Colors.Grey.Lighten1,

            AppbarBackground = Colors.Shades.White,
            AppbarText = Colors.Shades.Black,

            DrawerBackground = Colors.Shades.White,
            DrawerText = Colors.Shades.Black,
            DrawerIcon = Colors.Shades.Black,

            ActionDefault = Colors.Shades.Black,
        },
        Typography = new Typography
        {
            H1 = new H1 { FontSize = "22px", FontWeight = 700 },
            H2 = new H2 { FontSize = "20px", FontWeight = 700 },
            H3 = new H3 { FontSize = "18px", FontWeight = 500 },
            H4 = new H4 { FontSize = "16px", FontWeight = 500 },
            H5 = new H5 { FontSize = "14px", FontWeight = 400 },
            H6 = new H6 { FontSize = "14px", FontWeight = 300 },
            Body1 = new Body1 { FontSize = "14px" },
            Body2 = new Body2 { FontSize = "14px" },
            Button = new Button { FontSize = "14px", FontWeight = 700 },
            Caption = new Caption { FontSize = "12px" },
            Subtitle1 = new Subtitle1 { FontSize = "16px", FontWeight = 700 },
            Subtitle2 = new Subtitle2 { FontSize = "16px", FontWeight = 700 },
            Default = new Default { FontSize = "14px" }
        }
    };
}
