using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using NightFall.Data.Shop;

namespace NightFall.Scripts.Shop;

public partial class ShopManager : Node
{
    private List<ItemData> _allItems;
    private List<ItemData> _currentItems = new();

    public override void _Ready()
    {
        LoadItems();
        GenerateShopItems();
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


    private void GenerateShopItems()
    {
        Random random = new();
        
        while (_currentItems.Count < 3)
        {
            int randomIndex = random.Next(0, _allItems.Count);
            ItemData item = _allItems[randomIndex];

            if (!_currentItems.Contains(item)) { _currentItems.Add(item); }
        }

        foreach (var item in _currentItems)
        {
            GD.Print($"{item.Name} - {item.Price}");
        }
    }
}