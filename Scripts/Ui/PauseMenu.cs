using Godot;

namespace NightFall.Scripts.Ui;

public partial class PauseMenu : CanvasLayer
{
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        Hide();
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if (!inputEvent.IsActionPressed("pause"))
            return;

        TogglePause();

        GetViewport().SetInputAsHandled();
    }

    private void TogglePause()
    {
        if (GetTree().Paused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        Show();
        GetTree().Paused = true;
    }

    private void ResumeGame()
    {
        Hide();
        GetTree().Paused = false;
    }
}