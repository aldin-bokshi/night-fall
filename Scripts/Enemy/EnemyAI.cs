using System;
using Godot;

namespace NightFall.Scripts.Enemy;

public partial class EnemyAi : Node
{
    private EnemyStats _stats = null!;
    private Enemy _enemy = null!;
    private Player.Player _player = null!;
    private EnemyCombat _combat = null!;

    public float DetectionRange => _stats.DetectionRange;
    public float AttackRange => _stats.AttackRange;

    public void Initialize(EnemyStats stats, EnemyCombat combat)
    {
        ArgumentNullException.ThrowIfNull(stats);
        ArgumentNullException.ThrowIfNull(combat);

        _stats = stats;
        _combat = combat;
    }

    public override void _Ready()
    {
        _enemy = GetParent<Enemy>();

        _player = GetTree().GetFirstNodeInGroup("player") as Player.Player
                  ?? throw new InvalidOperationException("Player not found.");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsPlayerInAttackRange())
        {
            return;
        }

        if (_combat.CanAttack())
        {
            _combat.Attack(GetDirectionToPlayer());
        }
    }

    private bool IsPlayerInAttackRange()
    {
        float distance = _enemy.GlobalPosition.DistanceTo(
            _player.GlobalPosition
        );

        return distance <= _stats.AttackRange;
    }

    private Vector2 GetDirectionToPlayer()
    {
        return (
            _player.GlobalPosition -
            _enemy.GlobalPosition
        ).Normalized();
    }
}