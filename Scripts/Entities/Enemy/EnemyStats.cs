using Godot;

namespace NightFall.Scripts.Entities.Enemy;

public partial class EnemyStats : Node
{
    [Export] public EnemyVariant Variant { get; set; } = EnemyVariant.Standard;

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
        ApplyVariant();
        _health = MaxHealth;
    }

    private void ApplyVariant()
    {
        switch (Variant)
        {
            case EnemyVariant.Fast:
                MaxHealth = 65f;
                MoveSpeed = 260f;
                AttackDamage = 14f;
                AttackCooldown = 0.35f;
                AttackRange = 55f;
                DetectionRange = 420f;
                break;

            case EnemyVariant.Tank:
                MaxHealth = 240f;
                MoveSpeed = 90f;
                AttackDamage = 30f;
                AttackCooldown = 0.9f;
                AttackDuration = 0.65f;
                AttackRange = 70f;
                DetectionRange = 300f;
                break;
        }
    }

    public void TakeDamage(float amount)
    {
        _health = Mathf.Max(_health - amount, 0f);
    }
}