using MudBlazor;
using MudBlazor.Utilities;

namespace Uploadify.Client.Application.Utilities.Constants;

public static class Themes
{
    public static MudTheme DefaultTheme = new()
    {
        PaletteDark = new PaletteDark
        {
            Background = new MudColor("121212FF"),
            BackgroundGrey =  new MudColor("424242FF"),
            AppbarBackground = new MudColor("121212FF"),
            AppbarText = new MudColor("FFFFFFDE"),
            DrawerBackground = new MudColor("121212FF"),
            DrawerText = new MudColor("FFFFFFDE"),
            Black = new MudColor("121212FF"),
            Primary = new MudColor("FFFFFFDE"),
            PrimaryContrastText = new MudColor("FFFFFFDE"),
            Secondary = new MudColor("FFFFFFDE"),
            SecondaryContrastText = new MudColor("FFFFFFDE"),
            Surface = new MudColor("121212FF"),
            Error = new MudColor("CF6679FF"),
            ErrorContrastText = new MudColor("FFFFFFDE"),
            TextPrimary = new MudColor("FFFFFFDE"),
            TextSecondary = new MudColor("FFFFFF99"),
            TextDisabled = new MudColor("FFFFFF61"),
            Dark = new MudColor("121212FF"),
            DarkContrastText = new MudColor("FFFFFFDE"),
            DrawerIcon = new MudColor("FFFFFFDE")
        },
        Typography = new Typography()
        {
            H1 = new H1 { FontSize = "22px", FontWeight = 700 },
            H2 = new H2 { FontSize = "20px", FontWeight = 700 },
            H3 = new H3 { FontSize = "18px", FontWeight = 500 },
            H4 = new H4 { FontSize = "16px", FontWeight = 500 },
            H5 = new H5 { FontSize = "14px", FontWeight = 400 },
            H6 = new H6 { FontSize = "14px", FontWeight = 300 },
            Body1 = new Body1 { FontSize = "14px" },
            Body2 = new Body2 { FontSize = "14px" },
            Button= new Button { FontSize = "14px", FontWeight = 700 },
            Caption = new Caption { FontSize = "12px" },
            Subtitle1 = new Subtitle1 { FontSize = "16px", FontWeight = 700 },
            Subtitle2 = new Subtitle2 { FontSize = "16px", FontWeight = 700 },
            Default = new Default { FontSize = "14px" }
        }
    };
}
