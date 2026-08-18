using Godot;

namespace NightFall.Scripts.UI.MainMenu;

public partial class Buttons : Control
{
    private void OnStartButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/Dungeon/Hub/Hub.tscn");
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
