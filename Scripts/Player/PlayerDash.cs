using Godot;

namespace RougeLike.Scripts.Player;

public partial class PlayerDash : Node
{
    [Export] public float DashSpeed { get; set; } = 500f;
    [Export] public float DashDuration { get; set; } = 0.15f;
    [Export] public float DashCooldown { get; set; } = 1f;

    private PlayerStats _stats;
    private float _dashTimer;
    private float _cooldownTimer;
    private Vector2 _direction;

    public bool IsDashing { get; private set; }

    public void Initialize(PlayerStats stats)
    {
        _stats = stats;
    }

    public void StartDash(Vector2 direction)
    {
        if (direction == Vector2.Zero) { return; }
        if (IsDashing) { return; }
        if (_cooldownTimer > 0f) { return; }

        _direction = direction;
        _dashTimer = _stats?.DashDuration ?? DashDuration;
        _cooldownTimer = _stats?.DashCooldown ?? DashCooldown;
        IsDashing = true;
    }

    public void UpdateDash(CharacterBody2D player, double delta)
    {
        if (!IsDashing) { return; }

        var dashSpeed = _stats?.DashSpeed ?? DashSpeed;
        player.Velocity = _direction * dashSpeed;
        player.MoveAndSlide();

        _dashTimer -= (float)delta;
        if (_dashTimer <= 0f)
        {
            IsDashing = false;
        }
    }

    public void UpdateCooldown(double delta)
    {
        if (_cooldownTimer <= 0f) { return; }
        _cooldownTimer = Mathf.Max(_cooldownTimer - (float)delta, 0f);
    }
}