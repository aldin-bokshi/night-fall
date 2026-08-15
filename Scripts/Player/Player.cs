using Godot;

namespace NightFall.Scripts.Player;

public partial class Player : CharacterBody2D
{
    private PlayerInput? _input;
    private PlayerMovement? _movement;
    private PlayerCombat? _combat;
    private PlayerDash? _dash;
    private PlayerStats? _stats;
    private Sprite2D? _sprite;

    public override void _Ready()
    {
        _input = GetNode<PlayerInput>("PlayerInput");
        _movement = GetNode<PlayerMovement>("PlayerMovement");
        _combat = GetNode<PlayerCombat>("PlayerCombat");
        _dash = GetNode<PlayerDash>("PlayerDash");
        _stats = GetNode<PlayerStats>("PlayerStats");
        _sprite = GetNode<Sprite2D>("Sprite2D");

        if (_movement != null && _stats != null)
            _movement.Initialize(_stats);
        if (_combat != null && _stats != null)
            _combat.Initialize(_stats);
        if (_dash != null && _stats != null)
            _dash.Initialize(_stats);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_combat != null)
            _combat.UpdateAttack(delta);
        if (_dash != null)
            _dash.UpdateCooldown(delta);

        if (_input != null && _input.FacingDirection.X != 0f)
        {
            if (_sprite != null)
                _sprite.FlipH = _input.FacingDirection.X < 0f;
        }

        if (_input != null && _input.DashPressed && _dash != null)
        {
            _dash.StartDash(_input.MovementInput);
        }

        if (_dash != null && _dash.IsDashing)
        {
            if (_dash != null)
                _dash.UpdateDash(this, delta);
            return;
        }

        if (_input != null && _input.AttackPressed && _combat != null)
        {
            _combat.Attack(_input.FacingDirection);
        }

        if (_movement != null && _input != null)
        {
            _movement.Move(this, _input.MovementInput);
        }
    }
}