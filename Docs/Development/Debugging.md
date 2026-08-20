# Debugging Guide

These are the most relevant NightFall-specific things to check when something stops working.

## Scene Or Node Name Mismatches

Many scripts use hard-coded `GetNode(...)` paths.

If a scene is renamed or restructured, check:

- `Player.tscn`
- `Enemy.tscn`
- `PlayerUi.tscn`
- `DeathScreen.tscn`
- `PauseMenu.tscn`
- `Shop.tscn`

Common symptoms:

- Null reference errors
- Missing UI labels
- Combat no longer activating
- Death screen not appearing

## Ability Problems

If an ability does not fire:

- Confirm the ability node is a direct child of `AbilityManager`
- Confirm the ability is in the expected slot order
- Confirm the corresponding `ability_1` through `ability_4` Input Map action exists
- Confirm the player input code is actually polling that slot

For `BlinkAbility`, also confirm:

- The player has a valid `Hurtbox/CollisionShape2D`
- The player is in a scene that allows the raycast and shape checks to run

## Combat Problems

If attacks do not deal damage:

- Check collision layers and masks
- Check that the enemy still has an `EnemyStats` node
- Check that the player still has a `PlayerStats` node
- Check that the attack hitbox is activating
- Check that the target hurtbox name still matches what the combat code expects

Important detail:

- Enemy attacks only damage areas named `Hurtbox`
- Player attacks look for an enemy root with `EnemyStats`

## Pause Problems

Pause menus and death overlays must keep working while the tree is paused.

If the menu stops responding:

- Confirm the canvas layer script sets `ProcessModeEnum.Always`
- Confirm the input action name is still `pause`
- Confirm another node is not consuming the input first

## Shop Problems

If shop items do not appear correctly:

- Confirm `Data/Shop/ShopItems.json` is valid JSON
- Confirm the three exported shop item slots are assigned in the scene
- Confirm the item data keys match the JSON property names

If buying does nothing, that is currently expected: purchasing is not implemented yet.

## Room Problems

If room activation does nothing, that is also currently expected.

`RoomManager` only hooks the activation zone right now. The actual activation logic is still a placeholder.

## Build Or Project File Problems

One current footgun in the repository:

- `NightFall.csproj` includes `Scripts/Shop/ShopItemDatabase.json`
- The actual shop data file is `Data/Shop/ShopItems.json`

If you see a project file or content-import warning, check that mismatch first.

## Godot C# Sync Issues

If Godot stops seeing a script:

- Make sure the class name matches the file and scene reference
- Make sure the namespace has not changed unexpectedly
- Regenerate project files if needed
- Verify the `uid://` and `res://` references in the scene still point at real files

## Death Screen Problems

If the death screen never appears:

- Confirm the current scene contains a node named `DeathScreen`
- Confirm the player is actually dead
- Confirm the current scene has not been changed to one without the overlay

## Good First Checks

When debugging a broken feature, check these in order:

1. Scene node names
2. Exported references
3. Input actions
4. Collision layers and masks
5. Data file contents
6. Whether the feature is actually implemented yet
