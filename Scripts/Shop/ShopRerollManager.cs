using System;
using Godot;
using NightFall.Scripts.Core;
using NightFall.Scripts.Entities.Player;
using NightFall.Scripts.Ui;

namespace NightFall.Scripts.Shop;

public partial class ShopRerollManager : Node
{
    private const int RerollCost = 150;
    private const int MaxRerolls = 3;

    private int _rerollsRemaining = MaxRerolls;

    public static int RerollCostValue => RerollCost;
    public int RerollsRemaining => _rerollsRemaining;
    public static int MaxRerollsValue => MaxRerolls;

    public event EventHandler<RerolledEventArgs>? Rerolled;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        _rerollsRemaining = MaxRerolls;
    }

    public bool CanReroll()
    {
        if (_rerollsRemaining <= 0)
            return false;

        Player? player =
            GetTree().GetFirstNodeInGroup("player") as Player;

        if (player == null)
        {
            GD.PushWarning(
                "ShopRerollManager: Player could not be found.");

            return false;
        }

        return player.Stats.CanAfford(RerollCost);
    }

    public bool TryReroll(Vector2 buttonPosition)
    {
        Player? player =
            GetTree().GetFirstNodeInGroup("player") as Player;

        if (player == null)
        {
            GD.PushError(
                "ShopRerollManager: Player could not be found.");

            return false;
        }

        if (_rerollsRemaining <= 0)
        {
            GD.PushWarning(
                "ShopRerollManager: No rerolls remaining.");

            return false;
        }

        if (!player.Stats.CanAfford(RerollCost))
        {
            GD.PushWarning(
                $"ShopRerollManager: Cannot afford reroll. " +
                $"Gold: {player.Stats.Gold}, Cost: {RerollCost}.");

            return false;
        }

        if (!player.Stats.SpendGold(RerollCost))
        {
            GD.PushError(
                "ShopRerollManager: SpendGold failed.");

            return false;
        }

        _rerollsRemaining--;

        AudioSynthManager.PlayBuy();

        FloatingText.Spawn(
            GetParent() ?? this,
            buttonPosition,
            "REROLLED!",
            new Color(0.68f, 0.46f, 0.86f),
            14f);

        Rerolled?.Invoke(
            this,
            new RerolledEventArgs(_rerollsRemaining));

        return true;
    }
}