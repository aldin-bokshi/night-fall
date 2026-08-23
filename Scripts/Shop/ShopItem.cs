using Godot;
using NightFall.Data.Shop;
using NightFall.Scripts.Core;
using NightFall.Scripts.Entities.Player;
using NightFall.Scripts.Run;
using NightFall.Scripts.Ui;

namespace NightFall.Scripts.Shop;

public partial class ShopItem : Control
{
    [Export] private Label? _rarityLabel;
    [Export] private Label? _nameLabel;
    [Export] private Label? _priceLabel;
    [Export] private VBoxContainer? _statUpgradesContainer;
    [Export] private Button? _buyButton;

    private ItemData? _item;
    private int _effectivePrice;
    private bool _purchased;

    public override void _Ready()
    {
        _buyButton ??= GetNodeOrNull<Button>(
            "Panel/MarginContainer/VBoxContainer/BottomRow/BuyButton");

        _statUpgradesContainer ??= GetNodeOrNull<VBoxContainer>(
            "Panel/MarginContainer/VBoxContainer/StatScroll/StatUpgradesContainer");

        _rarityLabel ??= GetNodeOrNull<Label>(
            "Panel/MarginContainer/VBoxContainer/RarityLabel");

        _nameLabel ??= GetNodeOrNull<Label>(
            "Panel/MarginContainer/VBoxContainer/NameLabel");

        _priceLabel ??= GetNodeOrNull<Label>(
            "Panel/MarginContainer/VBoxContainer/BottomRow/PriceLabel");

        if (_buyButton != null) _buyButton.Pressed += OnBuyPressed;

        this.AttachJuiceToTree();
    }

    public void SetItem(ItemData item)
    {
        _item = item;
        _purchased = false;

        if (_nameLabel != null) _nameLabel.Text = item.Name ?? "ITEM";

        SetRarity(item);
        CalculatePrice(item);
        DisplayStatUpgrades(item);

        if (_buyButton == null) return;

        _buyButton.Disabled = false;
        _buyButton.Text = "ACQUIRE";
    }

    private void SetRarity(ItemData item)
    {
        if (_rarityLabel == null) return;

        string rarity = ShopItemFormatter.GetRarityText(item);

        _rarityLabel.Text = $"◆ {ShopItemFormatter.FormatRarity(rarity)}";
        _rarityLabel.AddThemeColorOverride(
            "font_color",
            ShopItemFormatter.GetRarityColor(rarity));
    }

    private void CalculatePrice(ItemData item)
    {
        _effectivePrice = item.Price;

        if (RunSession.Current is { Greed: true })
            _effectivePrice = (int)(_effectivePrice * 1.5f);

        if (_priceLabel != null) _priceLabel.Text = $"{_effectivePrice} CR";
    }

    private void DisplayStatUpgrades(ItemData item)
    {
        if (_statUpgradesContainer == null) return;

        foreach (Node child in _statUpgradesContainer.GetChildren())
            child.QueueFree();

        if (item.StatUpgrades.Count == 0)
        {
            _statUpgradesContainer.AddChild(
                ShopItemUi.CreateStatLabel("NO STAT UPGRADES"));
            return;
        }

        foreach (var upgrade in item.StatUpgrades)
        {
            string statName = ShopItemFormatter.FormatStatName(upgrade.Key);
            string value = ShopItemFormatter.FormatStatValue(
                upgrade.Key,
                upgrade.Value);

            _statUpgradesContainer.AddChild(
                ShopItemUi.CreateStatLabel($"{statName}    {value}"));
        }
    }

    private void OnBuyPressed()
    {
        if (_purchased || _item == null) return;

        Player? player = GetTree().GetFirstNodeInGroup("player") as Player;
        if (player == null) return;

        var stats = player.Stats;

        if (!stats.CanAfford(_effectivePrice))
        {
            AudioSynthManager.PlayPlayerHurt();

            FloatingText.Spawn(
                GetParent() ?? this,
                GlobalPosition,
                "NOT ENOUGH GOLD!",
                new Color(1f, 0.3f, 0.3f),
                14f);

            return;
        }

        if (!stats.SpendGold(_effectivePrice)) return;

        _purchased = true;

        foreach (var upgrade in _item.StatUpgrades)
            stats.ApplyUpgrade(upgrade.Key, upgrade.Value);

        AudioSynthManager.PlayBuy();

        FloatingText.Spawn(
            GetParent() ?? this,
            GlobalPosition,
            "PURCHASED!",
            new Color(0.3f, 0.95f, 0.4f),
            16f);

        if (_buyButton == null) return;

        _buyButton.Disabled = true;
        _buyButton.Text = "SOLD OUT";
    }
}