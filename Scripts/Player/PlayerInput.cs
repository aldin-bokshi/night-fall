using Godot;

namespace NightFall.Scripts.Player;

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

        AttackPressed = Input.IsActionJustPressed("attack");
        DashPressed = Input.IsActionJustPressed("dash");
    }

    private void UpdateFacingDirection()
    {
        if (MovementInput == Vector2.Zero)
            return;

        // Horizontal
        if (Mathf.Abs(MovementInput.X) >= Mathf.Abs(MovementInput.Y))
        {
            FacingDirection = new Vector2(
                Mathf.Sign(MovementInput.X),
                0
            );
        }
        // Vertical
        else
        {
            FacingDirection = new Vector2(
                0,
                Mathf.Sign(MovementInput.Y)
            );
        }
    }
}