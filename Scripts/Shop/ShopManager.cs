using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using NightFall.Data.Shop;

namespace NightFall.Scripts.Shop;

public partial class ShopManager : Node
{
    private const string ShopItemsPath = "res://Data/Shop/ShopItems.json";
    private const int ShopItemCount = 3;

    private List<ItemData>? _allItems;
    private readonly List<ItemData> _currentItems = [];

    [Export] private ShopItem? _shopItem1;
    [Export] private ShopItem? _shopItem2;
    [Export] private ShopItem? _shopItem3;
    [Export] private Button? _leaveButton;

    public override void _Ready()
    {
        LoadItems();
        GenerateShopItems();
        DisplayShopItems();

        _leaveButton ??= GetParent().GetNodeOrNull<Button>("Leave");

        if (_leaveButton != null) _leaveButton.Pressed += OnLeavePressed;
        else GD.PushWarning("ShopManager: Leave button could not be found.");
    }

    private void DisplayShopItems()
    {
        if (_shopItem1 != null && _currentItems.Count > 0) _shopItem1.SetItem(_currentItems[0]);

        if (_shopItem2 != null && _currentItems.Count > 1) _shopItem2.SetItem(_currentItems[1]);

        if (_shopItem3 != null && _currentItems.Count > 2) _shopItem3.SetItem(_currentItems[2]);
    }

    private void OnLeavePressed()
    {
        GetTree().Paused = false;

        if (GetParent() is Control shopRoot) shopRoot.Hide();
    }

    private void LoadItems()
    {
        if (!FileAccess.FileExists(ShopItemsPath))
        {
            GD.PushError(
                $"ShopManager: Shop item file not found: {ShopItemsPath}"
            );

            return;
        }

        string json = FileAccess.GetFileAsString(ShopItemsPath);

        if (string.IsNullOrWhiteSpace(json))
        {
            GD.PushError("ShopManager: Shop item file is empty.");
            return;
        }

        JsonSerializerOptions options = new(){PropertyNameCaseInsensitive = true};

        try
        {
            _allItems = JsonSerializer.Deserialize<List<ItemData>>(
                json,
                options
            );
        }
        catch (JsonException exception)
        {
            GD.PushError(
                $"ShopManager: Failed to parse shop items: {exception.Message}"
            );
        }
    }

    private void GenerateShopItems()
    {
        _currentItems.Clear();

        if (_allItems == null || _allItems.Count == 0)
        {
            GD.PushWarning("ShopManager: No shop items are available.");
            return;
        }

        int itemCount = Math.Min(ShopItemCount, _allItems.Count);

        Random random = new();

        while (_currentItems.Count < itemCount)
        {
            int randomIndex = random.Next(_allItems.Count);
            ItemData item = _allItems[randomIndex];

            if (!_currentItems.Contains(item)) _currentItems.Add(item);
        }
    }

    public override void _ExitTree()
    {
        if (_leaveButton != null) _leaveButton.Pressed -= OnLeavePressed;
    }
}