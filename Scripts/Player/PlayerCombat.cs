using Godot;

namespace RougeLike.Scripts.Player;

public partial class PlayerCombat : Node
{
    [Export] public float AttackDamage = 10f;
    [Export] public float AttackCooldown = 1f;
    [Export] public float AttackDuration = 0.5f;
    [Export] public Area2D AttackHitbox;
    [Export] public Vector2 AttackkDirection;
    [Export] public Animation AttackAnimation;
    
    private PlayerStats _stats;
    private float _attackTimer;
    private float _cooldownTimer;
    private Vector2 _facingDirection = Vector2.Right;
    private Vector2 _hitboxNeutralPosition;
    private Area2D _attackHitboxTemplate;

    public bool IsAttacking { get; private set; }

    public void Initialize(PlayerStats stats)
    {
        _stats = stats;

        if (AttackHitbox == null)
        {
            AttackHitbox = GetParent()?.GetNodeOrNull<Area2D>("AttackHitbox");
        }

        if (AttackHitbox != null)
        {
            _hitboxNeutralPosition = AttackHitbox.Position;
            _attackHitboxTemplate = AttackHitbox.Duplicate() as Area2D;
            if (_attackHitboxTemplate != null)
            {
                _attackHitboxTemplate.Name = "AttackHitboxTemplate";
                _attackHitboxTemplate.Monitoring = false;
                _attackHitboxTemplate.Visible = false;
            }

            AttackHitbox.QueueFree();
            AttackHitbox = null;
        }
    }

    public void Attack(Vector2 facingDirection)
    {
        if (IsAttacking || _cooldownTimer > 0f) { return; }

        if (facingDirection != Vector2.Zero)
        {
            if (Mathf.Abs(facingDirection.X) >= Mathf.Abs(facingDirection.Y))
            {
                _facingDirection = new Vector2(Mathf.Sign(facingDirection.X), 0f);
            }
            else
            {
                _facingDirection = new Vector2(0f, Mathf.Sign(facingDirection.Y));
            }
        }

        IsAttacking = true;
        _attackTimer = AttackDuration;
        _cooldownTimer = _stats?.AttackCooldown ?? AttackCooldown;

        SpawnAttackHitbox();
        ConfigureHitboxForFacing();

        if (AttackHitbox != null)
        {
            AttackHitbox.Monitoring = true;
            AttackHitbox.Visible = true;
        }

        var damage = _stats?.AttackDamage ?? AttackDamage;
        GD.Print($"Player attacked for {damage} damage!");

        // play animation
        // enable hitbox
        // damage enemies
    }

    public void UpdateAttack(double delta)
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer = Mathf.Max(_cooldownTimer - (float)delta, 0f);
        }

        if (!IsAttacking) { return; }

        _attackTimer -= (float)delta;
        if (_attackTimer <= 0f)
        {
            FinishAttack();
        }
    }

    public void FinishAttack()
    {
        IsAttacking = false;
        if (AttackHitbox != null)
        {
            AttackHitbox.Monitoring = false;
            AttackHitbox.Visible = false;
            AttackHitbox.QueueFree();
            AttackHitbox = null;
        }
    }

    private void SpawnAttackHitbox()
    {
        if (_attackHitboxTemplate == null) { return; }
        if (AttackHitbox != null) { AttackHitbox.QueueFree(); }

        AttackHitbox = _attackHitboxTemplate.Duplicate() as Area2D;
        if (AttackHitbox == null) { return; }

        AttackHitbox.Name = "AttackHitbox";
        AttackHitbox.Monitoring = false;
        AttackHitbox.Visible = false;
        AttackHitbox.Position = _hitboxNeutralPosition;

        GetParent()?.AddChild(AttackHitbox);
    }

    private void ConfigureHitboxForFacing()
    {
        if (AttackHitbox == null) { return; }

        var collisionShape = AttackHitbox.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (collisionShape == null) { return; }

        var shape = collisionShape.Shape as RectangleShape2D;
        if (shape == null) { return; }

        if (_facingDirection.X != 0f)
        {
            shape.Size = new Vector2(48f, 24f);
        }
        else if (_facingDirection.Y != 0f)
        {
            shape.Size = new Vector2(24f, 48f);
        }

        var offsetX = shape.Size.X * 0.5f + 10f;
        var offsetY = shape.Size.Y * 0.5f + 10f;

        AttackHitbox.Position = new Vector2(
            _facingDirection.X != 0f ? offsetX * _facingDirection.X : 0f,
            _facingDirection.Y != 0f ? offsetY * _facingDirection.Y : 0f
        );
    }
}