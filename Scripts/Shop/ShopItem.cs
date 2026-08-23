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
        _buyButton ??=
            GetNodeOrNull<Button>(
                "Panel/MarginContainer/VBoxContainer/BottomRow/BuyButton"
            );

        _statUpgradesContainer ??=
            GetNodeOrNull<VBoxContainer>(
                "Panel/MarginContainer/VBoxContainer/StatScroll/StatUpgradesContainer"
            );

        _rarityLabel ??=
            GetNodeOrNull<Label>(
                "Panel/MarginContainer/VBoxContainer/RarityLabel"
            );

        _nameLabel ??=
            GetNodeOrNull<Label>(
                "Panel/MarginContainer/VBoxContainer/NameLabel"
            );

        _priceLabel ??=
            GetNodeOrNull<Label>(
                "Panel/MarginContainer/VBoxContainer/BottomRow/PriceLabel"
            );

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

        if (_buyButton != null)
        {
            _buyButton.Disabled = false;
            _buyButton.Text = "ACQUIRE";
        }
    }

    private void SetRarity(ItemData item)
    {
        if (_rarityLabel == null) return;

        string rarity = GetRarityText(item);

        _rarityLabel.Text = $"◆ {rarity.ToUpper()}";

        _rarityLabel.AddThemeColorOverride(
            "font_color",
            GetRarityColor(rarity)
        );
    }

    private string GetRarityText(ItemData item)
    {
        // Uses the Rarity property from ItemData.
        // Falls back to Common if the value is empty.
        return string.IsNullOrWhiteSpace(item.Rarity)
            ? "Common"
            : item.Rarity;
    }

    private Color GetRarityColor(string rarity)
    {
        return rarity.ToLower() switch
        {
            "common" => new Color(0.55f, 0.5f, 0.62f),
            "uncommon" => new Color(0.35f, 0.75f, 0.55f),
            "rare" => new Color(0.35f, 0.55f, 0.95f),
            "epic" => new Color(0.7f, 0.4f, 0.95f),
            "legendary" => new Color(1f, 0.65f, 0.25f),
            _ => new Color(0.5f, 0.42f, 0.62f)
        };
    }

    private void CalculatePrice(ItemData item)
    {
        _effectivePrice = item.Price;

        var runConfig = RunSession.Current;

        if (runConfig is { Greed: true }) _effectivePrice = (int)(_effectivePrice * 1.5f);

        if (_priceLabel != null) _priceLabel.Text = $"{_effectivePrice} CR";
    }

    private void DisplayStatUpgrades(ItemData item)
    {
        if (_statUpgradesContainer == null) return;

        foreach (Node child in _statUpgradesContainer.GetChildren())
        {
            child.QueueFree();
        }

        if (item.StatUpgrades.Count == 0)
        {
            Label emptyLabel = CreateStatLabel("NO STAT UPGRADES");

            emptyLabel.AddThemeColorOverride(
                "font_color",
                new Color(0.38f, 0.35f, 0.43f)
            );

            _statUpgradesContainer.AddChild(emptyLabel);
            return;
        }

        foreach (var upgrade in item.StatUpgrades)
        {
            string statName = FormatStatName(upgrade.Key);
            string value = FormatStatValue(upgrade.Key, upgrade.Value);

            Label statLabel = CreateStatLabel(
                $"{statName}    {value}"
            );

            _statUpgradesContainer.AddChild(statLabel);
        }
    }

    private Label CreateStatLabel(string text)
    {
        Label label = new()
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            CustomMinimumSize = new Vector2(210, 17)
        };

        label.AddThemeFontSizeOverride("font_size", 12);

        label.AddThemeColorOverride(
            "font_color",
            new Color(0.65f, 0.62f, 0.71f)
        );

        return label;
    }

    private string FormatStatName(string statName)
    {
        return statName
            .Replace("_", " ")
            .ToUpper();
    }

    private string FormatStatValue(string statName, float value)
    {
        string lowerName = statName.ToLower();

        // Percentage-based stats.
        if (
            lowerName.Contains("cooldown") ||
            lowerName.Contains("chance") ||
            lowerName.Contains("percent")
        ) return $"{value:+0;-0}%";

        // Normal numerical stats.
        return $"{value:+0;-0}";
    }

    private void OnBuyPressed()
    {
        if (_purchased || _item == null) return;

        Player? player =
            GetTree().GetFirstNodeInGroup("player") as Player;

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
                14f
            );

            return;
        }

        if (!stats.SpendGold(_effectivePrice)) return;

        _purchased = true;

        foreach (var upgrade in _item.StatUpgrades)
        {
            stats.ApplyUpgrade(
                upgrade.Key,
                upgrade.Value
            );
        }

        AudioSynthManager.PlayBuy();

        FloatingText.Spawn(
            GetParent() ?? this,
            GlobalPosition,
            "PURCHASED!",
            new Color(0.3f, 0.95f, 0.4f),
            16f
        );

        if (_buyButton != null)
        {
            _buyButton.Disabled = true;
            _buyButton.Text = "SOLD OUT";
        }
    }
}