# Shop

NightFall currently has a data-driven shop display, but not a complete purchase and ownership system.

## Current Flow

```text
Data/Shop/ShopItems.json
  ↓
ShopManager
  ↓
Three ShopItem scene instances
```

## Core Types

### `ItemData`

`ItemData` is a plain C# data object used for JSON deserialization.

Current fields:

- `Id`
- `Name`
- `Price`
- `StatUpgrades`

`StatUpgrades` is a `Dictionary<string, float>`.

### `ShopManager`

`ShopManager` loads the JSON file, chooses three unique items at random, and pushes them into the three exported shop item slots.

Current behavior:

- Reads `res://Data/Shop/ShopItems.json`
- Deserializes it with `System.Text.Json`
- Randomly selects up to three unique items
- Calls `SetItem(...)` on the configured `ShopItem` nodes

There is no purchase handling in `ShopManager` yet.

### `ShopItem`

`ShopItem` is currently a presentation widget.

It fills in:

- Name
- Price
- Upgrade text

The `BuyButton` exists in the scene, but it is not wired to gameplay behavior in the current implementation.

## Data File

The current shop catalog lives in:

```text
Data/Shop/ShopItems.json
```

The file contains item definitions such as:

- `health_potion`
- `iron_ring`
- `sharp_blade`
- `swift_boots`
- `hunter_gloves`
- `lucky_coin`
- `vampiric_charm`
- `heavy_armor`
- `berserker_belt`
- `dash_core`

## Player Interaction

`PlayerStats` already has gold helpers:

- `AddGold`
- `CanAfford`
- `SpendGold`

Those methods are not yet connected to the shop purchase flow.

## Extension Guidance

- Keep item data in JSON or another data layer
- Keep purchase rules in a gameplay system, not in UI labels
- If item upgrades should affect player stats, add a dedicated application step instead of treating the JSON key as automatically meaningful

## Current Limitation

The shop is a display and selection system right now, not a complete economy system.
