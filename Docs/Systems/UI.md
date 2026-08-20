# UI Architecture

NightFall UI is split into a few separate overlays and scene-local widgets.

## Main UI Pieces

### `PlayerUi`

`PlayerUi` is the in-game HUD for the player.

Current behavior:

- Finds the player through the `"player"` group
- Reads `PlayerStats`
- Updates health and gold every frame
- Rebuilds the ability widget list when the ability order or count changes

The ability widget list is created from the player’s live `AbilityManager` state.

### `AbilityUi`

`AbilityUi` displays one ability entry inside the HUD.

It reads:

- The live ability instance
- `AbilityData.AbilityName`
- `AbilityData.Icon`
- The live cooldown progress from the ability instance

The displayed keybind comes from the ability slot, not from `AbilityData`.

### `PauseMenu`

`PauseMenu` is a `CanvasLayer` overlay that can pause and resume the tree.

Current behavior:

- Sets `ProcessMode` to `Always`
- Hides itself on ready
- Toggles pause when the `pause` action is pressed in `_UnhandledInput`

The scene files contain button controls, but the current script only handles input-driven toggling.

### `DeathScreenOverlay`

`DeathScreenOverlay` is the game-over overlay.

Current behavior:

- Sets `ProcessMode` to `Always`
- Hides itself on ready
- Pauses the tree when shown
- Animates the red overlays and the content fade-in
- Shows placeholder run stats
- Picks a random death quote
- Reloads the current scene when retry is pressed

### `MainMenu`

`MainMenu` handles the startup menu buttons.

Current behavior:

- Start launches `Scenes/Dungeon/Dev/TestWorld.tscn`
- Quit exits the game

## UI Ownership Rules

- UI should display gameplay state, not own gameplay rules
- Gameplay scripts should not directly implement HUD presentation
- Overlays that must work while paused should use `ProcessModeEnum.Always`

## Scene Notes

There are multiple UI scene variants in the repository:

- `Scenes/UI/MainMenu/MainMenu.tscn`
- `Scenes/UI/MainMenu/MainMenu2.tscn`
- `Scenes/UI/PauseMenu/PauseMenu.tscn`
- `Scenes/UI/PauseMenu/PauseMenu2.tscn`

They share the same underlying scripts but use different layouts or styling.
