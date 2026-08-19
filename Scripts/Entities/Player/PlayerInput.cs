using Godot;

namespace NightFall.Scripts.Entities.Player;

public partial class PlayerInput : Node
{
    public Vector2 MovementInput { get; private set; }

    public Vector2 FacingDirection { get; private set; } = Vector2.Right;

    public bool AttackPressed { get; private set; }
    public string? AbilityActionPressed { get; private set; }

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

        for (int index = 1; index <= 4; index++)
        {
            string action = $"ability_{index}";

            if (Input.IsActionJustPressed(action))
            {
                AbilityActionPressed = action;
                break;
            }
        }
    }

    public void ConsumeAttack()
    {
        AttackPressed = false;
    }

    public void ConsumeAbility()
    {
        AbilityActionPressed = null;
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