using System.Collections.Generic;
using System.Globalization;
using Godot;
using NightFall.Scripts.Enemy;

namespace NightFall.Scripts.Player;

public partial class AttackHitbox : Area2D
{
    [Export] public float Distance = 75f;

    private CollisionShape2D _collisionShape;

    private readonly HashSet<Node> _hitEnemies = [];

    public override void _Ready()
    {
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        AreaEntered += OnAreaEntered;
        BodyEntered += OnBodyEntered;

        Deactivate();
    }

    public void Configure(Vector2 direction)
    {
        if (direction == Vector2.Zero) return;

        Position = direction * Distance;
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