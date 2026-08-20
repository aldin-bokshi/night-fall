# Abilities

NightFall abilities are node-based gameplay objects that live under the player’s `AbilityManager`.

## Current Ability Stack

```text
Player
  └── AbilityManager
        └── Ability instances
```

The current player scene instantiates two abilities:

- `BlinkAbility`
- `GravityWellAbility`

`AbilityTemplate.cs` and `Data/Abilities/AbilityTemplate.tres` are scaffold files for creating additional abilities.

## Core Types

### `Ability`

`Ability` is the base class for all abilities.

It currently provides:

- An exported `AbilityData` resource
- A cooldown timer
- `Use()` gating through `IsReady`
- Per-frame cooldown ticking in `_Process`

The base class does not implement targeting or game-specific effects.

### `AbilityData`

`AbilityData` is a `Resource` used for ability presentation and ability configuration.

Current fields:

- `AbilityName`
- `Icon`
- `CooldownDuration`

These are presentation and balance values only. The current gameplay and HUD derive keybinds from slot order, not from ability metadata.

### `AbilityManager`

`AbilityManager` owns the active ability nodes.

Important behavior:

- Only direct child `Ability` nodes are considered active
- `MaxAbilities` limits the active list, but the system also clamps to four slots
- `RefreshAbilities()` rebuilds the list from the current children
- `TryUseAbility(slotIndex)` activates the ability in that slot
- `AbilityManager` also owns the slot-to-input and slot-to-key mapping
- `TryAddAbility()` and `RemoveAbility()` can manage ability nodes at runtime

There is no separate unlock database or persistence layer in the current implementation.

### `AbilityUi`

`AbilityUi` is the HUD representation of one ability.

It displays:

- Name
- Input label
- Optional icon
- Cooldown ring
- Cooldown text

It reads directly from the live ability instance every frame.

### `BlinkAbility`

`BlinkAbility` is the current concrete ability in the player scene.

Current behavior:

- Uses the player’s current movement input when available
- Falls back to facing direction if movement input is neutral
- Raycasts toward the target direction
- Uses the player hurtbox shape to avoid blinking into blocked space
- Moves the player instantly to the safe target position
- Starts its cooldown after a successful blink

The ability is currently configured by:

- The `BlinkAbility` script
- `Data/Abilities/BlinkAbility.tres`
- The `BlinkAbility` node in `Player.tscn`

### `GravityWellAbility`

`GravityWellAbility` spawns a projectile toward the mouse position.

When the projectile reaches its target, it creates a transient `GravityWell` node that:

- Lives for a fixed duration
- Scans the `"enemy"` group every physics frame
- Applies pull force to enemies within its radius

The gravity-well chain is currently configured by:

- `GravityWellAbility.cs`
- `GravityWellProjectile.cs`
- `GravityWell.cs`
- `Data/Abilities/GravityWell.tres`
- The `GravityWellAbility` node in `Player.tscn`

## How Input Reaches an Ability

```text
project.godot InputMap
  ↓
PlayerInput
  ↓
slot index
  ↓
AbilityManager
  ↓
Ability.Use()
```

The slot order determines which input action is checked and which key the HUD displays:

```text
Slot 0 -> Shift -> ability_1
Slot 1 -> Q     -> ability_2
Slot 2 -> E     -> ability_3
Slot 3 -> R     -> ability_4
```

## How The HUD Finds Abilities

`PlayerUi` asks the player’s `AbilityManager` for the current ability list, then instantiates one `AbilityUi` per active ability.

```text
AbilityManager.Abilities
  ↓
PlayerUi
  ↓
AbilityUi instances
```

## Current Ability Configuration

The existing blink resource uses:

- `AbilityName = "BLINK"`
- `CooldownDuration = 5.0`

The gravity-well resource uses:

- `AbilityName = "GRAVITY WELL"`
- `CooldownDuration = 8.0`

## Creating A New Ability

See [Creating New Abilities](../Guides/Creating-New-Abilities.md).

## What Abilities Should Not Own

Abilities should not be responsible for:

- Rendering the player HUD
- Reading raw input directly from the keyboard
- Owning global game state
- Replacing the player entity
- Implementing unrelated combat or shop logic

## Known Current Constraints

- Ability nodes are authored directly in the player scene
- Ability registration is based on node parenting and slot order
- There is no saved unlock progression yet
