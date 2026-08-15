using Godot;

namespace NightFall.Scripts.UI.HUD;

public partial class Hud : CanvasLayer
{
    [Export] private Label? _goldLabel;
    [Export] private ProgressBar? _healthBar;
    [Export] private Control? _wheel;
    private Control? _pointerPivot;

    public override void _Ready()
    {
        _pointerPivot = GetNode<Control>(
            "Panel/MarginContainer/VBoxContainer/Wheel/PointerPivot"
        );
    }

    public override void _Process(double delta)
    {
        if(_pointerPivot!=null)_pointerPivot.Rotation += (float)delta * 2;
    }
}