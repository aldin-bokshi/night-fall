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

    [ExportGroup("Gravity Well")]
    [Export] public float GravityWellProjectileSpeed { get; set; } = 500f;
    [Export] public float GravityWellPullStrength { get; set; } = 300f;
    [Export] public float GravityWellRadius { get; set; } = 100f;
    [Export] public float GravityWellDuration { get; set; } = 3f;
    [Export] public float GravityWellDamage { get; set; } = 0f;

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
        if (!CanAfford(amount))
            return false;

        Gold -= amount;
        return true;
    }

    public override void _Ready()
    {
        _health = MaxHealth;

        GD.Print($"[PlayerStats] MaxHealth = {MaxHealth}");
        GD.Print($"[PlayerStats] Health initialized to = {_health}");
}

    public void TakeDamage(float amount)
    {
        _health = Mathf.Max(_health - amount, 0f);
    }

    private void Heal(float amount)
    {
        _health = Mathf.Min(_health + amount, MaxHealth);
    }
}