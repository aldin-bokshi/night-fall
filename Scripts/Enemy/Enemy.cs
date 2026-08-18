using Godot;

namespace NightFall.Scripts.Enemy;

public partial class Enemy : CharacterBody2D
{
    private EnemyMovement _movement = null!;
    private EnemyCombat _combat = null!;
    private EnemyStats _stats = null!;
    private EnemyAi _ai = null!;

    public int RoomId { get; private set; }

    public override void _Ready()
    {
        _movement = GetNode<EnemyMovement>("EnemyMovement");
        _combat = GetNode<EnemyCombat>("EnemyCombat");
        _stats = GetNode<EnemyStats>("EnemyStats");
        _ai = GetNode<EnemyAi>("EnemyAi");

        _movement.Initialize(_stats);
        _combat.Initialize(_stats);
        _ai.Initialize(_stats, _combat);
    }

    public void Initialize(int roomId)
    {
        RoomId = roomId;
    }

    public override void _PhysicsProcess(double delta)
    {
        _combat.UpdateAttack(delta);

        if (_stats.IsDead) Die();
    }

    private void Die()
    {
        QueueFree();
    }
}
