using Godot;

namespace NightFall.Scripts.Ui;

public partial class DungeonButtons : Button
{
    [Export] private float _hoverBrightness = 1.08f;
    [Export] private float _pressedBrightness = 1.14f;
    [Export] private float _animationDuration = 0.12f;

    private Color _normalModulate = Colors.White;

    public override void _Ready()
    {
        MouseDefaultCursorShape = CursorShape.PointingHand;

        PivotOffset = Size / 2f;

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        ButtonDown += OnButtonDown;
        ButtonUp += OnButtonUp;

        FocusEntered += OnFocusEntered;
        FocusExited += OnFocusExited;
    }

    private void OnMouseEntered()
    {
        if (!Disabled)
            AnimateBrightness(_hoverBrightness);
    }

    private void OnMouseExited()
    {
        if (!Disabled)
            AnimateBrightness(1f);
    }

    private void OnFocusEntered()
    {
        if (!Disabled)
            AnimateBrightness(_hoverBrightness);
    }

    private void OnFocusExited()
    {
        if (!Disabled)
            AnimateBrightness(1f);
    }

    private void OnButtonDown()
    {
        if (!Disabled)
            AnimateBrightness(_pressedBrightness, 0.06f);
    }

    private void OnButtonUp()
    {
        if (Disabled)
            return;

        AnimateBrightness(
            IsHovered() || HasFocus()
                ? _hoverBrightness
                : 1f
        );
    }

    private void AnimateBrightness(
        float brightness,
        float duration = -1f)
    {
        float actualDuration =
            duration < 0f
                ? _animationDuration
                : duration;

        CreateTween()
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out)
            .TweenProperty(
                this,
                "modulate",
                new Color(
                    brightness,
                    brightness,
                    brightness,
                    1f
                ),
                actualDuration
            );
    }
}