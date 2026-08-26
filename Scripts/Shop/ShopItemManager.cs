using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using NightFall.Data.Shop;
using NightFall.Scripts.Core;

namespace NightFall.Scripts.Shop;

public partial class ShopItemManager : Node
{
    private const int ShopItemCount = 3;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private List<ItemData>? _allItems;
    private readonly List<ItemData> _currentItems = [];

    [Export] private ShopItem? _shopItem1;
    [Export] private ShopItem? _shopItem2;
    [Export] private ShopItem? _shopItem3;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
    }

    public void Initialize()
    {
        LoadItems();
        GenerateShopItems();
    }

    public void GenerateShopItems()
    {
        _currentItems.Clear();

        if (_allItems == null || _allItems.Count == 0)
        {
            GD.PushWarning(
                "ShopItemManager: No shop items are available.");

            return;
        }

        Dictionary<string, List<ItemData>> itemsByType =
            _allItems
                .GroupBy(
                    item => item.Type ?? "Unknown",
                    StringComparer.Ordinal)
                .ToDictionary(
                    group => group.Key,
                    group => group.ToList(),
                    StringComparer.Ordinal);

        List<string> availableTypes =
            [.. itemsByType.Keys];

        if (availableTypes.Count < ShopItemCount)
        {
            GD.PushWarning(
                $"ShopItemManager: Only {availableTypes.Count} item type(s) " +
                $"available, expected at least {ShopItemCount}. Shop will " +
                $"show fewer unique-type items than usual.");
        }

        Random random = new();

        int slotCount =
            Math.Min(ShopItemCount, availableTypes.Count);

        for (int i = 0; i < slotCount; i++)
        {
            int typeIndex =
                random.Next(availableTypes.Count);

            string chosenType =
                availableTypes[typeIndex];

            availableTypes.RemoveAt(typeIndex);

            List<ItemData> itemsOfType =
                itemsByType[chosenType];

            ItemData chosenItem =
                itemsOfType[random.Next(itemsOfType.Count)];

            _currentItems.Add(chosenItem);
        }

        DisplayShopItems();
    }

    private void DisplayShopItems()
    {
        if (_shopItem1 != null && _currentItems.Count > 0)
            _shopItem1.SetItem(_currentItems[0]);

        if (_shopItem2 != null && _currentItems.Count > 1)
            _shopItem2.SetItem(_currentItems[1]);

        if (_shopItem3 != null && _currentItems.Count > 2)
            _shopItem3.SetItem(_currentItems[2]);
    }

    private void LoadItems()
    {
        if (!FileAccess.FileExists(GamePaths.ShopItems))
        {
            GD.PushError(
                $"ShopItemManager: Shop item file not found: " +
                $"{GamePaths.ShopItems}");

            return;
        }

        string json =
            FileAccess.GetFileAsString(GamePaths.ShopItems);

        if (string.IsNullOrWhiteSpace(json))
        {
            GD.PushError(
                "ShopItemManager: Shop item file is empty.");

            return;
        }

        try
        {
            _allItems =
                JsonSerializer.Deserialize<List<ItemData>>(
                    json,
                    JsonOptions);
        }
        catch (JsonException exception)
        {
            GD.PushError(
                $"ShopItemManager: Failed to parse shop items: " +
                $"{exception.Message}");
        }
    }
}