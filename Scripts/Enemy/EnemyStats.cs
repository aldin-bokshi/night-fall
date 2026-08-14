using Godot;

namespace RougeLike.Scripts.Enemy;

public partial class EnemyStats : Node
{
    [Export] public float MaxHealth { get; set; } = 100f;
    [Export] public float MoveSpeed { get; set; } = 150f;
    [Export] public float AttackDamage { get; set; } = 20f;
    [Export] public float AttackCooldown { get; set; } = 0.5f;

    public float Health { get; private set; }

    public override void _Ready()
    {
        Health = MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        Health = Mathf.Max(Health, 0);
    }
}