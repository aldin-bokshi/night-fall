using System.Collections.Generic;

namespace NightFall.Data.Shop;

public class ItemData
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Rarity { get; set; }

    public int Price { get; set; }

    public IDictionary<string, float> StatUpgrades { get; set; } =
        new Dictionary<string, float>(System.StringComparer.Ordinal);
}