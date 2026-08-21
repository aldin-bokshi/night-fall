using Godot;

namespace NightFall.Scripts.Core;

public partial class VfxManager : Node
{
    public static void TriggerHitFlash(CanvasItem item, Color flashColor, float duration = 0.12f)
    {
        if (item == null || !GodotObject.IsInstanceValid(item)) return;

        Color original = item.Modulate;
        item.Modulate = flashColor;

        Tween tween = item.CreateTween();
        tween.TweenProperty(item, "modulate", original, duration)
             .SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
    }

    public static void TriggerScreenShake(Node requester, float intensity = 6.0f, float duration = 0.2f)
    {
        if (!AudioSynthManager.ScreenShakeEnabled) return;
        if (requester == null || requester.GetTree() == null) return;

        Camera2D? camera = requester.GetViewport()?.GetCamera2D();
        if (camera == null) return;

        Vector2 originalOffset = camera.Offset;
        Tween tween = requester.CreateTween();

        int steps = (int)(duration / 0.03f);
        for (int i = 0; i < steps; i++)
        {
            float decay = 1.0f - ((float)i / steps);
            Vector2 randomOffset = new(
                (float)GD.RandRange(-intensity, intensity) * decay,
                (float)GD.RandRange(-intensity, intensity) * decay
            );
            tween.TweenProperty(camera, "offset", originalOffset + randomOffset, 0.03f);
        }

        tween.TweenProperty(camera, "offset", originalOffset, 0.03f);
    }

    public static void SpawnParticles(Node parent, Vector2 globalPos, Color color, int count = 12)
    {
        if (parent == null) return;

        CpuParticles2D particles = new()
        {
            GlobalPosition = globalPos,
            Emitting = false,
            OneShot = true,
            Amount = count,
            Lifetime = 0.45f,
            Explosiveness = 0.9f,
            Spread = 180f,
            Gravity = new Vector2(0, 90),
            Color = color,
            InitialVelocityMin = 40f,
            InitialVelocityMax = 120f,
            ScaleAmountMin = 2.0f,
            ScaleAmountMax = 4.0f,
            ZIndex = 50
        };

        parent.AddChild(particles);
        particles.Emitting = true;

        SceneTreeTimer timer = parent.GetTree().CreateTimer(0.6f);
        timer.Timeout += () => particles.QueueFree();
    }

    public static void SpawnSlashArc(Node parent, Vector2 globalPos, Vector2 direction, float radius = 40f)
    {
        if (parent == null) return;

        Node2D arcNode = new()
        {
            GlobalPosition = globalPos,
            Rotation = direction.Angle(),
            ZIndex = 40
        };

        Line2D line = new()
        {
            Width = 6.0f,
            DefaultColor = new Color(1.0f, 0.85f, 0.4f, 0.9f)
        };

        int points = 8;
        float arcAngle = Mathf.DegToRad(70f);
        for (int i = 0; i < points; i++)
        {
            float t = (float)i / (points - 1) - 0.5f;
            float angle = t * arcAngle;
            line.AddPoint(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
        }

        arcNode.AddChild(line);
        parent.AddChild(arcNode);

        Tween tween = arcNode.CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(line, "width", 0f, 0.12f);
        tween.TweenProperty(line, "default_color:a", 0f, 0.12f);
        tween.TweenProperty(arcNode, "scale", new Vector2(1.3f, 1.3f), 0.12f);

        tween.Finished += () => arcNode.QueueFree();
    }
}
