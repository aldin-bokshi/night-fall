using System;
using Godot;

namespace NightFall.Scripts.Entities.Player;

public partial class PlayerMovement : Node
{
    private PlayerStats? _stats;

    public void Initialize(PlayerStats stats)
    {
        ArgumentNullException.ThrowIfNull(stats);
        _stats = stats;
    }

    public void Move(CharacterBody2D player, Vector2 input)
    {
        if (_stats == null)
        {
            return;
        }

        var speed = _stats.MoveSpeed;

        if (input != Vector2.Zero)
        {
            input = input.Normalized();
        }

        player.Velocity = input * speed;
        player.MoveAndSlide();
    }
}