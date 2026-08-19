using Godot;

namespace NightFall.Scripts.Entities.Player.Ui;

// max limit of 4 abilities for now

public partial class Ability : Panel
{
    [Export] public Texture2D Icon { get; set; } = null!;
    [Export] public string AbilityName { get; set; } = "ABILITY";
    [Export] public float CooldownDuration { get; set; } = 1f;
    [Export] public string InputDisplay { get; set; } = "KEY";

    private Label _nameLabel = null!;
    private Label _inputLabel = null!;
    private TextureProgressBar _cooldown = null!;

    private float _cooldownRemaining;

    public bool IsReady => _cooldownRemaining <= 0f;

    public override void _Ready()
    {
        _nameLabel = GetNode<Label>(
            "MarginContainer/VBoxContainer/Header/Name"
        );

        _inputLabel = GetNode<Label>(
            "MarginContainer/VBoxContainer/Header/Input"
        );

        _cooldown = GetNode<TextureProgressBar>(
            "MarginContainer/VBoxContainer/Cooldown"
        );

        _nameLabel.Text = AbilityName;
        _inputLabel.Text = InputDisplay;

        _cooldown.TextureUnder = Icon;
        _cooldown.TextureProgress = Icon;

        _cooldownRemaining = 0f;
        UpdateCooldownVisual();
    }

    public override void _Process(double delta)
    {
        if (_cooldownRemaining <= 0f) return;

        _cooldownRemaining -= (float)delta;

        if (_cooldownRemaining < 0f) _cooldownRemaining = 0f;

        UpdateCooldownVisual();
    }

    public void StartCooldown()
    {
        _cooldownRemaining = CooldownDuration;
        UpdateCooldownVisual();
    }

    public void StartCooldown(float duration)
    {
        _cooldownRemaining = Mathf.Max(0f, duration);
        UpdateCooldownVisual();
    }

    private void UpdateCooldownVisual()
    {
        if (CooldownDuration <= 0f)
        {
            _cooldown.Value = 100f;
            return;
        }

        float progress = 1f - (_cooldownRemaining / CooldownDuration);

        _cooldown.Value = progress * 100f;
    }
}