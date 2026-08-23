using Godot;
using NightFall.Scripts.Core;

namespace NightFall.Scripts.Ui;

public partial class MainMenu : Control
{
    private PackedScene? _optionsScene;
    private OptionsMenu? _optionsInstance;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        _optionsScene = GD.Load<PackedScene>(GamePaths.OptionsMenu);

        this.AttachJuiceToTree();
    }

    private void OnStartButtonPressed()
    {
        GetTree().ChangeSceneToFile(GamePaths.DungeonSetup);
    }

    private void OnOptionsButtonPressed()
    {
        if (_optionsInstance != null && IsInstanceValid(_optionsInstance)) return;
        if (_optionsScene == null) return;
        _optionsInstance = _optionsScene.Instantiate<OptionsMenu>();
        AddChild(_optionsInstance);

        _optionsInstance?.Open();
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
