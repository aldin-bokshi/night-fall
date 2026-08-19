using Godot;

namespace NightFall.Scripts.Entities.Player;

public partial class Player : CharacterBody2D
{
    private PlayerInput _input = null!;
    private PlayerMovement _movement = null!;
    private PlayerCombat _combat = null!;
    private PlayerDash _dash = null!;
    private PlayerStats _stats = null!;
    private AttackHitbox _attackHitbox = null!;
    private Sprite2D _sprite = null!;

    public PlayerStats Stats => _stats;
    public PlayerDash Dash => _dash;

    public override void _Ready()
    {
        _input = GetNode<PlayerInput>("PlayerInput");
        _movement = GetNode<PlayerMovement>("PlayerMovement");
        _combat = GetNode<PlayerCombat>("PlayerCombat");
        _dash = GetNode<PlayerDash>("PlayerDash");
        _stats = GetNode<PlayerStats>("PlayerStats");
        _attackHitbox = GetNode<AttackHitbox>("AttackHitbox");
        _sprite = GetNode<Sprite2D>("Sprite2D");

        _movement.Initialize(_stats);
        _combat.Initialize(_stats, _attackHitbox);
        _dash.Initialize(_stats);
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateSystems(delta);
        UpdateSprite();

        if (HandleDash()) return;

        HandleAttack();
        HandleMovement();

        if (_stats.IsDead) Die();
    }

    private void UpdateSystems(double delta)
    {
        _combat.UpdateAttack(delta);
        _dash.UpdateCooldown(delta);
        _dash.UpdateDash(this, delta);
    }

    private void UpdateSprite()
    {
        if (_input.FacingDirection.X != 0f) _sprite.FlipH = _input.FacingDirection.X < 0f;
    }

    private bool HandleDash()
    {
        if (!_input.DashPressed) return false;

        Vector2 dashDirection =
            _input.MovementInput == Vector2.Zero
                ? _input.FacingDirection
                : _input.MovementInput;

        _input.ConsumeDash();

        return _dash.StartDash(dashDirection);
    }

    private void HandleAttack()
    {
        if (!_input.AttackPressed) return;

        if (_input.FacingDirection == Vector2.Zero) return;

        if (!_combat.CanAttack()) return;

        _combat.Attack(_input.FacingDirection);
        _input.ConsumeAttack();
    }

    private void HandleMovement()
    {
        _movement.Move(this, _input.MovementInput);
    }

    private void Die()
    {
        GetTree().ChangeSceneToFile("res://Scenes/UI/DeathScreen/DeathScreen.tscn");

        QueueFree();
    }
}