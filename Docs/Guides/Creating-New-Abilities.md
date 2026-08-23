# Creating New Abilities

This guide matches the current ability implementation.

## Current Pattern

Abilities are node instances under `AbilityManager`, not loose scripts spawned from a factory.

```text
Player
  └── AbilityManager
        └── YourAbility
```

`AbilityManager` assigns slots by the **child order** of `Ability` nodes in the scene. There are exactly four slots (`AbilitySlotCount = 4`), each bound to a fixed input action and displayed key:

```text
Slot 0 → ability_1 → Shift
Slot 1 → ability_2 → Q
Slot 2 → ability_3 → E
Slot 3 → ability_4 → R
```

You cannot define per-ability input actions. To give an ability a keybind, place it in the slot whose action you want.

## Recommended Workflow

### 1. Copy The Scaffold

Duplicate:

- `Scripts/Entities/Player/Abilities/AbilityTemplate.cs`
- `Data/Abilities/AbilityTemplate.tres`

Rename the copies to your new ability name.

### 2. Create A New Ability Class

Make the new class inherit from `Ability`.

Put ability-specific gameplay in `Use()`.

If the ability needs the player, cache it in `_Ready()` the same way `BlinkAbility` and `GravityWellAbility` do:

```csharp
public override void _Ready()
{
    base._Ready();
    _player = GetParent().GetParent<Player>();
    if (_player == null) GD.PushError("YourAbility could not find the Player.");
}
```

### 3. Create The Ability Resource

Create a new `AbilityData` resource. `AbilityData` only supports:

- `AbilityName`
- `Icon` (optional `Texture2D`)
- `CooldownDuration` (seconds, 0–60)

There is no input display text or input action field in `AbilityData`. The HUD derives the keybind from the slot index.

### 4. Add The Ability To The Player Scene

Add a new child node under `AbilityManager` in `Scenes/Entities/Player/Player.tscn`.

Assign:

- The new ability script
- The new `AbilityData` resource

Child order determines the slot. `MaxAbilities` defaults to 4; abilities beyond the slot limit are ignored with a warning.

### 5. No New Input Action Required

The existing `ability_1` through `ability_4` actions in `project.godot` already cover all four slots. Placing your ability in a slot is sufficient; `PlayerInput` polls those four fixed actions.

### 6. Verify The HUD

`PlayerUi` will automatically create a HUD widget for every active ability if the ability exists under `AbilityManager`.

If you want custom display behavior, update `AbilityUi`.

## Important Rules

- Keep targeting and gameplay in the ability class
- Keep input polling in `PlayerInput`
- Keep cooldown timing in `Ability`
- Keep HUD presentation in `AbilityUi`

## Existing Abilities As References

The current player scene has:

- `GravityWellAbility` (slot 0) — spawns a projectile to the mouse position (`GetGlobalMousePosition()`), which becomes a pulling `GravityWell`; stats come from `PlayerStats` gravity-well group
- `BlinkAbility` (slot 1) — reads player movement/facing, raycasts, moves the player safely, plays FX, starts cooldown only on success

Use them as implementation references, not as a mandatory template for all abilities.

## Testing

The quickest way to test a new ability is:

1. Launch `Scenes/Dungeon/Dev/TestWorld.tscn`
2. Press the bound input action for the slot you placed the ability in
3. Watch the HUD cooldown and the gameplay result

If the ability does not appear in the HUD, confirm that it is a direct child of `AbilityManager` and within the first four children.