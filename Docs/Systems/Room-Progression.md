# Room Progression

NightFall has a deterministic generated room layout plus the existing wave-based encounter hook. `DungeonGenerator` creates the room sequence, `Game` instantiates its scenes, and `RoomManager` continues to own combat waves inside rooms that contain that manager.

## Generated Layout

`DungeonGenerator.Generate(ulong seed)` builds the sequence using a **private, SHA256-based deterministic RNG** (`DeterministicRandom`, nested inside `DungeonGenerator`) seeded from `RunConfig.Seed` — it does not use `System.Random` or Godot's global RNG. `DeterministicRandom.Next(maxExclusive)` hashes the seed bytes plus an internal counter with `SHA256.HashData` and reduces the result via modulo, so the same seed always produces the same sequence of rolls.

Generation rules:

- The sequence always starts with `Start`.
- It contains **four to seven** choice rooms (`MinimumChoiceRooms = 4` plus `0–3` extra from the RNG).
- Each choice room is `Combat`, `Elite`, or `Shop`.
- **At most one `Shop` room can appear in the entire run.** Every choice room gets a 20% roll to become the shop, but only while no shop has been placed yet (`shopCount == 0`); once one shop is chosen, no later room can roll into a shop.
- A room immediately following a `Shop` is restricted to `Combat` (70%) or `Elite` (30%) — never another shop.
- Outside of the post-shop and pre-shop cases, a room is `Combat` with 70% probability and `Elite` with 30% probability.
- The sequence always ends with `Boss`.

The same numeric seed always produces the same room count, order, and types. `Game.cs` uses that sequence to load the matching room scenes from `GamePaths.RoomScenes`, removes the legacy hub instance, and places the generated rooms in order under `World/Dungeon`. Room scenes are currently laid out spatially in a horizontal line (`index * 700f - 320f` on the X axis); portals and automatic room-to-room progression are not implemented yet.

## Current Pieces

### `Room`

`Room` is a `Node2D` with one exported field:

- `RoomId`

It is a lightweight identity marker and is not currently used to drive progression logic.

### `RoomType`

`RoomType` is an enum with six values: `Start`, `Combat`, `Elite`, `Shop`, `Boss`, `Hub`. Only `Start`, `Combat`, `Elite`, `Shop`, and `Boss` are ever produced by `DungeonGenerator`; `Hub` exists as a fallback case in `Game.GetRoomScenePath` and backs the legacy `HubRoom.tscn`, but the generator itself never emits it.

### `RoomManager`

`RoomManager` is a `Node` that owns the wave lifecycle. It exports:

- `RoomActivationZone` (`Area2D`) - the trigger that starts waves when the player enters
- `Room` (`Node2D`) - the room root, unused by current logic
- `EnemyScene` (`PackedScene`) - defaults to `res://Scenes/Entities/Enemies/Enemy.tscn`

In `_Ready`, `RoomManager`:

- Resolves the activation zone from the export, or falls back to itself / a child node named `RoomActivationZone`
- Resolves the enemy scene, falling back to the shared `Enemy.tscn`
- Subscribes `BodyEntered` → `OnBodyEntered`

## Current Behavior

`RoomManager` runs a wave loop:

```text
Player enters RoomActivationZone
  → StartWave()
  → Pause new waves while enemies are alive
  → All enemies die → OnWaveCleared()
  → Award gold + record room cleared
  → Next player entry starts the next (harder) wave
```

### Wave Start

- Spawns `3 + (wave - 1)` enemies by default
- `HardNight` adds `+2` enemies per wave
- Enemies spawn in a ring around the activation zone (radius `160px + random jitter(-20, 40)`)
- Spawns enemies under the `Enemies` node in the current scene (fallback: scene root)
- Plays `gravity_well` audio and spawns particle VFX + a "WAVE N STARTED!" floating text

### Wave Clear

- Checked every `_Process` frame by pruning `_activeEnemies` of freed/queued-for-deletion instances
- Sets `_waveActive = false`
- Records `RunTracker.RecordRoomCleared()`
- Plays `buy` audio and spawns "ROOM CLEARED!" floating text
- Awards the player `25 * wave` gold bonus via `PlayerStats.AddGold`, tracked by `RunTracker.RecordGoldCollected`
- Increments `_currentWave` for the next encounter

## What This Means For Developers

If you want to change room generation rules (room count, room-type odds, shop frequency), start with `DungeonGenerator` — all of that logic, including the deterministic RNG, lives in that one file. If you want to add drops, portals, room completion, or next-room movement, extend the generated-room flow in `Game` and the room-specific scene scripts. `RoomManager` remains the place for wave lifecycle behavior and is independent of how the room was placed in the dungeon sequence.

## Relevant Scenes

- `Scenes/Dungeon/StartRoom/StartRoom.tscn`
- `Scenes/Dungeon/CombatRoom/CombatRoom.tscn`
- `Scenes/Dungeon/EliteRoom/EliteRoom.tscn`
- `Scenes/Dungeon/ShopRoom/ShopRoom.tscn`
- `Scenes/Dungeon/BossRoom/BossRoom.tscn`
- `Scenes/Dungeon/HubRoom/HubRoom.tscn` (legacy source scene; only reached via the `Hub` fallback case, not by generation)

## Current Limitations

- One activation zone per scene; no multi-room chain
- No automated room-transition logic — generated rooms are placed side-by-side spatially, but the player must currently be moved manually between them
- No enemy spawn manager beyond the wave loop; enemies are placed in scenes normally
- At most one shop per run is a generation rule, not a limitation of `RoomManager` itself — `ShopRoom.tscn` and `HubRoom.tscn`'s `ShopAltar` are otherwise independent of how many shop rooms exist in a layout
