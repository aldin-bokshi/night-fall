using System.Collections.Generic;
using Godot;
using NightFall.Scripts.Core;
using NightFall.Scripts.Entities.Enemy;
using NightFall.Scripts.Ui;

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

        AudioSynthManager.PlaySlash();
        VfxManager.SpawnSlashArc(GetParent(), GlobalPosition, Transform.X, 36f);
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

        float damage = playerStats.AttackDamage;
        enemyStats.TakeDamage(damage);

        AudioSynthManager.PlayHit();
        FloatingText.Spawn(enemyRoot.GetParent() ?? enemyRoot, enemyRoot.GlobalPosition, $"{damage:F0}", new Color(1.0f, 0.9f, 0.3f));
        VfxManager.TriggerHitFlash(enemyRoot, new Color(2f, 2f, 2f, 1f));
        VfxManager.TriggerScreenShake(this, 5.0f, 0.15f);
        VfxManager.SpawnParticles(enemyRoot.GetParent() ?? enemyRoot, enemyRoot.GlobalPosition, new Color(0.9f, 0.2f, 0.2f), 10);
    }
}