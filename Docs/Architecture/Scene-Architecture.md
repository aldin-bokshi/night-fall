# Scene Architecture

This document maps the important Godot scenes and how they relate to each other at runtime.

## Startup Flow

`project.godot` currently points the game to the main menu scene:

```text
project.godot
  ↓
Scenes/UI/MainMenu/MainMenu.tscn
  ↓
Scenes/UI/SetupScreen/DungeonSetup.tscn
  ↓
Scenes/Game.tscn
```

The main menu opens dungeon setup. The setup scene creates the `RunConfig`, stores it in
`RunSession`, and then opens the active gameplay root.

## Important Scenes

### `Scenes/UI/MainMenu/MainMenu.tscn`

Current role:

- Startup menu
- Opens the dungeon setup screen

Important nodes:

- `MainMenu` root `Control`
- `StartButton`
- `OptionsButton`
- `QuitButton`

Important behavior:

- `StartButton` changes the scene to `Scenes/UI/SetupScreen/DungeonSetup.tscn`
- `OptionsButton` opens the in-menu `OptionsMenu` overlay
- `QuitButton` exits the game

### `Scenes/UI/SetupScreen/DungeonSetup.tscn`

Collects the seed and the five run modifiers (Blood Moon, Glass Cannon, Hard Night, Greed, Fragile), stores them in `RunSession` as a `RunConfig`, and transitions to `Scenes/Game.tscn`.

### `Scenes/Game.tscn`

The active gameplay composition root:

```text
Game (Scripts/Game/Game.cs)
├── World (Node2D)
│   ├── Dungeon (generated room sequence)
│   ├── Player (Scenes/Entities/Player/Player.tscn)
│   ├── Enemies
│   ├── WorldObjects
│   ├── Projectiles
│   └── Effects
└── UI (CanvasLayer)
    └── HUD (Scenes/UI/UI.tscn)
```

`Game.cs` coordinates scene-level setup and generates the deterministic room sequence from `RunConfig.Seed`. It loads the room scenes listed in `GamePaths.RoomScenes`, removes the legacy hub instance, and places the rooms in order under `Dungeon`. The Player keeps its camera and ability manager, while the reused HUD supplies health, gold, abilities, pause, and death overlays.

`Game.cs` reads the active `RunSession` during `_EnterTree` and validates the required composition nodes during `_Ready`. `DungeonGenerator` uses a dedicated deterministic RNG for layout generation; it does not seed Godot's global RNG. Game still does not own movement, combat, enemy AI, ability gameplay, or HUD rendering.

The active run is entered as follows:

```text
MainMenu
  -> DungeonSetup
  -> RunSession.Start(RunConfig)
  -> Game.tscn
```

When Game is opened directly from the editor without a `RunSession`, `Game.cs`
creates an `EDITOR` fallback configuration so the scene remains runnable.

### `Scenes/Dungeon/Dev/TestWorld.tscn`

Current role:

- Standalone developer fixture
- Useful for testing a player, enemy, placeholder tilemap, and HUD without going through setup

Important contents:

- `World` root `Node2D`
- One `Enemy` instance
- One `Player` instance
- `HUD` instance from `Scenes/UI/UI.tscn`
- A tilemap layer for the placeholder environment

This remains a standalone developer fixture and is not part of the normal menu-to-run flow.

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

This older shell is retained for compatibility. Active runs use `Scenes/Game.tscn`.

### `Scenes/Entities/Player/Player.tscn`

Current role:

- Player entity composition

Important child nodes:

- `PlayerInput`
- `PlayerMovement`
- `PlayerCombat`
- `PlayerStats`
- `AbilityManager`
  - `GravityWellAbility`
  - `BlinkAbility`
- `Sprite2D`
- `AttackHitbox`
- `Camera2D`
- `Hurtbox`

Node order matters: `AbilityManager` assigns slots by child order, so `GravityWellAbility` is slot 0 and `BlinkAbility` is slot 1 in the current scene.

### `Scenes/Entities/Enemies/Enemy.tscn`

Current role:

- Enemy entity composition

Important child nodes:

- `EnemyAi`
- `EnemyMovement`
- `EnemyStats`
- `EnemyCombat`
- `Sprite2D`
- `AttackHitbox`
- `Hurtbox`

### `Scenes/UI/UI.tscn`

Current role:

- Reusable gameplay HUD container instanced at `Game/UI/HUD`

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

The current `PauseMenu.tscn` wires its buttons to script handlers:

- `ContinueButton` → resumes the game
- `OptionsButton` → opens the `OptionsMenu`
- `QuitButton` → returns to the main menu

### `Scenes/UI/OptionsMenu/OptionsMenu.tscn`

Current role:

- Options overlay used by both the main menu and the pause menu

Important contents:

- Master / SFX / Music volume sliders (drives `AudioSynthManager` volumes)
- Screen shake toggle (`AudioSynthManager.ScreenShakeEnabled`)
- Fullscreen toggle (`DisplayServer`)
- Close button

`OptionsMenu` sets `ProcessMode = Always` so it works while paused.

### `Scenes/Shop/Shop.tscn`

Current role:

- Shop UI shell, instanced at runtime by `ShopTrigger`

Important contents:

- Three `ShopItem` instances
- `ShopManager`
- `Leave` button (hides the shop and unpauses the tree)

### `Scenes/Shop/ShopItem.tscn`

Current role:

- Reusable item card inside the shop

Important contents:

- Name / price / stat upgrade labels
- `BuyButton`, wired to `ShopItem.OnBuyPressed`

### `Scenes/UI/PlayerUi/Ability.tscn`

Current role:

- One ability widget inside the HUD, driven by `AbilityUi`

Important contents:

- Name label, input label, icon, cooldown bar, cooldown text

## Scene Dependencies

Some scripts depend on exact node names in their scenes.

Examples:

- `Player.cs` expects `PlayerInput`, `PlayerMovement`, `PlayerCombat`, `PlayerStats`, `AttackHitbox`, `Sprite2D`, and `AbilityManager`
- `Enemy.cs` expects `EnemyAi`, `EnemyMovement`, `EnemyStats`, and `EnemyCombat`
- `PlayerUi.cs` expects named labels and containers from `PlayerUi.tscn`
- `DeathScreenOverlay.cs` expects specific overlay and label nodes by path

If you rename those nodes, update the scripts at the same time.
