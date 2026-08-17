using Godot;
using NightFall.Scripts.Player;

namespace NightFall.Scripts.Enemy;

public partial class EnemyCombat : Node
{
    [Export] public float AttackDuration { get; set; } = 0.5f;
    [Export] public Area2D AttackHitbox { get; set; }

    private EnemyStats _stats;

    private float _attackTimer;
    private float _cooldownTimer;

    private Vector2 _facingDirection = Vector2.Right;
    private Vector2 _hitboxNeutralPosition;

    private Area2D _attackHitboxTemplate;

    public bool IsAttacking { get; private set; }

    public void Initialize(EnemyStats stats)
    {
        _stats = stats;

        var attackHitbox = GetParent()?.GetNodeOrNull<Area2D>("AttackHitbox");
        if (attackHitbox == null) return;

        _hitboxNeutralPosition = attackHitbox.Position;
        _attackHitboxTemplate = attackHitbox.Duplicate() as Area2D;

        if (_attackHitboxTemplate != null)
        {
            _attackHitboxTemplate.Name = "AttackHitboxTemplate";
            _attackHitboxTemplate.Monitoring = false;
            _attackHitboxTemplate.Visible = false;
        }

        attackHitbox.QueueFree();
    }

    public void Attack(Vector2 facingDirection)
    {
        if (IsAttacking || _cooldownTimer > 0f) return;

        UpdateFacingDirection(facingDirection);

        IsAttacking = true;
        _attackTimer = AttackDuration;
        _cooldownTimer = _stats?.AttackCooldown ?? 0f;

        SpawnAttackHitbox();
        ConfigureHitboxForFacing();

        if (AttackHitbox != null)
        {
            AttackHitbox.Monitoring = true;
            AttackHitbox.Visible = true;
        }

        GD.Print($"Enemy attacked for {_stats?.AttackDamage ?? 0f} damage.");
    }

    public void UpdateAttack(double delta)
    {
        var deltaTime = (float)delta;

        if (_cooldownTimer > 0f)
        {
            _cooldownTimer = Mathf.Max(
                _cooldownTimer - deltaTime,
                0f
            );
        }

        if (!IsAttacking) return;

        _attackTimer -= deltaTime;

        if (_attackTimer <= 0f) FinishAttack();
    }

    public void FinishAttack()
    {
        IsAttacking = false;

        if (AttackHitbox == null) return;

        AttackHitbox.Monitoring = false;
        AttackHitbox.Visible = false;
        AttackHitbox.QueueFree();

        AttackHitbox = null;
    }

    private void UpdateFacingDirection(Vector2 direction)
    {
        if (direction == Vector2.Zero) return;

        _facingDirection =
            Mathf.Abs(direction.X) >= Mathf.Abs(direction.Y)
                ? new Vector2(Mathf.Sign(direction.X), 0f)
                : new Vector2(0f, Mathf.Sign(direction.Y));
    }

    private void SpawnAttackHitbox()
    {
        if (_attackHitboxTemplate == null) return;

        AttackHitbox = _attackHitboxTemplate.Duplicate() as Area2D;

        if (AttackHitbox == null) return;

        AttackHitbox.Name = "AttackHitbox";
        AttackHitbox.Monitoring = false;
        AttackHitbox.Visible = false;
        AttackHitbox.Position = _hitboxNeutralPosition;

        GetParent()?.AddChild(AttackHitbox);

        AttackHitbox.AreaEntered += OnAttackHitboxAreaEntered;
    }

    //temp

    private void OnAttackHitboxAreaEntered(Area2D area)
    {
        var playerStats = area.GetNodeOrNull<PlayerStats>("PlayerStats");
        if (playerStats == null) return;

        playerStats.TakeDamage(_stats?.AttackDamage ?? 0f);

        GD.Print(
            $"Enemy dealt {_stats?.AttackDamage ?? 0f} damage to player. " +
            $"Player HP: {playerStats.Health}/{playerStats.MaxHealth}"
        );
    }


    private void ConfigureHitboxForFacing()
    {
        if (AttackHitbox == null) return;

        var collisionShape =
            AttackHitbox.GetNodeOrNull<CollisionShape2D>(
                "CollisionShape2D"
            );

        if (collisionShape == null) return;

        if (collisionShape.Shape is not RectangleShape2D shape) return;
        
        shape.Size = _facingDirection.X != 0f ? new Vector2(48f, 24f) : new Vector2(24f, 48f);

        var offsetX = shape.Size.X * 0.5f + 10f;
        var offsetY = shape.Size.Y * 0.5f + 10f;

        AttackHitbox.Position = new Vector2(
            _facingDirection.X != 0f
                ? offsetX * _facingDirection.X
                : 0f,
            _facingDirection.Y != 0f
                ? offsetY * _facingDirection.Y
                : 0f
        );
    }
}