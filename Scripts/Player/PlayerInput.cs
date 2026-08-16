using Godot;

namespace RougeLike.Scripts.Player;

public partial class PlayerInput : Node
{
    public Vector2 MovementInput { get; private set; }

    public Vector2 FacingDirection { get; private set; } = Vector2.Right;

    public bool AttackPressed { get; private set; }
    public bool DashPressed { get; private set; }

    public override void _Process(double delta)
    {
        MovementInput = Input.GetVector(
            "move_left",
            "move_right",
            "move_up",
            "move_down"
        );

        UpdateFacingDirection();

        if (Input.IsActionJustPressed("attack"))
            AttackPressed = true;

        if (Input.IsActionJustPressed("dash"))
            DashPressed = true;
    }

    public void ConsumeAttack()
    {
        AttackPressed = false;
    }

    public void ConsumeDash()
    {
        DashPressed = false;
    }

    private void UpdateFacingDirection()
    {
        if (MovementInput == Vector2.Zero)
            return;

        if (Mathf.Abs(MovementInput.X) >= Mathf.Abs(MovementInput.Y))
        {
            FacingDirection = new Vector2(
                Mathf.Sign(MovementInput.X),
                0
            );
        }
        else
        {
            FacingDirection = new Vector2(
                0,
                Mathf.Sign(MovementInput.Y)
            );
        }
    }
}