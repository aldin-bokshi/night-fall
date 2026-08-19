using System.Collections.Generic;
using Godot;
using NightFall.Scripts.Entities.Enemy;

namespace NightFall.Scripts.Entities.Player;

public partial class AttackHitbox : Area2D
{
    private CollisionShape2D _collisionShape = null!;

    private readonly HashSet<Node> _hitEnemies = [];

    public override void _Ready()
    {
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        AreaEntered += OnAreaEntered;
        BodyEntered += OnBodyEntered;

        Deactivate();
    }

    public void Configure(Vector2 direction, float distance)
    {
        if (direction == Vector2.Zero) return;

        Position = direction * distance;
        Rotation = direction.Angle();
    }

    public void Activate()
    {
        _hitEnemies.Clear();

        Visible = true;
        Monitoring = true;
        Monitorable = true;

        _collisionShape.Disabled = false;
    }

    public void Deactivate()
    {
        Visible = false;
        Monitoring = false;
        Monitorable = false;

        _collisionShape.Disabled = true;
    }

    private void OnAreaEntered(Area2D area)
    {
        TryApplyDamage(area.GetParent() as Node2D ?? area);
    }

    private void OnBodyEntered(Node2D body)
    {
        TryApplyDamage(body);
    }

    private void TryApplyDamage(Node target)
    {
        var enemyRoot = target as Node2D;
        if (enemyRoot == null)
            return;

        if (_hitEnemies.Contains(enemyRoot))
            return;

        var enemyStats = enemyRoot.GetNodeOrNull<EnemyStats>("EnemyStats");
        if (enemyStats == null)
            return;

        _hitEnemies.Add(enemyRoot);

        var playerStats = GetParent()?.GetNodeOrNull<PlayerStats>("PlayerStats");
        if (playerStats == null)
            return;

        enemyStats.TakeDamage(playerStats.AttackDamage);
    }
}