using Godot;

namespace NightFall.Scripts.Entities.Player.Abilities;

public partial class GravityWellAbility : Ability
{
    private Player? _player;
    private PlayerStats? _stats;

    public override void _Ready()
    {
        base._Ready();

        _player = GetParent().GetParent<Player>();

        if (_player == null)
        {
            GD.PushError(
                "GravityWellAbility could not find the Player."
            );

            return;
        }

        _stats = _player.GetNodeOrNull<PlayerStats>("PlayerStats");

        if (_stats == null)
        {
            GD.PushError(
                "GravityWellAbility could not find PlayerStats."
            );
        }
    }

    public override bool Use()
    {
        if (!IsReady || _player == null || _stats == null)
        {
            return false;
        }

        Vector2 targetPosition = GetGlobalMousePosition();

        LaunchProjectile(targetPosition);

        StartCooldown();
        return true;
    }

    private void LaunchProjectile(Vector2 targetPosition)
    {
        if (_player == null || _stats == null)
        {
            return;
        }

        GravityWellProjectile projectile = new();

        projectile.Initialize(
            _player.GlobalPosition,
            targetPosition,
            _stats
        );

        _player.GetTree().CurrentScene.AddChild(projectile);
    }

    private Vector2 GetGlobalMousePosition()
    {
        if (_player == null)
        {
            return Vector2.Zero;
        }

        return _player.GetGlobalMousePosition();
    }
}
