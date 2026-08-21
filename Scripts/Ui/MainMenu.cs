using Godot;

namespace NightFall.Scripts.Ui;

public partial class MainMenu : Control
{
    private PackedScene? _optionsScene;
    private OptionsMenu? _optionsInstance;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        _optionsScene = GD.Load<PackedScene>("res://Scenes/UI/OptionsMenu/OptionsMenu.tscn");

        this.AttachJuiceToTree();
    }

    private void OnStartButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/UI/SetupScreen/DungeonSetup.tscn");
    }

    private void OnOptionsButtonPressed()
    {
        if (_optionsInstance == null || !IsInstanceValid(_optionsInstance))
        {
            if (_optionsScene != null)
            {
                _optionsInstance = _optionsScene.Instantiate<OptionsMenu>();
                AddChild(_optionsInstance);
            }
        }

        _optionsInstance?.Open();
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
