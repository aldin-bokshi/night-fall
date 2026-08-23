using Godot;

namespace NightFall.Scripts.Shop;

public static class ShopItemUi
{
    public static Label CreateStatLabel(string text)
    {
        Label label = new()
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            CustomMinimumSize = new Vector2(210, 17)
        };

        label.AddThemeFontSizeOverride("font_size", 12);
        label.AddThemeColorOverride(
            "font_color",
            new Color(0.65f, 0.62f, 0.71f));

        return label;
    }
}