using System.Collections.Generic;
using Godot;
using NightFall.Scripts.Entities.Player.Abilities;

namespace NightFall.Scripts.Entities.Player.Ui;

public partial class PlayerUi : CanvasLayer
{
    [Export] private Label? _goldLabel;
    [Export] private ProgressBar? _healthBar;
    [Export] private HBoxContainer? _abilityContainer;
    [Export] private PackedScene? _abilityUiScene;

    private Player? _player;
    private AbilityManager? _abilityManager;
    private PlayerStats? _playerStats;
    private int _lastAbilityCount = -1;

    public override void _Ready()
    {
        _player = GetTree().GetFirstNodeInGroup("player") as Player;

        if (_player == null)
        {
            GD.PushError("PlayerUi could not find the Player.");
            return;
        }

        _playerStats = _player.Stats;
        _abilityManager = _player.AbilityManager;

        SyncAbilityUi();
        UpdateHealth();
        UpdateGold();
    }

    public override void _Process(double delta)
    {
        if (_playerStats == null) return;

        UpdateHealth();
        UpdateGold();
        SyncAbilityUi();
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

    private void SyncAbilityUi()
    {
        if (_abilityContainer == null || _abilityUiScene == null || _abilityManager == null)
            return;

        _abilityManager.RefreshAbilities();

        IReadOnlyList<Ability> abilities = _abilityManager.Abilities;

        if (abilities.Count == _lastAbilityCount)
            return;

        foreach (Node child in _abilityContainer.GetChildren())
            child.QueueFree();

        foreach (Ability ability in abilities)
        {
            AbilityUi? abilityUi = _abilityUiScene.Instantiate<AbilityUi>();

            if (abilityUi == null)
                continue;

            _abilityContainer.AddChild(abilityUi);
            abilityUi.Initialize(ability);
        }

        _lastAbilityCount = abilities.Count;
    }
}
