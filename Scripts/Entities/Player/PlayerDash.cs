using System;
using Godot;

namespace NightFall.Scripts.Entities.Player;

public partial class PlayerDash : Node
{
    private PlayerStats _stats = null!;

    private float _dashTimer;
    private float _cooldownTimer;
    private Vector2 _direction;

    public bool IsDashing { get; private set; }

    public float CooldownRemaining => _cooldownTimer;

    public float CooldownDuration => _stats.DashCooldown;

    public bool IsOnCooldown => _cooldownTimer > 0f;

    public void Initialize(PlayerStats stats)
    {
        ArgumentNullException.ThrowIfNull(stats);

        _stats = stats;
    }

    public bool StartDash(Vector2 direction)
    {
        GD.Print($"StartDash called | Cooldown: {_cooldownTimer:F2} | IsDashing: {IsDashing}");

        if (direction == Vector2.Zero)
        {
            GD.Print("Dash blocked: zero direction");
            return false;
        }

        if (IsDashing)
        {
            GD.Print("Dash blocked: already dashing");
            return false;
        }

        if (_cooldownTimer > 0f)
        {
            GD.Print("Dash blocked: cooldown");
            return false;
        }

        _direction = direction.Normalized();
        _dashTimer = _stats.DashDuration;
        _cooldownTimer = _stats.DashCooldown;
        IsDashing = true;

        GD.Print("DASH STARTED");

        return true;
    }

    public void UpdateDash(CharacterBody2D player, double delta)
    {
        if (!IsDashing) return;

        player.Velocity = _direction * _stats.DashSpeed;
        player.MoveAndSlide();

        _dashTimer -= (float)delta;

        if (_dashTimer <= 0f)
        {
            _dashTimer = 0f;
            IsDashing = false;
            player.Velocity = Vector2.Zero;
        }
    }

    public void UpdateCooldown(double delta)
    {
        if (_cooldownTimer <= 0f) return;

        _cooldownTimer = Mathf.Max(
            _cooldownTimer - (float)delta,
            0f
        );

        GD.Print($"Cooldown: {_cooldownTimer:F2}, IsDashing: {IsDashing}");
    }
}