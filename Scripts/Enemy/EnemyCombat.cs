using System;
using Godot;
using NightFall.Scripts.Player;

namespace NightFall.Scripts.Enemy;

public partial class EnemyCombat : Node
{
    private EnemyStats _stats = null!;
    private Area2D _attackHitbox = null!;

    private float _attackTimer;
    private float _cooldownTimer;

    private Vector2 _facingDirection = Vector2.Right;

    public bool IsAttacking { get; private set; }

    public void Initialize(EnemyStats stats)
    {
        ArgumentNullException.ThrowIfNull(stats);

        _stats = stats;
        _attackHitbox = GetParent()!.GetNode<Area2D>("AttackHitbox");

        _attackHitbox.AreaEntered += OnTargetEntered;

        _attackHitbox.Monitoring = false;
        _attackHitbox.Visible = false;
    }

    public bool CanAttack()
    {
        return !IsAttacking && _cooldownTimer <= 0f;
    }

    public void Attack(Vector2 direction)
    {
        if (!CanAttack() || direction == Vector2.Zero) return;

        _facingDirection = direction.Normalized();

        IsAttacking = true;
        _attackTimer = _stats.AttackDuration;
        _cooldownTimer = _stats.AttackCooldown;

        _attackHitbox.Position = _facingDirection * _stats.AttackRange;

        _attackHitbox.Rotation = _facingDirection.Angle();

        _attackHitbox.Monitoring = true;
        _attackHitbox.Visible = true;
    }

    public void UpdateAttack(double delta)
    {
        float dt = (float)delta;

        _cooldownTimer = Mathf.Max(
            _cooldownTimer - dt,
            0f
        );

        if (!IsAttacking) return;

        _attackTimer -= dt;

        if (_attackTimer <= 0f) FinishAttack();
    }

    private void FinishAttack()
    {
        IsAttacking = false;
        _attackTimer = 0f;

        _attackHitbox.Monitoring = false;
        _attackHitbox.Visible = false;
    }

    private void OnTargetEntered(Area2D target)
    {
        if (target.Name != "Hurtbox") return;

        var player = target.GetParent<Player.Player>();

        if (player == null) return;

        var playerStats = player.GetNodeOrNull<PlayerStats>("PlayerStats");

        if (playerStats == null) return;

        playerStats.TakeDamage(_stats.AttackDamage);
        
        GD.Print(
            $"Enemy attacked! " +
            $"Damage: {_stats.AttackDamage:F0} | " +
            $"Player HP: {playerStats.Health:F0}/{playerStats.MaxHealth:F0} | " +
            $"Cooldown: {_stats.AttackCooldown:F2}s"
        );
    }
}