using Godot;

namespace RougeLike.Scripts.Player;

public partial class AttackHitbox : Area2D
{
    [Export] public float Distance = 75f;

    private CollisionShape2D _collisionShape;

    public override void _Ready()
    {
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        // Hitbox does NOT exist as an active hitbox until an attack starts.
        Deactivate();
    }

    public void Configure(Vector2 direction)
    {
        if (direction == Vector2.Zero)
            return;

        Position = direction * Distance;
        Rotation = direction.Angle();
    }

    public void Activate()
    {
        Visible = true;
        Monitoring = true;
        Monitorable = true;

        if (_collisionShape != null)
            _collisionShape.Disabled = false;
    }

    public void Deactivate()
    {
        Visible = false;
        Monitoring = false;
        Monitorable = false;

        if (_collisionShape != null)
            _collisionShape.Disabled = true;
    }
}