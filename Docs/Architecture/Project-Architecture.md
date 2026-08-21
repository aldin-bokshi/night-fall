# Project Architecture

NightFall is organized as a scene-driven Godot project. Active runs are composed by `Scenes/Game.tscn`; small focused component scripts still own runtime behavior.

## High-Level Layout

```text
NightFall/
├── Assets/     Visual, audio, and VFX assets
├── Data/       JSON and Resource-based configuration
├── Docs/       Developer documentation
├── Scenes/     Godot scene composition
├── Scripts/    C# gameplay and UI behavior
├── project.godot
└── NightFall.csproj
```

## Runtime Ownership

The current runtime is split into a few clear layers:

```text
project.godot
  ↓
Main menu scene
  ↓
Dungeon setup scene
  ↓
Scenes/Game.tscn (World + UI)
```

There is also a legacy reusable in-game shell scene:

```text
Scenes/Core/Game.tscn
```

`Scripts/Game/Game.cs` is a thin composition root and does not own player, enemy, dungeon, ability, or UI mechanics. `RunSession` remains the run-data handoff between setup and gameplay.

## Folder Responsibilities

### `Scripts/`

Contains C# behavior grouped by feature.

Current subfolders:

- `Scripts/Core/`
- `Scripts/Game/`
- `Scripts/Dungeon/`
- `Scripts/Entities/Enemy/`
- `Scripts/Entities/Player/`
- `Scripts/Entities/Player/Abilities/`
- `Scripts/Entities/Player/Ui/`
- `Scripts/Shop/`
- `Scripts/Ui/`

### `Scenes/`

Contains the runtime node graphs.

Important scene groups:

- `Scenes/UI/MainMenu/`
- `Scenes/UI/PauseMenu/`
- `Scenes/UI/DeathScreen/`
- `Scenes/UI/PlayerUi/`
- `Scenes/Entities/Player/`
- `Scenes/Entities/Enemies/`
- `Scenes/Dungeon/Dev/`
- `Scenes/Dungeon/Hub/`
- `Scenes/Core/`
- `Scenes/Game.tscn`

### `Data/`

Contains configuration and data files.

Current data types:

- Ability `Resource` files in `Data/Abilities/`
- Shop item JSON in `Data/Shop/ShopItems.json`

## Important Runtime Patterns

### Components Over Monoliths

Both player and enemy entities are split into smaller components rather than having one large script own everything.

```text
Player / Enemy root
  ├── Movement
  ├── Combat
  ├── Stats
  ├── AI or Input
  └── Ability system when relevant
```

### Scene-Driven Wiring

Most behavior depends on scene names, node names, and exported references. If a node name changes, code that uses `GetNode(...)` will usually need to change too.

### Data vs Behavior

The project keeps configuration in `Resource` files or plain data objects when possible, while gameplay logic lives in scripts.

Examples:

- `AbilityData` stores ability display and cooldown data.
- `ItemData` stores shop item data.
- `PlayerStats` and `EnemyStats` store runtime gameplay numbers.

## Run Configuration

Runs are configured from `DungeonSetup` and carried into `Scenes/Game.tscn` through the static `RunSession` handoff. `RunConfig` holds the seed plus five modifiers (`BloodMoon`, `GlassCannon`, `HardNight`, `Greed`, `Fragile`). `RunTracker` records rooms cleared, enemies slain, gold collected, and run time for the HUD and death screen.

See [Run System](../Systems/Run.md) for the full details and where each modifier affects gameplay.

## Current Gaps

These are architectural realities of the current codebase, not recommendations:

- `GameManager.cs` and `GameLoader.cs` currently contain no runtime logic.
- `Scenes/Dungeon/Rooms/`, `Scenes/Dungeon/BossRoom/`, and `Scenes/Dungeon/Shop/` are empty folders; there is no boss fight or multi-room progression.
- `Scenes/UI/Win/` is an empty folder; there is no win screen.

See the system-specific docs for the actual implementation details.
