using Godot;

namespace NightFall.Scripts.Entities.Player.Abilities;

public partial class BlinkAbility : Ability
{
    [Export] public float BlinkDistance { get; set; } = 120f;

    private Player? _player;

    public override void _Ready()
    {
        base._Ready();
        _player = GetParent<Player>();
        if (_player == null) GD.PushError("BlinkAbility could not find the Player.");
    }

    public override bool Use()
    {
        if (!IsReady || _player == null) return false;

        Vector2 direction = GetBlinkDirection();

        if (direction == Vector2.Zero) return false;

        Vector2 targetPosition = GetSafeTargetPosition(direction);

        if (targetPosition == _player.GlobalPosition) return false;

        _player.GlobalPosition = targetPosition;
        StartCooldown();
        return true;
    }

    private Vector2 GetBlinkDirection()
    {
        Vector2 input = _player?.MovementInput ?? Vector2.Zero;

        if (input != Vector2.Zero) return input.Normalized();

        return _player?.FacingDirection ?? Vector2.Zero;
    }

    private Vector2 GetSafeTargetPosition(Vector2 direction)
    {
        if (_player == null) return Vector2.Zero;

        Vector2 start = _player.GlobalPosition;
        Vector2 target = start + direction * BlinkDistance;

        PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(
            start,
            target
        );

        query.Exclude = [_player.GetRid()];

        var result = _player.GetWorld2D().DirectSpaceState.IntersectRay(query);

        if (result.Count == 0) return FindClearTarget(start, direction, BlinkDistance);

        Vector2 collisionPoint = (Vector2)result["position"];
        float availableDistance = start.DistanceTo(collisionPoint) - 4f;

        return FindClearTarget(
            start,
            direction,
            Mathf.Max(0f, Mathf.Min(BlinkDistance, availableDistance))
        );
    }

    private Vector2 FindClearTarget(
        Vector2 start,
        Vector2 direction,
        float distance
    )
    {
        if (_player == null) return start;

        CollisionShape2D? collisionShape =
            _player.GetNodeOrNull<CollisionShape2D>(
                "Hurtbox/CollisionShape2D"
            );

        if (collisionShape?.Shape == null) return start + direction * distance;

        for (float currentDistance = distance; currentDistance >= 0f; currentDistance -= 4f)
        {
            Vector2 candidate = start + direction * currentDistance;
            PhysicsShapeQueryParameters2D query = new()
            {
                Shape = collisionShape.Shape,
                Transform = collisionShape.GlobalTransform with { Origin = candidate },
                CollisionMask = _player.CollisionMask,
                Exclude = [_player.GetRid()]
            };

            if (_player.GetWorld2D().DirectSpaceState.IntersectShape(query, 1).Count == 0)
                return candidate;
        }

        return start;
    }
}