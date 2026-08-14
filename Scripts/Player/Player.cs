using Godot;

namespace RougeLike.Scripts.Player;

public partial class Player : CharacterBody2D
{
    private PlayerInput _input;
    private PlayerMovement _movement;
    private PlayerCombat _combat;
    private PlayerDash _dash;
    private PlayerStats _stats;
    private Sprite2D _sprite;

    public override void _Ready()
    {
        _input = GetNode<PlayerInput>("PlayerInput");
        _movement = GetNode<PlayerMovement>("PlayerMovement");
        _combat = GetNode<PlayerCombat>("PlayerCombat");
        _dash = GetNode<PlayerDash>("PlayerDash");
        _stats = GetNode<PlayerStats>("PlayerStats");
        _sprite = GetNode<Sprite2D>("Sprite2D");

        _movement.Initialize(_stats);
        _combat.Initialize(_stats);
        _dash.Initialize(_stats);
    }

    public override void _PhysicsProcess(double delta)
    {
        _combat.UpdateAttack(delta);
        _dash.UpdateCooldown(delta);

        if (_input.FacingDirection.X != 0f)
        {
            _sprite.FlipH = _input.FacingDirection.X < 0f;
        }

        if (_input.DashPressed)
        {
            _dash.StartDash(_input.MovementInput);
        }

        if (_dash.IsDashing)
        {
            _dash.UpdateDash(this, delta);
            return;
        }

        if (_input.AttackPressed)
        {
            _combat.Attack(_input.FacingDirection);
        }

        _movement.Move(this, _input.MovementInput);
    }
}