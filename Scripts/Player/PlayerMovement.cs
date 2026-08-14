using Godot;

namespace RougeLike.Scripts.Player;

public partial class PlayerMovement : Node
{
    [Export] public float Speed = 150f;

    private PlayerStats _stats;

    public void Initialize(PlayerStats stats)
    {
        _stats = stats;
    }

    public void Move(CharacterBody2D player, Vector2 input)
    {
        var speed = _stats?.MoveSpeed ?? Speed;

        if (input != Vector2.Zero)
        {
            input = input.Normalized();
        }

        player.Velocity = input * speed;
        player.MoveAndSlide();
    }
}