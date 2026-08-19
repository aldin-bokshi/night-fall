using Godot;
using NightFall.Scripts.Entities.Player.Abilities;

namespace NightFall.Scripts.Entities.Player.Ui;

public partial class PlayerUi : CanvasLayer
{
    [Export] private Label? _goldLabel;
    [Export] private ProgressBar? _healthBar;
    [Export] private HBoxContainer? _abilityContainer;
    [Export] private PackedScene? _abilityUiScene;

    [Export] public int MaxAbilitiesAllowed { get; set; } = 4;

    private Player? _player;
    private PlayerStats? _playerStats;

    public override void _Ready()
    {
        _player = GetTree().GetFirstNodeInGroup("player") as Player;

        if (_player == null)
        {
            GD.PushError("PlayerUi could not find the Player.");
            return;
        }

        _playerStats = _player.Stats;

        RegisterPlayerAbilities();
        UpdateHealth();
        UpdateGold();
    }

    public override void _Process(double delta)
    {
        if (_playerStats == null) return;

        UpdateHealth();
        UpdateGold();
    }

    private void UpdateHealth()
    {
        if (_healthBar == null || _playerStats == null) return;

        _healthBar.MaxValue = _playerStats.MaxHealth;
        _healthBar.Value = _playerStats.Health;
    }

    private void UpdateGold()
    {
        if (_goldLabel == null || _playerStats == null) return;

        _goldLabel.Text = $"◆ {_playerStats.Gold}";
    }

    private void RegisterPlayerAbilities()
    {
        if (_abilityContainer == null || _abilityUiScene == null) return;

        foreach (Ability ability in _player!.Abilities)
            TryAddAbility(ability);
    }

    /// <summary>
    /// Attempts to add an ability to the player's HUD.
    /// Returns false if the maximum number of abilities has been reached.
    /// </summary>
    public bool TryAddAbility(Ability? ability)
    {
        if (ability == null)
        {
            GD.PushWarning("Cannot add ability: ability is null.");
            return false;
        }

        if (_abilityContainer == null)
        {
            GD.PushWarning(
                "Cannot add ability: AbilityContainer is not assigned."
            );

            return false;
        }

        if (_abilityContainer.GetChildCount() >= MaxAbilitiesAllowed)
        {
            string abilityName = ability.Data?.AbilityName ?? ability.GetType().Name;

            GD.PushWarning(
                $"Cannot add ability '{abilityName}': " +
                $"maximum of {MaxAbilitiesAllowed} abilities reached."
            );

            return false;
        }

        AbilityUi? abilityUi = _abilityUiScene?.Instantiate<AbilityUi>();

        if (abilityUi == null) return false;

        _abilityContainer.AddChild(abilityUi);
        abilityUi.Initialize(ability);
        return true;
    }

    /// <summary>
    /// Removes an ability from the player's HUD.
    /// </summary>
    public bool RemoveAbility(Ability? ability)
    {
        if (ability == null || _abilityContainer == null) return false;

        foreach (Node child in _abilityContainer.GetChildren())
        {
            if (child is not AbilityUi abilityUi || abilityUi.Ability != ability)
                continue;

            _abilityContainer.RemoveChild(abilityUi);
            abilityUi.QueueFree();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns true if another ability can be added.
    /// </summary>
    public bool CanAddAbility()
    {
        return _abilityContainer != null &&
               _abilityContainer.GetChildCount() < MaxAbilitiesAllowed;
    }

    /// <summary>
    /// Returns the number of abilities currently displayed.
    /// </summary>
    public int GetAbilityCount()
    {
        return _abilityContainer?.GetChildCount() ?? 0;
    }

    /// <summary>
    /// Returns true if the maximum number of abilities has been reached.
    /// </summary>
    public bool IsAbilityLimitReached()
    {
        return _abilityContainer != null &&
               _abilityContainer.GetChildCount() >= MaxAbilitiesAllowed;
    }
}