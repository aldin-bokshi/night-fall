# Project Architecture

NightFall is organized as a scene-driven Godot project. There is no active global gameplay service layer that owns the whole run. Instead, scene composition and small focused component scripts define runtime behavior.

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
Dev/test gameplay scene
  ↓
Player / Enemy / UI node graphs
```

There is also a reusable in-game shell scene:

```text
Scenes/Core/Game.tscn
```

At the moment, the codebase does not use a central `GameManager` or `GameLoader` to orchestrate the run. Those files exist, but they are currently empty.

## Folder Responsibilities

### `Scripts/`

Contains C# behavior grouped by feature.

Current subfolders:

- `Scripts/Core/`
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

## Current Gaps

These are architectural realities of the current codebase, not recommendations:

- Room progression is only partially wired.
- Shop data exists, but purchasing is not implemented yet.
- The pause and menu scenes include visible buttons, but some of those buttons are not yet wired to behavior.
- `GameManager.cs` and `GameLoader.cs` currently contain no runtime logic.

See the system-specific docs for the actual implementation details.
