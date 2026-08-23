using Godot;
using NightFall.Scripts.Entities.Enemy;

namespace NightFall.Scripts.Entities.Player.Abilities;

public partial class GravityWell : Node2D
{
    private static readonly Texture2D? WellTexture =
        GD.Load<Texture2D>("res://Assets/Sprites/Abilities/GravityWell.png");

    private static readonly Texture2D? OutlineTexture =
        GD.Load<Texture2D>("res://Assets/Sprites/Abilities/GravityWellOutline.png");

    private PlayerStats? _stats;
    private float _remainingDuration;
    private Sprite2D? _outlineSprite;
    private Sprite2D? _coreSprite;
    private float _pulseTimer;

    public void Initialize(
        Vector2 position,
        PlayerStats stats
    )
    {
        GlobalPosition = position;

        _stats = stats;
        _remainingDuration = stats.GravityWellDuration;
        EnsureVisuals();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_stats == null)
        {
            QueueFree();
            return;
        }

        _remainingDuration -= (float)delta;

        if (_remainingDuration <= 0f)
        {
            QueueFree();
            return;
        }

        UpdateVisuals((float)delta);
        PullEnemies();
    }

    private void EnsureVisuals()
    {
        if (_coreSprite != null)
        {
            return;
        }

        _outlineSprite = new Sprite2D
        {
            Texture = OutlineTexture,
            Centered = true,
            Modulate = new Color(1f, 1f, 1f, 0.85f),
            Scale = Vector2.One * 0.15f
        };

        _coreSprite = new Sprite2D
        {
            Texture = WellTexture,
            Centered = true,
            Modulate = new Color(1f, 1f, 1f, 0f),
            Scale = Vector2.One * 0.1f
        };

        AddChild(_outlineSprite);
        AddChild(_coreSprite);

        Tween intro = CreateTween();
        intro.SetParallel(true);
        intro.TweenProperty(_outlineSprite, "scale", Vector2.One, 0.15f);
        intro.TweenProperty(_outlineSprite, "modulate:a", 0.9f, 0.15f);
        intro.TweenProperty(_coreSprite, "scale", Vector2.One, 0.15f);
        intro.TweenProperty(_coreSprite, "modulate:a", 1f, 0.15f);
    }

    private void UpdateVisuals(float dt)
    {
        if (_outlineSprite == null || _coreSprite == null) return;

        _pulseTimer += dt;

        float corePulse = 1f + Mathf.Sin(_pulseTimer * 5f) * 0.05f;
        float outlinePulse = 1f + Mathf.Sin(_pulseTimer * 3.5f) * 0.08f;

        _coreSprite.Rotation -= dt * 0.6f;
        _outlineSprite.Rotation += dt * 0.35f;

        _coreSprite.Scale = Vector2.One * corePulse;
        _outlineSprite.Scale = Vector2.One * outlinePulse;
    }

    private void PullEnemies()
    {
        if (_stats == null)
        {
            return;
        }

        float radius = _stats.GravityWellRadius;

        if (radius <= 0f)
        {
            return;
        }

        foreach (Node node in GetTree().GetNodesInGroup("enemy"))
        {
            if (node is not Enemy.Enemy enemy)
            {
                continue;
            }

            float distance = GlobalPosition.DistanceTo(
                enemy.GlobalPosition
            );

            if (distance > radius)
            {
                continue;
            }

            ApplyPull(enemy, distance, radius);
        }
    }

    private void ApplyPull(
        Enemy.Enemy enemy,
        float distance,
        float radius
    )
    {
        if (_stats == null)
        {
            return;
        }

        Vector2 direction =
            enemy.GlobalPosition.DirectionTo(GlobalPosition);

        if (direction == Vector2.Zero)
        {
            return;
        }

        float strengthMultiplier =
            1f - Mathf.Clamp(distance / radius, 0f, 1f);

        Vector2 force =
            direction *
            _stats.GravityWellPullStrength *
            strengthMultiplier;

        EnemyMovement? movement =
            enemy.GetNodeOrNull<EnemyMovement>("EnemyMovement");

        movement?.AddExternalForce(force);
    }
}
