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

        GD.Print("EnemyAi initialized.");
    }

    public override void _Ready()
    {
        _enemy = GetParent<Enemy>();

        GD.Print($"EnemyAi found enemy: {_enemy.Name}");

        _player = GetTree().GetFirstNodeInGroup("player") as Player.Player
                  ?? throw new InvalidOperationException("Player not found.");

        GD.Print($"EnemyAi found player: {_player.Name}");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsPlayerInAttackRange())
        {
            GD.Print("EnemyAi: Player is in attack range.");

            if (_combat.CanAttack())
            {
                GD.Print("EnemyAi: Enemy is attacking.");

                _combat.Attack(GetDirectionToPlayer());
            }
        }
    }

    private bool IsPlayerInAttackRange()
    {
        float distance = _enemy.GlobalPosition.DistanceTo(
            _player.GlobalPosition
        );

        GD.Print($"EnemyAi: Distance to player = {distance:F1}");

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