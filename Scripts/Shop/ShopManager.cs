using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using NightFall.Data.Shop;

namespace NightFall.Scripts.Shop;

public partial class ShopManager : Node
{
    private List<ItemData>? _allItems;
    private readonly List<ItemData> _currentItems = [];

    [Export] private ShopItem? _shopItem1;
    [Export] private ShopItem? _shopItem2;
    [Export] private ShopItem? _shopItem3;

    public override void _Ready()
    {
        LoadItems();
        GenerateShopItems();

        if (_shopItem1 != null && _currentItems.Count > 0)
            _shopItem1.SetItem(_currentItems[0]);
        if (_shopItem2 != null && _currentItems.Count > 1)
            _shopItem2.SetItem(_currentItems[1]);
        if (_shopItem3 != null && _currentItems.Count > 2)
            _shopItem3.SetItem(_currentItems[2]);
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
        if (_allItems == null || _allItems.Count == 0)
            return;

        Random random = new();
        
        while (_currentItems.Count < 3)
        {
            int randomIndex = random.Next(0, _allItems.Count);
            ItemData item = _allItems[randomIndex];

            if (!_currentItems.Contains(item)) { _currentItems.Add(item); }
        }
    }
}