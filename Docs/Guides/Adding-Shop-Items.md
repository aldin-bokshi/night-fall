# Adding Shop Items

This guide matches the current shop implementation.

## Current Pattern

Shop items are defined in JSON, loaded by `ShopManager`, displayed through a reusable `ShopItem` scene, and purchased with gold.

```text
Data/Shop/ShopItems.json
  ↓
ShopManager
  ↓
ShopItem scene instances
  ↓
ShopItem.OnBuyPressed → PlayerStats.SpendGold + ApplyUpgrade
```

## Add A New Item

Add a new object to `Data/Shop/ShopItems.json` with the same shape as the existing entries:

- `id`
- `name`
- `rarity`
- `price`
- `statUpgrades`

Example:

```json
{
  "id": "new_item",
  "name": "New Item",
  "rarity": "common",
  "price": 50,
  "statUpgrades": {
    "damage": 3
  }
}
```

## Data Rules

- Keep `id` unique
- Keep `price` as an integer
- Keep `statUpgrades` as a JSON object of string keys and numeric values

## Stat Upgrade Keys

The keys inside `statUpgrades` are interpreted by `PlayerStats.ApplyUpgrade`:

- `max_health`
- `damage`
- `move_speed`
- `attack_speed`
- `defense`
- `lifesteal`
- `luck`
- `dash_cooldown`

Negative values are supported (e.g. `heavy_armor` uses `move_speed: -5`). Unknown keys log a warning and are ignored. See [Shop System](../Systems/Shop.md) for the exact effects.

## Current Display Flow

`ShopManager` randomly selects three unique entries and assigns them to the three exported shop item slots.

`ShopItem` then writes:

- Name
- Price (after `Greed` modifier scaling, ×1.5)
- Stat upgrade text

## Purchase Flow

Press `BuyButton`:

1. If the player cannot afford the price: `player_hurt` audio + "Not enough gold!" floating text
2. If affordable: `SpendGold(price)`, apply each upgrade via `ApplyUpgrade`, play `buy` audio, show "PURCHASED!" text, disable the button and set its text to "BOUGHT"

## Where The Shop Opens From

The shop opens from the world through `ShopTrigger` (`Area2D`). The player presses `ui_accept` while inside the trigger to open `Scenes/Shop/Shop.tscn`. The shop instance is reused; the tree pauses while open, and `Leave` hides it and unpauses. See [Shop System](../Systems/Shop.md).

## Testing

- Add an item to `Data/Shop/ShopItems.json`
- Open `Scenes/Dungeon/Dev/TestWorld.tscn`, earn gold (kill enemies or clear a wave), then open the shop and verify the item card, price, and purchase
- Verify the `Greed` ×1.5 price multiplier and `PlayerStats.ApplyUpgrade` effects

## Not Implemented

Inventory/ownership persistence between runs — purchased upgrades apply to the live `PlayerStats` only.