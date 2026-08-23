using System;
using Godot;

namespace NightFall.Scripts.Entities.Player;

public partial class PlayerCombat : Node
{
    private PlayerStats _stats = null!;
    private AttackHitbox _attackHitbox = null!;

    private float _cooldownTimer;
    private float _attackTimer;

    public bool IsAttacking { get; private set; }

    public void Initialize(PlayerStats stats, AttackHitbox attackHitbox)
    {
        ArgumentNullException.ThrowIfNull(stats);
        ArgumentNullException.ThrowIfNull(attackHitbox);

        _stats = stats;
        _attackHitbox = attackHitbox;

        _attackHitbox.Deactivate();
    }

    public bool CanAttack()
    {
        return !IsAttacking && _cooldownTimer <= 0f;
    }

    public void Attack(Vector2 direction)
    {
        if (!CanAttack()) return;
        if (direction == Vector2.Zero) return;

        IsAttacking = true;
        _cooldownTimer = _stats.AttackCooldown;
        _attackTimer = _stats.AttackDuration;

        _attackHitbox.Configure(direction, _stats.AttackRange);
        _attackHitbox.Activate();
    }

    public void UpdateAttack(double delta)
    {
        float dt = (float)delta;

        if (_cooldownTimer > 0f)
        {
            _cooldownTimer = Mathf.Max(_cooldownTimer - dt, 0f);
        }

        if (!IsAttacking) return;

        _attackTimer -= dt;

        if (_attackTimer <= 0f)
            FinishAttack();
    }

    private void FinishAttack()
    {
        IsAttacking = false;
        _attackTimer = 0f;

        _attackHitbox.Deactivate();
    }
}