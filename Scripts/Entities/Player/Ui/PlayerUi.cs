using Godot;

namespace NightFall.Scripts.Entities.Player.Ui;

public partial class PlayerUi : CanvasLayer
{
    [Export] private Label? _goldLabel;
    [Export] private ProgressBar? _healthBar;
    [Export] private Control? _wheel;

    private TextureProgressBar? _dashCooldownBar;

    private Player? _player;
    private PlayerStats? _playerStats;
    private PlayerDash? _playerDash;

    public override void _Ready()
    {
        _dashCooldownBar = GetNode<TextureProgressBar>(
            "Panel/MarginContainer/VBoxContainer/DashCooldown"
        );

        _player = GetTree().GetFirstNodeInGroup("player") as Player;

        if (_player == null)
        {
            GD.PushError("PlayerUi could not find the Player.");
            return;
        }

        // Access the player's private fields through
        // the read-only properties exposed by Player.
        _playerStats = _player.Stats;
        _playerDash = _player.Dash;

        UpdateHealth();
        UpdateGold();
        UpdateDashCooldown();
    }

    public override void _Process(double delta)
    {
        if (_playerStats == null || _playerDash == null)
            return;

        UpdateHealth();
        UpdateGold();
        UpdateDashCooldown();
    }

    private void UpdateHealth()
    {
        if (_healthBar == null || _playerStats == null)
            return;

        _healthBar.MaxValue = _playerStats.MaxHealth;
        _healthBar.Value = _playerStats.Health;
    }

    private void UpdateGold()
    {
        if (_goldLabel == null || _playerStats == null)
            return;

        _goldLabel.Text = $"◆ {_playerStats.Gold}";
    }

    private void UpdateDashCooldown()
    {
        if (_dashCooldownBar == null || _playerDash == null)
            return;

        float duration = _playerDash.CooldownDuration;

        if (duration <= 0f)
        {
            _dashCooldownBar.Value = 100f;
            return;
        }

        float progress =
            1f - (_playerDash.CooldownRemaining / duration);

        _dashCooldownBar.Value = Mathf.Clamp(progress * 100f, 0f, 100f);
    }
}