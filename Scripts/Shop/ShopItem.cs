using Godot;
using NightFall.Data.Shop;

namespace NightFall.Scripts.Shop;

public partial class ShopItem : Control
{
    [Export] private Label? _nameLabel;
    [Export] private Label? _priceLabel;
    [Export] private Label? _statLabel;
    private string _allUpgrades = "";

    public void SetItem(ItemData item)
    {
        if (_nameLabel != null) _nameLabel.Text = item.Name;
        GD.Print(item.Name);

        // _priceLabel.Text = $"{item.Price}";
        if (_priceLabel != null) _priceLabel.Text = item.Price.ToString();
        GD.Print(item.Price);

        foreach (var upgrade in item.StatUpgrades)
        {
            _allUpgrades += $"{upgrade.Key}: {upgrade.Value:+#;-#;0}\n";
            GD.Print($"{upgrade.Key}: {upgrade.Value}");
        }

        if (_statLabel != null) _statLabel.Text = _allUpgrades;
    }
}