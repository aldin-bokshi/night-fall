using Godot;

namespace NightFall.Scripts.Entities.Player.Abilities;

public partial class GravityWellProjectile : Node2D
{
    private static readonly Texture2D? BombTexture =
        GD.Load<Texture2D>("res://Assets/Sprites/Abilities/GravityWellBomb.png");

    private Vector2 _targetPosition;
    private PlayerStats? _stats;
    private Sprite2D? _sprite;

    private bool _initialized;
    private float _spinTimer;

    public void Initialize(
        Vector2 startPosition,
        Vector2 targetPosition,
        PlayerStats stats
    )
    {
        GlobalPosition = startPosition;

        _targetPosition = targetPosition;
        _stats = stats;

        _initialized = true;
        EnsureVisuals();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_initialized || _stats == null)
        {
            return;
        }

        float speed = _stats.GravityWellProjectileSpeed;
        float dt = (float)delta;

        UpdateVisuals(dt);

        if (speed <= 0f)
        {
            CreateGravityWell();
            return;
        }

        Vector2 toTarget = _targetPosition - GlobalPosition;
        float distance = toTarget.Length();

        float movementDistance = speed * dt;

        if (distance <= movementDistance)
        {
            GlobalPosition = _targetPosition;
            CreateGravityWell();
            return;
        }

        GlobalPosition += toTarget.Normalized() * movementDistance;
    }

    private void EnsureVisuals()
    {
        if (_sprite != null)
        {
            return;
        }

        _sprite = new Sprite2D
        {
            Texture = BombTexture,
            Centered = true,
            Scale = Vector2.One * 0.85f
        };

        AddChild(_sprite);
    }

    private void UpdateVisuals(float dt)
    {
        if (_sprite == null)
        {
            return;
        }

        _spinTimer += dt;

        _sprite.Rotation += dt * 3.5f;

        float pulse = 0.85f + Mathf.Sin(_spinTimer * 10f) * 0.08f;
        _sprite.Scale = Vector2.One * pulse;
    }

    private void CreateGravityWell()
    {
        if (_stats == null)
        {
            QueueFree();
            return;
        }

        GravityWell gravityWell = new();

        gravityWell.Initialize(
            GlobalPosition,
            _stats
        );

        GetTree().CurrentScene.AddChild(gravityWell);

        QueueFree();
    }
}
