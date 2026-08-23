# UI Architecture

NightFall UI is split into separate overlays and scene-local widgets. Active gameplay instances `Scenes/UI/UI.tscn` once at `Game/UI/HUD` under the screen-space `Game/UI` CanvasLayer.

## Main UI Pieces

### `PlayerUi`

`PlayerUi` is the in-game HUD for the player.

Current behavior:

- Finds the player through the `"player"` group
- Reads `PlayerStats`
- Updates health bar and text every frame
- Updates gold every frame, with a pulse tween when gold increases
- Updates run timer (`RunTracker.GetRunTimeSeconds`) and room/kill counters
- Displays active run modifiers (from `RunConfig`) or "STANDARD DESCENT"
- Rebuilds the ability widget list when the ability order or count changes

The ability widget list is created from the player’s live `AbilityManager` state.

The HUD is not parented to the Player; it locates the world Player through the existing `player` group.

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
- `ContinueButton` resumes the game
- `OptionsButton` opens an `OptionsMenu` instance
- `QuitButton` returns to the main menu (unpauses first)

### `DeathScreenOverlay`

`DeathScreenOverlay` is the game-over overlay.

Current behavior:

- Sets `ProcessMode` to `Always`
- Hides itself on ready
- Pauses the tree when shown
- Animates the red overlays and the content fade-in
- Shows real run stats passed from `Player.Die()` via `RunTracker` (rooms cleared, enemies slain, gold collected, run time)
- Picks a random death quote from `Data/DeathQuotes.json` via `DeathQuoteLoader`
- Reloads the current scene when retry is pressed

### `MainMenu`

`MainMenu` handles the startup menu buttons.

Current behavior:

- Start opens `Scenes/UI/SetupScreen/DungeonSetup.tscn`
- Options opens an `OptionsMenu` overlay instance
- Quit exits the game

### `OptionsMenu`

`OptionsMenu` is a `CanvasLayer` overlay opened from both the main menu and the pause menu.

Current behavior:

- Sets `ProcessMode` to `Always`
- Reads/writes `AudioSynthManager` volume levels (Master / SFX / Music)
- Toggles screen shake (`AudioSynthManager.ScreenShakeEnabled`)
- Toggles fullscreen via `DisplayServer`
- Closes with a close button

## UI Ownership Rules

- UI should display gameplay state, not own gameplay rules
- Gameplay scripts should not directly implement HUD presentation
- Overlays that must work while paused should use `ProcessModeEnum.Always`
- World-space gameplay belongs under `Game/World`; screen-space presentation belongs under `Game/UI`.

## Scene Notes

There are multiple UI scene variants in the repository:

- `Scenes/UI/MainMenu/MainMenu.tscn`
- `Scenes/UI/MainMenu/MainMenu2.tscn`
- `Scenes/UI/PauseMenu/PauseMenu.tscn`
- `Scenes/UI/PauseMenu/PauseMenu2.tscn`

They share the same underlying scripts but use different layouts or styling.
