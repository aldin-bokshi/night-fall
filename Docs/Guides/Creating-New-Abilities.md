# Creating New Abilities

This guide matches the current ability implementation.

## Current Pattern

Abilities are node instances under `AbilityManager`, not loose scripts that are spawned from a factory.

```text
Player
  └── AbilityManager
        └── YourAbility
```

## Recommended Workflow

### 1. Copy The Scaffold

Duplicate:

- `Scripts/Entities/Player/Abilities/AbilityTemplate.cs`
- `Data/Abilities/AbilityTemplate.tres`

Rename the copies to your new ability name.

### 2. Create A New Ability Class

Make the new class inherit from `Ability`.

Put ability-specific gameplay in `Use()`.

If the ability needs the player, cache it in `_Ready()` the same way `BlinkAbility` does.

### 3. Create The Ability Resource

Create a new `AbilityData` resource and set:

- Ability name
- Cooldown duration
- Input display text
- Input action string
- Icon if needed

### 4. Add The Ability To The Player Scene

Add a new child node under `AbilityManager` in `Scenes/Entities/Player/Player.tscn`.

Assign:

- The new ability script
- The new `AbilityData` resource

### 5. Bind An Input Action

If you need a new action, add it in `project.godot`.

The current input system checks action strings, not direct key codes.

### 6. Verify The HUD

`PlayerUi` will automatically create a HUD widget for every active ability if the ability exists under `AbilityManager`.

If you want custom display behavior, update `AbilityUi`.

## Important Rules

- Keep targeting and gameplay in the ability class
- Keep input polling in `PlayerInput`
- Keep cooldown timing in `Ability`
- Keep HUD presentation in `AbilityUi`

## Blink As Reference

`BlinkAbility` shows the current pattern:

- Reads player movement or facing
- Performs collision-safe targeting
- Moves the player
- Starts cooldown only after a successful use

Use it as an implementation reference, not as a mandatory template for all abilities.

## Testing

The quickest way to test a new ability is:

1. Launch `Scenes/Dungeon/Dev/TestWorld.tscn`
2. Press the bound input action
3. Watch the HUD cooldown and the gameplay result

If the ability does not appear in the HUD, confirm that it is a direct child of `AbilityManager`.
