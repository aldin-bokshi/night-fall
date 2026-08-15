using Godot;

namespace NightFall.Scripts.Enemy;

public partial class Enemy : CharacterBody2D
{
    private EnemyMovement _movement;
    private EnemyCombat _combat;
    private EnemyStats _stats;
    private Sprite2D _sprite;

    public int RoomId { get; private set; }

    public void Initialize(int roomId)
    {
        RoomId = roomId;
    }

    public override void _Ready()
    {
        _movement = GetNode<EnemyMovement>("EnemyMovement");
        _combat = GetNode<EnemyCombat>("EnemyCombat");
        _stats = GetNode<EnemyStats>("EnemyStats");
        _sprite = GetNode<Sprite2D>("Sprite2D");

        _movement.Initialize(_stats);
        _combat.Initialize(_stats);
    }

    public override void _PhysicsProcess(double delta)
    {
        _combat.UpdateAttack(delta);
    }
}