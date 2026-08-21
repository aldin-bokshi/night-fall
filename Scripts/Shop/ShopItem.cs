using System.Text;
using Godot;
using NightFall.Data.Shop;
using NightFall.Scripts.Core;
using NightFall.Scripts.Entities.Player;
using NightFall.Scripts.Run;
using NightFall.Scripts.Ui;

namespace NightFall.Scripts.Shop;

public partial class ShopItem : Control
{
    [Export] private Label? _nameLabel;
    [Export] private Label? _priceLabel;
    [Export] private Label? _statLabel;
    [Export] private Button? _buyButton;

    private ItemData? _item;
    private int _effectivePrice;
    private bool _purchased;

    public override void _Ready()
    {
        _buyButton ??= GetNodeOrNull<Button>("Panel/MarginContainer/VBoxContainer/BuyButton");
        if (_buyButton != null)
        {
            _buyButton.Pressed += OnBuyPressed;
        }

        this.AttachJuiceToTree();
    }

    public void SetItem(ItemData item)
    {
        _item = item;
        _purchased = false;

        if (_nameLabel != null) _nameLabel.Text = item.Name ?? "Item";

        _effectivePrice = item.Price;
        var runConfig = RunSession.Current;
        if (runConfig != null && runConfig.Greed)
        {
            _effectivePrice = (int)(_effectivePrice * 1.5f);
        }

        if (_priceLabel != null)
        {
            _priceLabel.Text = $"{_effectivePrice} Gold";
        }

        StringBuilder upgradesText = new();
        foreach (var upgrade in item.StatUpgrades)
        {
            string name = upgrade.Key.Replace("_", " ").ToUpper();
            upgradesText.AppendLine($"{name}: +{upgrade.Value}");
        }

        if (_statLabel != null)
        {
            _statLabel.Text = upgradesText.ToString();
        }

        if (_buyButton != null)
        {
            _buyButton.Disabled = false;
            _buyButton.Text = "Buy";
        }
    }

    private void OnBuyPressed()
    {
        if (_purchased || _item == null) return;

        var player = GetTree().GetFirstNodeInGroup("player") as Player;
        if (player == null || player.Stats == null) return;

        var stats = player.Stats;
        if (!stats.CanAfford(_effectivePrice))
        {
            AudioSynthManager.PlayPlayerHurt();
            FloatingText.Spawn(GetParent() ?? this, GlobalPosition, "Not enough gold!", new Color(1f, 0.3f, 0.3f), 14f);
            return;
        }

        if (stats.SpendGold(_effectivePrice))
        {
            _purchased = true;
            foreach (var upgrade in _item.StatUpgrades)
            {
                stats.ApplyUpgrade(upgrade.Key, upgrade.Value);
            }

            AudioSynthManager.PlayBuy();
            FloatingText.Spawn(GetParent() ?? this, GlobalPosition, "PURCHASED!", new Color(0.3f, 0.95f, 0.4f), 16f);

            if (_buyButton != null)
            {
                _buyButton.Disabled = true;
                _buyButton.Text = "BOUGHT";
            }
        }
    }
}