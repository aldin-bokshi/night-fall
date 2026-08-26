# Shop

NightFall has a complete shop system: data-driven items are loaded, displayed, and can be purchased with gold. Purchases apply stat upgrades to the player immediately.

## Current Flow

```text
Data/Shop/ShopItems.json
  ↓
ShopManager (loads JSON, picks 3 unique items)
  ↓
Three ShopItem scene instances (display + buy buttons)
  ↓
ShopItem.OnBuyPressed → PlayerStats.SpendGold + ApplyUpgrade
```

The shop is opened from the world through `ShopTrigger`.

## Opening The Shop

`ShopTrigger` is an `Area2D` placed in the world (e.g. `ShopAltar` in `Hub.tscn`). When the player (group `"player"`) enters, a prompt appears and the `[E] OPEN SHOP` floating text spawns. Pressing `ui_accept` while inside:

- Instantiates `Scenes/Shop/Shop.tscn` once and reuses the instance
- Adds it to the scene's `UI` (or `HUD`) `CanvasLayer` (fallback: scene root)
- Plays `buy` audio and pauses the tree (`GetTree().Paused = true`)

When the player presses `Leave` in the shop UI, the shop hides and the tree is unpaused.

## Core Types

### `ItemData`

`ItemData` is a plain C# data object used for JSON deserialization.

Current fields:

- `Id`
- `Name`
- `Rarity`
- `Price`
- `StatUpgrades` (`Dictionary<string, float>`)

It lives in `Data/Shop/ItemData.cs` under the `NightFall.Data.Shop` namespace.

## Reroll

The shop supports rerolling its three item slots.

Each shop grants **3 rerolls**, tracked per `ShopManager` instance (`_rerollsRemaining`, reset via field initializer). Since each `ShopTrigger` instantiates and caches its own `Shop` scene the first time it's opened, this means 3 rerolls per distinct shop altar, not per visit if you leave and reopen the same altar — closing/reopening currently doesn't regenerate items or reset rerolls (pre-existing behavior, unchanged by this feature).
Each reroll costs a fixed **150 credits**, deducted only after confirming `_rerollsRemaining > 0` and `PlayerStats.CanAfford(150)`.
A successful reroll calls the same `GenerateShopItems()` used on shop entry, so it keeps the existing unique-item-type-per-visit behavior. Duplicate items *across* separate rerolls are allowed.
`DisplayShopItems()` re-runs after reroll, and `ShopItem.SetItem()` already resets each card's purchased/button state.
The `Reroll` button's label doubles as the remaining-reroll counter, e.g. `↻ REROLL // 150 CR (2/3)`, and is disabled whenever `_rerollsRemaining` is 0 or the player can't afford 150 credits — this is re-checked every frame (`ProcessMode.Always`, since the shop is open exactly when the tree is paused) so it also disables immediately after spending gold on an item purchase.
The top-of-shop `Gold`/credits label (previously unused/static) is now kept in sync with `PlayerStats.Gold` on shop open and after each reroll.

### `ShopManager`

`ShopManager` loads the JSON file, chooses three unique items at random, and pushes them into the three exported shop item slots.

Current behavior:

- Reads `res://Data/Shop/ShopItems.json`
- Deserializes it with `System.Text.Json` (case-insensitive)
- Randomly selects three unique items
- Calls `SetItem(...)` on the configured `ShopItem` nodes
- Wires the `Leave` button (`../Leave`) to hide the shop root and unpause the tree

`ShopManager` does not handle per-item purchases; that lives in `ShopItem`.

### `ShopItem`

`ShopItem` is a `Control` card that displays one item and manages its purchase.

It fills in:

- Name
- Price (after `Greed` modifier scaling)
- Upgrade text (from each `StatUpgrades` key, e.g. `max_health` → `MAX HEALTH: +20`)

Purchase flow:

```text
BuyButton pressed
  → PlayerStats.CanAfford(price)?
      → No: PlayPlayerHurt + "Not enough gold!" floating text
      → Yes: SpendGold(price)
        → Apply each StatUpgrades via PlayerStats.ApplyUpgrade()
        → PlayBuy audio, "PURCHASED!" floating text
        → Disable button, set text to "BOUGHT"
```

The `Greed` run modifier increases all item prices by 50% (`price * 1.5`).

## Stat Upgrade Keys

The keys inside `StatUpgrades` are interpreted by `PlayerStats.ApplyUpgrade(string, float)`:

| Key | Effect |
| --- | --- |
| `max_health` | Increases `MaxHealth` and heals by the same amount |
| `damage` | Increases `AttackDamage` |
| `move_speed` | Increases `MoveSpeed` |
| `attack_speed` | Decreases `AttackCooldown` by `amount` percent (min 0.15s) |
| `defense` | Increases `Defense` |
| `lifesteal` | Increases `Lifesteal` |
| `luck` | Increases `Luck` |
| `cooldown` | Reduces `AttackCooldown` by 15% (min 0.15s) |

Unknown keys log `Unknown stat upgrade: {key}` and are ignored.

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

## Current Limitation

There is no inventory/ownership persistence between screens or runs — purchased upgrades apply to the live `PlayerStats` and are lost when the run ends.
</｜DSML｜tool>
<task_progress>

- [x] Explore documentation structure
- [x] Explore source code structure
- [x] Read existing documentation files
- [x] Audit docs against implementation
- [x] Verify remaining details (tres files, core services)
- [x] Update Room-Progression.md
- [x] Update Shop.md
- [ ] Update Combat.md
- [ ] Update Enemy.md
- [ ] Update Project-Architecture.md
- [ ] Update Scene-Architecture.md
- [ ] Update Player-Architecture.md
- [ ] Update Abilities.md
- [ ] Update UI.md
- [ ] Update Guides (Adding-Shop-Items, Creating-New-Abilities)
- [ ] Update Development (Debugging, Coding-Guidelines)
- [ ] Update Documentation.md index
- [ ] Create Systems/Run.md
- [ ] Create Systems/Core-Services.md
- [ ] Second audit & verification
</task_progress>
