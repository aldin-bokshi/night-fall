using Godot;

namespace NightFall.Scripts.Player;

public partial class PlayerCombat : Node
{
    [Export] public float AttackCooldown = 0.5f;
    [Export] public float AttackDuration = 0.15f;

    private PlayerStats _stats;
    private AttackHitbox _attackHitbox;

    private float _cooldownTimer;
    private float _attackTimer;

    public bool IsAttacking { get; private set; }

    public bool CanAttack()
    {
        return !IsAttacking && _cooldownTimer <= 0f;
    }

    public void Initialize(PlayerStats stats, AttackHitbox attackHitbox)
    {
        _stats = stats;
        _attackHitbox = attackHitbox;

        _attackHitbox.Deactivate();
    }

    public void Attack(Vector2 direction)
    {
        if (!CanAttack()) return;
        if (direction == Vector2.Zero) return;

        IsAttacking = true;

        _cooldownTimer = _stats?.AttackCooldown ?? AttackCooldown;
        _attackTimer = AttackDuration;

        _attackHitbox.Configure(direction);
        _attackHitbox.Activate();
    }

    public void UpdateAttack(double delta)
    {
        float dt = (float)delta;

        if (_cooldownTimer > 0f)
        {
            _cooldownTimer = Mathf.Max(_cooldownTimer - dt,0f);
        }

        if (!IsAttacking) return;
        _attackTimer -= dt;

        if (_attackTimer <= 0f) FinishAttack();
    }

    private void FinishAttack()
    {
        IsAttacking = false;
        _attackTimer = 0f;

        _attackHitbox.Deactivate();
    }
}