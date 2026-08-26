using Godot;
using NightFall.Scripts.Entities.Player;

namespace NightFall.Scripts.Shop;

public partial class ShopManager : Node
{
    [Export] private ShopItemManager? _itemManager;
    [Export] private ShopRerollManager? _rerollManager;

    [Export] private Button? _leaveButton;
    [Export] private RerollButton? _rerollButton;
    [Export] private Label? _creditsLabel;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        FindComponents();

        if (_rerollManager != null)
        {
            _rerollManager.Rerolled += OnRerolled;
        }
        else
        {
            GD.PushError(
                "ShopManager: ShopRerollManager could not be found.");
        }

        if (_rerollButton != null)
        {
            _rerollButton.SetRerollManager(_rerollManager);
        }
        else
        {
            GD.PushError(
                "ShopManager: Reroll button could not be found.");
        }

        if (_itemManager != null)
        {
            _itemManager.Initialize();
        }
        else
        {
            GD.PushError(
                "ShopManager: ShopItemManager could not be found.");
        }

        UpdateCreditsDisplay();
        UpdateRerollDisplay();

        if (_leaveButton != null)
        {
            _leaveButton.Pressed += OnLeavePressed;
        }
        else
        {
            GD.PushError(
                "ShopManager: Leave button could not be found.");
        }
    }

    public override void _Process(double delta)
    {
        if (GetParent() is not CanvasItem { Visible: true })
            return;

        UpdateCreditsDisplay();
        UpdateRerollDisplay();
    }

    private void FindComponents()
    {
        _itemManager ??= GetNodeOrNull<ShopItemManager>(

            "../ShopItemManager");

        _rerollManager ??= GetNodeOrNull<ShopRerollManager>(

            "../ShopRerollManager");

        _leaveButton ??= GetParent().GetNodeOrNull<Button>("Leave");

        _rerollButton ??= GetParent().GetNodeOrNull<RerollButton>("Reroll");

        _creditsLabel ??= GetParent().GetNodeOrNull<Label>("Gold");
    }

    private void OnRerolled(
        object? sender,
        RerolledEventArgs eventArgs)
    {
        GD.Print(
            $"ShopManager: Rerolled. " +
            $"Remaining: {eventArgs.RerollsRemaining}");

        _itemManager?.GenerateShopItems();

        UpdateCreditsDisplay();
        UpdateRerollDisplay();
    }

    private void UpdateCreditsDisplay()
    {
        if (_creditsLabel == null)
            return;

        Player? player =
            GetTree().GetFirstNodeInGroup("player") as Player;

        int gold = player?.Stats.Gold ?? 0;

        _creditsLabel.Text = $"CREDITS  {gold}";
    }

    private void UpdateRerollDisplay()
    {
        if (_rerollButton == null || _rerollManager == null)
            return;

        _rerollButton.UpdateDisplay(
            ShopRerollManager.RerollCostValue,
            _rerollManager.RerollsRemaining,
            ShopRerollManager.MaxRerollsValue,
            _rerollManager.CanReroll());
    }

    private void OnLeavePressed()
    {
        GetTree().Paused = false;

        if (GetParent() is Control shopRoot)
        {
            shopRoot.Hide();
        }
    }

    public override void _ExitTree()
    {
        if (_leaveButton != null) _leaveButton.Pressed -= OnLeavePressed;

        if (_rerollManager != null) _rerollManager.Rerolled -= OnRerolled;
    }
}