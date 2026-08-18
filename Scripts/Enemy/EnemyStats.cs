using Godot;

namespace NightFall.Scripts.Enemy;

public partial class EnemyStats : Node
{
    [Export] public float MaxHealth { get; set; } = 100f;
    public bool IsDead => Health <= 0f;

    [Export] public float MoveSpeed { get; set; } = 150f;

    [Export] public float AttackDamage { get; set; } = 20f;
    [Export] public float AttackCooldown { get; set; } = 0.5f;
    [Export] public float AttackDuration { get; set; } = 0.5f;
    [Export] public float AttackRange { get; set; } = 60f;
    [Export] public float DetectionRange { get; set; } = 120f;

    private float _health;
    public float Health => _health;

    public override void _Ready()
    {
        _health = MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        _health = Mathf.Max(_health - amount, 0f);
    }
}