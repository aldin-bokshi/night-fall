using Godot;

namespace NightFall.Scripts.Enemy;

public partial class Enemy : CharacterBody2D
{
    private EnemyMovement _movement;
    private EnemyCombat _combat;
    private EnemyStats _stats;
    private EnemyAi _ai;

    public int RoomId { get; private set; }

    public override void _Ready()
    {
        _movement = GetNode<EnemyMovement>("EnemyMovement");
        _combat = GetNode<EnemyCombat>("EnemyCombat");
        _stats = GetNode<EnemyStats>("EnemyStats");
        _ai = GetNode<EnemyAi>("EnemyAi");

        _movement.Initialize(_stats);
        _combat.Initialize(_stats);
        _ai.Initialize(_stats);
    }

    public void Initialize(int roomId)
    {
        RoomId = roomId;
    }

    public override void _PhysicsProcess(double delta)
    {
        _combat.UpdateAttack(delta);

        if (_stats.IsDead)
        {
            Die();
            return;
        }

        _combat.UpdateAttack(delta);
    }

    private void Die()
    {
        QueueFree();
    }
}
