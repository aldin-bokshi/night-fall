using Godot;

namespace NightFall.Scripts.Entities.Player;

public partial class PlayerStats : Node
{
    [Export] public float MaxHealth { get; set; } = 100f;

    public bool IsDead => Health <= 0f;

    [Export] public float MoveSpeed { get; set; } = 150f;

    [Export] public float AttackDamage { get; set; } = 20f;
    [Export] public float AttackCooldown { get; set; } = 0.5f;
    [Export] public float AttackDuration { get; set; } = 0.15f;
    [Export] public float AttackRange { get; set; } = 75f;

    [Export] public float Defense { get; set; }
    [Export] public float Lifesteal { get; set; }
    [Export] public float Luck { get; set; }

    [ExportGroup("Gravity Well")] [Export] public float GravityWellProjectileSpeed { get; set; } = 500f;
    [Export] public float GravityWellPullStrength { get; set; } = 300f;
    [Export] public float GravityWellRadius { get; set; } = 100f;
    [Export] public float GravityWellDuration { get; set; } = 3f;
    [Export] public float GravityWellDamage { get; set; }

    public int Gold { get; private set; }

    private float _health;

    public float Health => _health;

    public void AddGold(int amount)
    {
        Gold += amount;
    }

    public bool CanAfford(int price)
    {
        return Gold >= price;
    }

    public bool SpendGold(int amount)
    {
        if (!CanAfford(amount)) return false;

        Gold -= amount;
        return true;
    }

    public override void _Ready()
    {
        _health = MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        float actualDamage = Mathf.Max(1.0f, amount - Defense);
        _health = Mathf.Max(_health - actualDamage, 0f);
    }

    public void Heal(float amount)
    {
        _health = Mathf.Min(_health + amount, MaxHealth);
    }

    public void ApplyUpgrade(string statKey, float amount)
    {
        switch (statKey.ToUpperInvariant())
        {
            case "MAX_HEALTH":
                MaxHealth += amount;
                Heal(amount);
                break;

            case "DAMAGE":
                AttackDamage += amount;
                break;

            case "MOVE_SPEED":
                MoveSpeed += amount;
                break;

            case "ATTACK_SPEED":
                AttackCooldown = Mathf.Max(
                    0.15f,
                    AttackCooldown * (1.0f - amount / 100.0f));
                break;

            case "DEFENSE":
                Defense += amount;
                break;

            case "LIFESTEAL":
                Lifesteal += amount;
                break;

            case "LUCK":
                Luck += amount;
                break;

            case "COOLDOWN":
                AttackCooldown = Mathf.Max(
                    0.15f,
                    AttackCooldown * (1.0f + amount / 100.0f));
                break;

            default:
                GD.Print($"Unknown stat upgrade: {statKey}");
                break;
        }
    }
}