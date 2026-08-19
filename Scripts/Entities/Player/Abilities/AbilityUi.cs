using Godot;

namespace NightFall.Scripts.Entities.Player.Abilities;

public partial class AbilityUi : Panel
{
    private Ability? _ability;

    private Label? _nameLabel;
    private Label? _inputLabel;
    private Label? _cooldownText;
    private TextureProgressBar? _cooldown;
    private TextureRect? _icon;

    public Ability? Ability => _ability;

    public void Initialize(Ability ability)
    {
        _ability = ability;
        UpdateDisplay();
    }

    public override void _Ready()
    {
        _nameLabel = GetNodeOrNull<Label>(
            "MarginContainer/VBoxContainer/Header/Name"
        );

        _inputLabel = GetNodeOrNull<Label>(
            "MarginContainer/VBoxContainer/Header/Input"
        );

        _cooldown = GetNodeOrNull<TextureProgressBar>(
            "MarginContainer/VBoxContainer/Cooldown"
        );

        _cooldownText = GetNodeOrNull<Label>(
            "MarginContainer/VBoxContainer/CooldownText"
        );

        _icon = GetNodeOrNull<TextureRect>(
            "MarginContainer/VBoxContainer/Icon"
        );

        if (_nameLabel == null) GD.PushError($"AbilityUi '{Name}' could not find its Name label.");
        if (_inputLabel == null) GD.PushError($"AbilityUi '{Name}' could not find its Input label.");
        if (_cooldown == null) GD.PushError($"AbilityUi '{Name}' could not find its Cooldown node.");

        UpdateDisplay();
    }

    public override void _Process(double delta)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_ability == null) return;

        AbilityData? data = _ability.Data;

        if (data == null) return;

        if (_nameLabel != null) _nameLabel.Text = data.AbilityName;
        if (_inputLabel != null) _inputLabel.Text = data.InputDisplay;

        if (_icon != null)
        {
            _icon.Texture = data.Icon;
            _icon.Visible = data.Icon != null;
        }

        if (_cooldown == null) return;

        _cooldown.Value = _ability.CooldownProgress;

        if (_cooldownText != null)
        {
            _cooldownText.Text = _ability.IsReady
                ? "READY"
                : $"{_ability.CooldownRemaining:0.0}s";
        }
    }
}