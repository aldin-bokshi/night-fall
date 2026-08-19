using Godot;

namespace NightFall.Scripts.Ui;

public partial class MainMenu : Control
{
    private void OnStartButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/Dungeon/Dev/TestWorld.tscn");
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
