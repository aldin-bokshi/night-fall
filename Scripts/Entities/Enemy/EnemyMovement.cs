using System;
using Godot;

namespace NightFall.Scripts.Entities.Enemy;

public partial class EnemyMovement : Node
{
    private EnemyStats? _stats;
    private Vector2 _externalForce;

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

        enemy.Velocity = direction * _stats.MoveSpeed + _externalForce;
        enemy.MoveAndSlide();

        _externalForce = Vector2.Zero;
    }

    /// <summary>
    /// Adds a temporary force that affects the enemy's movement
    /// during the current movement frame.
    /// </summary>
    public void AddExternalForce(Vector2 force)
    {
        _externalForce += force;
    }
}