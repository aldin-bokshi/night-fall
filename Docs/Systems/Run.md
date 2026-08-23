# Run Configuration And Tracking

NightFall runs are configured before gameplay, handed over through a static session, and tracked while active. This document covers the three pieces: `RunConfig`, `RunSession`, and `RunTracker`.

## Run Lifecycle

```text
MainMenu.tscn
  ↓
DungeonSetup.tscn
  ↓  RunSession.Start(RunConfig)
Game.tscn
  ↓
Game.cs (_EnterTree): CurrentRun = RunSession.Current
  ↓
RunTracker and generated dungeon added by Game.cs in _Ready
```

If `Game.tscn` is opened without an active `RunSession` (e.g. directly from the editor), `Game.cs` creates a fallback `RunConfig` with seed text `EDITOR` and a random seed.

## `RunConfig`

`Scripts/Run/RunConfig.cs` is a small immutable configuration object:

- `SeedText` — the user-entered or generated seed string
- `Seed` — the deterministic `ulong` produced from `SeedText`
- `BloodMoon` — red screen tint (`CanvasModulate`) on the world, and enemy damage ×1.25
- `GlassCannon` — player `AttackDamage` ×2 and `MaxHealth` ×0.5 (min 25)
- `HardNight` — `RoomManager` spawns +2 enemies per wave
- `Greed` — enemy gold rewards ×2 and shop prices ×1.5
- `Fragile` — enemy damage ×1.50

`DungeonSetup` preserves the exact non-empty text entered in the seed field, including case, spaces, punctuation, and Unicode. Empty input creates an eight-character random seed before the run starts. `SeedTranslator` encodes the preserved text as UTF-8, hashes it with SHA-256, and converts the first eight hash bytes to a deterministic `ulong`. It does not use `GetHashCode()` or normalize the text.

## `RunSession`

`Scripts/Run/RunSession.cs` is a static handoff between the setup screen and gameplay:

- `Current` — the active `RunConfig`, or `null`
- `Start(RunConfig)` — sets `Current`
- `Clear()` — clears `Current`

`Game.cs` reads `RunSession.Current` during `_EnterTree`. In `_Ready`, it passes `CurrentRun.Seed` to `DungeonGenerator`, which owns a dedicated deterministic `System.Random`; the generator does not seed or consume Godot's global RNG. Nothing clears `RunSession` on death; the death screen reloads the current scene.

## Dungeon Generation

`DungeonGenerator.Generate(RunConfig.Seed)` always produces `Start`, four to seven choice rooms, and `Boss`. Each choice is selected deterministically from `Combat`, `Elite`, and `Shop`: the first shop has a 20% chance, combat has the largest weight, and a room immediately after a shop is restricted to combat or elite. The returned room types are logged with the exact seed text, numeric seed, and room count.

`Game.cs` uses that sequence to load the matching scenes from `GamePaths.RoomScenes`, removes the legacy hub instance, and places the generated rooms in order under `World/Dungeon`. The first room is positioned around the existing player origin. The current implementation lays out the rooms but does not yet provide portals or automatic transitions between them.

## `RunTracker`

`Scripts/Run/RunTracker.cs` is a `Node` added to the tree by `Game.cs` in `_Ready`. It tracks run performance:

- `Instance` — static access to the single tracker
- `RoomsCleared` — incremented via `RecordRoomCleared()` (from `RoomManager.OnWaveCleared`)
- `EnemiesSlain` — incremented via `RecordEnemySlain()` (from `Enemy.Die`)
- `GoldCollected` — incremented via `RecordGoldCollected(int)` (from enemy kills and room bonuses)
- `StartTimeMs` — set on enter tree; run time via `GetRunTimeSeconds()`

Consumers:

- `PlayerUi` shows run time, rooms, and kills
- `Player.Die()` reads rooms/kills/gold/time to populate the death screen

## Modifier Effects Summary

| Modifier | Where It Applies | Effect |
| --- | --- | --- |
| `BloodMoon` | `Game.cs`, `EnemyCombat` | Red world tint; enemy damage ×1.25 |
| `GlassCannon` | `Game.cs` | Player damage ×2, max health ×0.5 |
| `HardNight` | `RoomManager` | +2 enemies per wave |
| `Greed` | `Enemy`, `ShopItem` | Enemy gold ×2; shop prices ×1.5 |
| `Fragile` | `EnemyCombat` | Enemy damage ×1.50 |

## Related Docs

- [Enemies](Enemy.md)
- [Combat](Combat.md)
- [Shop](Shop.md)
- [Room Progression](Room-Progression.md
