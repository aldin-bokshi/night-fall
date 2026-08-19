using System;
using Godot;

namespace NightFall.Scripts.Entities.Enemy;

public partial class EnemyMovement : Node
{
    private EnemyStats? _stats;

    public void Initialize(EnemyStats stats)
    {
        ArgumentNullException.ThrowIfNull(stats);
        _stats = stats;
    }

    public void Move(Enemy enemy, Vector2 direction)
    {
        if (_stats == null)
        {
            return;
        }

        if (direction != Vector2.Zero)
        {
            direction = direction.Normalized();
        }

        enemy.Velocity = direction * _stats.MoveSpeed;
        enemy.MoveAndSlide();
    }
}