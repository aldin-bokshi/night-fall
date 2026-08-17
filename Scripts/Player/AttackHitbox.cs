using System.Collections.Generic;
using System.Globalization;
using Godot;
using NightFall.Scripts.Enemy;

namespace NightFall.Scripts.Player;

public partial class AttackHitbox : Area2D
{
    [Export] public float Distance = 75f;

    private CollisionShape2D _collisionShape;

    // Enemies already hit during the current attack
    private readonly HashSet<Node> _hitEnemies = [];

    public override void _Ready()
    {
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

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

    private void OnBodyEntered(Node2D body)
    {
        GD.Print($"AttackHitbox collision detected with: {body.Name} (Type: {body.GetType().Name})");

        if (_hitEnemies.Contains(body))
        {
            GD.Print("Already hit this enemy this attack");
            return;
        }

        var enemyStats = body.GetNodeOrNull<EnemyStats>("EnemyStats");
        if (enemyStats == null)
        {
            GD.Print($"No EnemyStats found on {body.Name}");
            return;
        }

        GD.Print($"Found EnemyStats on {body.Name}. Current HP: {enemyStats.Health}/{enemyStats.MaxHealth}");

        _hitEnemies.Add(body);

        var playerStats = GetParent().GetNodeOrNull<PlayerStats>("PlayerStats");
        if (playerStats == null)
        {
            GD.Print("No PlayerStats found on parent");
            return;
        }

        GD.Print($"Player attack damage: {playerStats.AttackDamage}");
        enemyStats.TakeDamage(playerStats.AttackDamage);

        GD.Print(
            $"Player dealt {playerStats.AttackDamage.ToString(CultureInfo.InvariantCulture)} damage to {body.Name}. " +
            $"Enemy HP: {enemyStats.Health.ToString(CultureInfo.InvariantCulture)}/{enemyStats.MaxHealth.ToString(CultureInfo.InvariantCulture)}"
        );
    }
}