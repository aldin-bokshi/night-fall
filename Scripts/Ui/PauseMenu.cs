using Godot;

namespace NightFall.Scripts.Ui;

public partial class PauseMenu : CanvasLayer
{
    private PackedScene? _optionsScene;
    private OptionsMenu? _optionsInstance;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        _optionsScene = GD.Load<PackedScene>("res://Scenes/UI/OptionsMenu/OptionsMenu.tscn");

        this.AttachJuiceToTree();
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
        if (_optionsInstance != null && _optionsInstance.Visible)
        {
            _optionsInstance.Hide();
            return;
        }

        Hide();
        GetTree().Paused = false;
    }

    private void OnContinueButtonPressed()
    {
        ResumeGame();
    }

    private void OnOptionsButtonPressed()
    {
        EnsureOptionsMenu();
        _optionsInstance?.Open();
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/UI/MainMenu/MainMenu.tscn");
    }

    private void EnsureOptionsMenu()
    {
        if (_optionsInstance != null && IsInstanceValid(_optionsInstance))
            return;

        if (_optionsScene != null)
        {
            _optionsInstance = _optionsScene.Instantiate<OptionsMenu>();
            AddChild(_optionsInstance);
        }
    }
}