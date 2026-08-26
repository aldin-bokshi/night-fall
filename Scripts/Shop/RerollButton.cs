using Godot;

namespace NightFall.Scripts.Shop;

public partial class RerollButton : Button
{
    private ShopRerollManager? _rerollManager;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        Pressed += OnPressed;
    }

    public void SetRerollManager(
        ShopRerollManager? rerollManager)
    {
        _rerollManager = rerollManager;
    }

    public void UpdateDisplay(
        int rerollCost,
        int rerollsRemaining,
        int maxRerolls,
        bool canReroll)
    {
        Text =
            $"↻  REROLL  //  {rerollCost} CR  " +
            $"({rerollsRemaining}/{maxRerolls})";

        Disabled = !canReroll;
    }

    private void OnPressed()
    {
        GD.Print("RerollButton: Pressed.");

        if (_rerollManager == null)
        {
            GD.PushError(
                "RerollButton: ShopRerollManager reference is missing.");

            return;
        }

        GD.Print(_rerollManager.TryReroll(GlobalPosition)
            ? "RerollButton: Reroll successful."
            : "RerollButton: Reroll rejected.");
    }

    public override void _ExitTree()
    {
        Pressed -= OnPressed;
    }
}