using Godot;

namespace NightFall.Scripts.UI.HUD;

public partial class Hud : CanvasLayer
{
    [Export] private Label? _goldLabel;
    [Export] private ProgressBar? _healthBar;
    [Export] private Control? _wheel;

    [Export] private float _dashCooldownTime = 2.0f;

    // TESTING ONLY
    [Export] private bool _testDashCooldown;

    private TextureProgressBar? _dashCooldown;
    private Tween? _dashCooldownTween;

    public override void _Ready()
    {
        _dashCooldown = GetNode<TextureProgressBar>(
            "Panel/MarginContainer/VBoxContainer/DashCooldown"
        );

        _dashCooldown.Value = 100.0f;

        // TESTING ONLY:
        // Automatically trigger the cooldown when the HUD starts.
        if (_testDashCooldown)
            StartDashCooldown();
    }

    private void StartDashCooldown()
    {
        if (_dashCooldown == null)
            return;

        _dashCooldownTween?.Kill();

        _dashCooldownTween = CreateTween();

        // Quickly and smoothly drain to 0%.
        _dashCooldownTween.TweenProperty(
                _dashCooldown,
                "value",
                0.0f,
                0.15f
            )
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);

        // Slowly recharge to 100%.
        _dashCooldownTween.TweenProperty(
                _dashCooldown,
                "value",
                100.0f,
                _dashCooldownTime
            )
            .SetTrans(Tween.TransitionType.Linear);

        // TESTING ONLY:
        // Repeat the cooldown forever.
        if (_testDashCooldown)
            _dashCooldownTween.SetLoops();
    }
}