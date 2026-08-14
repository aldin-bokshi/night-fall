using Godot;

namespace RougeLike.Scripts.UI.MainMenu;

public partial class Buttons : Control
{
    private void OnStartButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/Levels/Hub/Hub.tscn");
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
