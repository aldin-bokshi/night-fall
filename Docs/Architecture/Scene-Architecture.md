# Scene Architecture

This document maps the important Godot scenes and how they relate to each other at runtime.

## Startup Flow

`project.godot` currently points the game to the main menu scene:

```text
project.godot
  ↓
Scenes/UI/MainMenu/MainMenu.tscn
  ↓
Scenes/Dungeon/Dev/TestWorld.tscn
```

The main menu button currently loads the dev test world directly.

## Important Scenes

### `Scenes/UI/MainMenu/MainMenu.tscn`

Current role:

- Startup menu
- Launches the current test gameplay scene

Important nodes:

- `MainMenu` root `Control`
- `StartButton`
- `OptionsButton`
- `QuitButton`

Important behavior:

- `StartButton` changes the scene to `Scenes/Dungeon/Dev/TestWorld.tscn`
- `QuitButton` exits the game

### `Scenes/Dungeon/Dev/TestWorld.tscn`

Current role:

- Developer test scene
- Used by the main menu as the current play target

Important contents:

- `World` root `Node2D`
- One `Enemy` instance
- One `Player` instance
- `HUD` instance from `Scenes/UI/UI.tscn`
- A tilemap layer for the placeholder environment

This is the best scene for validating player, enemy, combat, ability, and HUD changes together.

### `Scenes/Core/Game.tscn`

Current role:

- Reusable in-game shell scene
- Composes player, pause, and death UI under a single root

Important contents:

- `UI` `CanvasLayer`
- `PauseMenu` instance
- `DeathScreen` instance
- `Managers/PauseManager`
- `World/Player`

This scene is present and wired, but it is not the startup scene in `project.godot`.

### `Scenes/Entities/Player/Player.tscn`

Current role:

- Player entity composition

Important child nodes:

- `PlayerInput`
- `PlayerMovement`
- `PlayerCombat`
- `PlayerStats`
- `AbilityManager`
- `BlinkAbility`
- `AttackHitbox`
- `Hurtbox`
- `Camera2D`

### `Scenes/Entities/Enemies/Enemy.tscn`

Current role:

- Enemy entity composition

Important child nodes:

- `EnemyAi`
- `EnemyMovement`
- `EnemyStats`
- `EnemyCombat`
- `AttackHitbox`
- `Hurtbox`

### `Scenes/UI/UI.tscn`

Current role:

- HUD container used in the test world

Important children:

- `DeathScreen`
- `PauseScreen2`
- `PlayerUi`

### `Scenes/UI/PlayerUi/PlayerUi.tscn`

Current role:

- Player HUD panel

Important child areas:

- Health and gold display
- Ability bar container

### `Scenes/UI/DeathScreen/DeathScreen.tscn`

Current role:

- Game-over overlay

### `Scenes/UI/PauseMenu/PauseMenu.tscn` and `PauseMenu2.tscn`

Current role:

- Pause overlays

These scenes share the same `PauseMenu.cs` script, but they are separate scene files with different visual layouts.

### `Scenes/Shop/Shop.tscn`

Current role:

- Shop UI shell

Important contents:

- Three `ShopItem` instances
- `ShopManager`

## Scene Dependencies

Some scripts depend on exact node names in their scenes.

Examples:

- `Player.cs` expects `PlayerInput`, `PlayerMovement`, `PlayerCombat`, `PlayerStats`, `AttackHitbox`, `Sprite2D`, and `AbilityManager`
- `Enemy.cs` expects `EnemyAi`, `EnemyMovement`, `EnemyStats`, and `EnemyCombat`
- `PlayerUi.cs` expects named labels and containers from `PlayerUi.tscn`
- `DeathScreenOverlay.cs` expects specific overlay and label nodes by path

If you rename those nodes, update the scripts at the same time.
