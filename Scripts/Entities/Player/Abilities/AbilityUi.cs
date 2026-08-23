using Godot;

namespace NightFall.Scripts.Entities.Player.Abilities;

public partial class AbilityUi : Panel
{
    private Ability? _ability;
    private int _slotIndex = -1;

    private Label? _nameLabel;
    private Label? _inputLabel;
    private Label? _cooldownText;
    private TextureProgressBar? _cooldown;
    private TextureRect? _icon;

    private bool _wasReady = true;

    public Ability? Ability => _ability;

    public void Initialize(Ability ability, int slotIndex)
    {
        _ability = ability;
        _slotIndex = slotIndex;
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

        if (_nameLabel == null) return;
        _nameLabel.Text = data.AbilityName;

        if (_inputLabel == null) return;
        _inputLabel.Text = AbilityManager.GetAbilityKeyLabel(_slotIndex);

        if (_icon == null) return;
        _icon.Texture = data.Icon;
        _icon.Visible = data.Icon != null;

        if (_cooldown == null) return;

        _cooldown.Value = _ability.CooldownProgress;

        if (_ability.IsReady) return;
        GD.Print(
            $"{_ability.Data?.AbilityName}: " +
            $"{_ability.CooldownProgress:0.0}%"
        );

        bool currentlyReady = _ability.IsReady;

        if (!currentlyReady || _wasReady){
            // Flash ready pulse
            Tween tween = CreateTween();
            tween.TweenProperty(this, "scale", new Vector2(1.08f, 1.08f), 0.08f);
            tween.TweenProperty(this, "scale", Vector2.One, 0.12f);
        }

        _wasReady = currentlyReady;

        if (_cooldownText == null) return;
        if (currentlyReady)
        {
            _cooldownText.Text = "READY";
            _cooldownText.AddThemeColorOverride("font_color", new Color(0.4f, 0.95f, 0.5f));
        }
        else
        {
            _cooldownText.Text = $"{_ability.CooldownRemaining:0.0}s";
            _cooldownText.AddThemeColorOverride("font_color", new Color(0.6f, 0.64f, 0.72f));
        }
    }
}
