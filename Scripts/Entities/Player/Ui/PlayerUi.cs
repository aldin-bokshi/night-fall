using System;
using System.Collections.Generic;
using Godot;
using NightFall.Scripts.Entities.Player.Abilities;
using NightFall.Scripts.Run;

namespace NightFall.Scripts.Entities.Player.Ui;

public partial class PlayerUi : CanvasLayer
{
    [Export] private Label? _goldLabel;
    [Export] private ProgressBar? _healthBar;
    [Export] private Label? _healthText;
    [Export] private Label? _timerLabel;
    [Export] private Label? _roomLabel;
    [Export] private Label? _modifierLabel;
    [Export] private HBoxContainer? _abilityContainer;
    [Export] private PackedScene? _abilityUiScene;

    private Player? _player;
    private AbilityManager? _abilityManager;
    private PlayerStats? _playerStats;
    private Ability[] _lastAbilities = [];

    private int _lastGold = -1;

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
        UpdateRunStats();
        UpdateModifiersDisplay();
    }

    public override void _Process(double delta)
    {
        if (_playerStats == null) return;

        UpdateHealth();
        UpdateGold();
        UpdateRunStats();
        SyncAbilityUi();
    }

    private void UpdateHealth()
    {
        if (_playerStats == null) return;

        if (_healthBar != null)
        {
            _healthBar.MaxValue = _playerStats.MaxHealth;
            _healthBar.Value = _playerStats.Health;
        }

        if (_healthText != null)
        {
            _healthText.Text = $"{_playerStats.Health:F0} / {_playerStats.MaxHealth:F0} HP";
        }
    }

    private void UpdateGold()
    {
        if (_goldLabel == null || _playerStats == null) return;

        int currentGold = _playerStats.Gold;
        _goldLabel.Text = $"◆ {currentGold} Gold";

        if (_lastGold != -1 && currentGold > _lastGold)
        {
            Tween tween = _goldLabel.CreateTween();
            tween.TweenProperty(_goldLabel, "scale", new Vector2(1.15f, 1.15f), 0.08f);
            tween.TweenProperty(_goldLabel, "scale", Vector2.One, 0.12f);
        }

        _lastGold = currentGold;
    }

    private void UpdateRunStats()
    {
        if (_timerLabel != null)
        {
            float elapsedSecs = RunTracker.GetRunTimeSeconds();
            int mins = (int)(elapsedSecs / 60);
            int secs = (int)(elapsedSecs % 60);
            _timerLabel.Text = $"TIME: {mins:00}:{secs:00}";
        }

        if (_roomLabel != null && RunTracker.Instance != null)
        {
            _roomLabel.Text = $"ROOMS: {RunTracker.Instance.RoomsCleared} | KILLS: {RunTracker.Instance.EnemiesSlain}";
        }
    }

    private void UpdateModifiersDisplay()
    {
        if (_modifierLabel == null) return;

        var config = RunSession.Current;
        if (config == null)
        {
            _modifierLabel.Text = "STANDARD DESCENT";
            return;
        }

        List<string> active = [];
        if (config.BloodMoon) active.Add("BLOOD MOON");
        if (config.GlassCannon) active.Add("GLASS CANNON");
        if (config.HardNight) active.Add("HARD NIGHT");
        if (config.Greed) active.Add("GREED");
        if (config.Fragile) active.Add("FRAGILE");

        _modifierLabel.Text = active.Count == 0
            ? "STANDARD DESCENT"
            : string.Join(" • ", active);
    }

    private void SyncAbilityUi()
    {
        if (_abilityContainer == null || _abilityUiScene == null || _abilityManager == null)
            return;

        _abilityManager.RefreshAbilities();

        IReadOnlyList<Ability> abilities = _abilityManager.Abilities;

        if (MatchesLastAbilities(abilities))
            return;

        foreach (Node child in _abilityContainer.GetChildren())
            child.QueueFree();

        for (int index = 0; index < abilities.Count; index++)
        {
            Ability ability = abilities[index];
            AbilityUi? abilityUi = _abilityUiScene.Instantiate<AbilityUi>();

            if (abilityUi == null)
                continue;

            _abilityContainer.AddChild(abilityUi);
            abilityUi.Initialize(ability, index);
        }

        _lastAbilities = [.. abilities];
    }

    private bool MatchesLastAbilities(IReadOnlyList<Ability> abilities)
    {
        if (abilities.Count != _lastAbilities.Length)
            return false;

        for (int index = 0; index < abilities.Count; index++)
        {
            if (!ReferenceEquals(abilities[index], _lastAbilities[index]))
                return false;
        }

        return true;
    }
}
