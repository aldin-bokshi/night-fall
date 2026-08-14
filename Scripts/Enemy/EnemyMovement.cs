using Godot;

namespace RougeLike.Scripts.Enemy;

public partial class EnemyMovement : Node
{
    [Export] public float Speed = 150f;

    private EnemyStats _stats;

    public void Initialize(EnemyStats stats)
    {
        _stats = stats;
    }

    // public void Move(CharacterBody2D player, Vector2 input)
    // {
    //     var speed = _stats?.MoveSpeed ?? Speed;

    //     if (input != Vector2.Zero)
    //     {
    //         input = input.Normalized();
    //     }

    //     player.Velocity = input * speed;
    //     player.MoveAndSlide();
    // }
}