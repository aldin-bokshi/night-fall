using Godot;

namespace NightFall.Scripts.Ui;

public partial class FloatingText : Node2D
{
    private Label _label = null!;

    public static void Spawn(Node parent, Vector2 globalPos, string text, Color color, float fontSize = 16f)
    {
        FloatingText ft = new();
        ft.GlobalPosition = globalPos + new Vector2(GD.RandRange(-8, 8), GD.RandRange(-8, 8));

        Label label = new()
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            ZIndex = 100
        };

        label.AddThemeColorOverride("font_color", color);
        label.AddThemeColorOverride("font_outline_color", Colors.Black);
        label.AddThemeConstantOverride("outline_size", 4);
        label.AddThemeFontSizeOverride("font_size", (int)fontSize);

        ft.AddChild(label);
        ft._label = label;
        parent.AddChild(ft);

        Tween tween = ft.CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(ft, "global_position:y", ft.GlobalPosition.Y - 35f, 0.6f)
             .SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(label, "modulate:a", 0f, 0.6f)
             .SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
        tween.TweenProperty(ft, "scale", new Vector2(1.2f, 1.2f), 0.15f);

        tween.Finished += ft.QueueFree;
    }
}
