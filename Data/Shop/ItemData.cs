using System.Collections.Generic;

namespace NightFall.Data.Shop;

public class ItemData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public Dictionary<string, float> StatUpgrades { get; set; }
}