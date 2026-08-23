using Godot;
using NightFall.Scripts.Core;

namespace NightFall.Scripts.Ui;

public static class UiExtensions
{
    public static void AttachJuice(this Button button)
    {
        if (!GodotObject.IsInstanceValid(button)) return;

        button.PivotOffset = button.Size / 2f;
        button.Resized += () => button.PivotOffset = button.Size / 2f;

        button.MouseEntered += () =>
        {
            AudioSynthManager.PlayUiHover();
            Tween tween = button.CreateTween();
            tween.SetParallel(true);
            tween.TweenProperty(button, "scale", new Vector2(1.04f, 1.04f), 0.12f)
                 .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        };

        button.MouseExited += () =>
        {
            Tween tween = button.CreateTween();
            tween.SetParallel(true);
            tween.TweenProperty(button, "scale", Vector2.One, 0.12f)
                 .SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        };

        button.Pressed += () =>
        {
            AudioSynthManager.PlayUiClick();
            Tween tween = button.CreateTween();
            tween.TweenProperty(button, "scale", new Vector2(0.96f, 0.96f), 0.05f);
            tween.TweenProperty(button, "scale", new Vector2(1.04f, 1.04f), 0.08f);
        };
    }

    public static void AttachJuiceToTree(this Node rootNode)
    {
        AudioSynthManager.EnsureInstance(rootNode);

        if (rootNode is Button btn) btn.AttachJuice();

        foreach (Node child in rootNode.GetChildren()) child.AttachJuiceToTree();
    }
}
