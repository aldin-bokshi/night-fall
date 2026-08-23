using Godot;
using NightFall.Scripts.Core;
using NightFall.Scripts.Entities.Player;
using NightFall.Scripts.Ui;

namespace NightFall.Scripts.Shop;

public partial class ShopTrigger : Area2D
{
    private PackedScene? _shopScene;
    private Control? _shopInstance;
    private Label? _promptLabel;
    private bool _playerInside;

    public override void _Ready()
    {
        _shopScene = GD.Load<PackedScene>(GamePaths.Shop);

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

        _promptLabel = GetNodeOrNull<Label>("PromptLabel");

        if (_promptLabel != null) _promptLabel.Visible = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_playerInside) return;

        if (!@event.IsActionPressed("ui_accept")) return;

        OpenShop();
        GetViewport().SetInputAsHandled();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is not Player) return;

        _playerInside = true;

        if (_promptLabel != null) _promptLabel.Visible = true;

        FloatingText.Spawn(
            GetParent() ?? this,
            GlobalPosition + new Vector2(0, -30),
            "[E] OPEN SHOP",
            new Color(0.95f, 0.8f, 0.2f),
            14f
        );
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is not Player) return;

        _playerInside = false;

        if (_promptLabel != null) _promptLabel.Visible = false;
    }

    private void OpenShop()
    {
        if (_shopScene == null)
        {
            GD.PushError("ShopTrigger: Shop scene could not be loaded.");
            return;
        }

        if (_shopInstance == null || !GodotObject.IsInstanceValid(_shopInstance))
        {
            CanvasLayer? uiLayer =
                GetTree().CurrentScene?.FindChild("UI", true, false) as CanvasLayer
                ?? GetTree().CurrentScene?.FindChild("HUD", true, false) as CanvasLayer
                ?? GetTree().Root.GetNodeOrNull<CanvasLayer>("HUD");

            _shopInstance = _shopScene.Instantiate<Control>();

            if (uiLayer != null) uiLayer.AddChild(_shopInstance);
            else AddChild(_shopInstance);
        }

        AudioSynthManager.PlayBuy();

        _shopInstance.Show();

        GetTree().Paused = true;
    }
}