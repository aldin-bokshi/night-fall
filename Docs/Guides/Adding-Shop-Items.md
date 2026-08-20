# Adding Shop Items

This guide matches the current shop implementation.

## Current Pattern

Shop items are defined in JSON and displayed through a reusable `ShopItem` scene.

```text
Data/Shop/ShopItems.json
  ↓
ShopManager
  ↓
ShopItem scene instances
```

## Add A New Item

Add a new object to `Data/Shop/ShopItems.json` with the same shape as the existing entries:

- `id`
- `name`
- `price`
- `statUpgrades`

Example:

```json
{
  "id": "new_item",
  "name": "New Item",
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

The key names inside `statUpgrades` are currently just data labels. They are not automatically interpreted by gameplay systems yet.

## Current Display Flow

`ShopManager` randomly selects three unique entries and assigns them to the three exported shop item slots.

`ShopItem` then writes:

- Name
- Price
- Stat upgrade text

## What Is Not Implemented Yet

The current shop does not yet:

- Deduct gold
- Mark an item as purchased
- Apply stat upgrades to the player
- Track inventory ownership
- Disable the buy button after purchase

`PlayerStats` already has gold helper methods, but the purchase pipeline is not connected.

## Testing

Open the shop scene and verify:

- Three item cards appear
- The JSON loads without errors
- The displayed values match the data file

If the cards appear but the buy button does nothing, that is expected in the current implementation.
