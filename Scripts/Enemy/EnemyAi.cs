using Godot;

namespace NightFall.Scripts.Enemy;

public partial class EnemyAi : Node
{
    private EnemyStats? _stats;
    private Enemy? _enemy;
    private Player.Player? _player;
    private EnemyCombat? _combat;
    private EnemyMovement? _movement;

    public float DetectionRange => _stats?.DetectionRange ?? 0f;
    public float AttackRange => _stats?.AttackRange ?? 0f;

    public void Initialize(
        EnemyStats stats,
        EnemyCombat combat,
        EnemyMovement movement)
    {
        _stats = stats;
        _combat = combat;
        _movement = movement;
    }

    public override void _Ready()
    {
        _enemy = GetParent<Enemy>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_stats == null || _enemy == null ||
            _combat == null || _movement == null)
        {
            return;
        }

        _player ??= GetTree().GetFirstNodeInGroup("player") as Player.Player;

        if (_player == null) return;

        float distanceToPlayer = _enemy.GlobalPosition.DistanceTo(_player.GlobalPosition);

        if (distanceToPlayer > DetectionRange)
        {
            _movement.Move(_enemy, Vector2.Zero);
            return;
        }

        if (distanceToPlayer <= AttackRange)
        {
            _movement.Move(_enemy, Vector2.Zero);

            if (_combat.CanAttack())
            {
                _combat.Attack(GetDirectionToPlayer());
            }

            return;
        }

        Vector2 directionToPlayer = (
            _player.GlobalPosition -
            _enemy.GlobalPosition
        ).Normalized();

        _movement.Move(_enemy, directionToPlayer);
    }

    private Vector2 GetDirectionToPlayer()
    {
        if (_enemy == null || _player == null)
        {
            return Vector2.Zero;
        }

        return (
            _player.GlobalPosition -
            _enemy.GlobalPosition
        ).Normalized();
    }


}