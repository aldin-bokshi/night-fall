using System.Collections.Generic;
using System.Text.Json;
using Godot;
using NightFall.Data.Shop;

namespace NightFall.Scripts.Shop;

public partial class ShopManager : Node
{
    private List<ItemData> _allItems;

    public override void _Ready()
    {
        LoadItems();
    }

    private void LoadItems()
    {
        string json = FileAccess.GetFileAsString(
            "res://Data/Shop/ShopItems.json"
        );

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        _allItems = JsonSerializer.Deserialize<List<ItemData>>(
            json,
            options
        );
    }
}