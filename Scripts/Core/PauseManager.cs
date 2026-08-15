using Godot;

namespace NightFall.Scripts.Core;

public partial class PauseManager : Node
{
    [Export] public Control PauseMenu { get; private set; }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        PauseMenu.Hide();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        GetTree().Paused = !GetTree().Paused;

        if (GetTree().Paused)
        {
            PauseMenu.Show();
        }
        else
        {
            PauseMenu.Hide();
        }
    }
}