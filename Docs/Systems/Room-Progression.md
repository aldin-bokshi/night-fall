# Room Progression

NightFall has a deterministic generated room layout plus the existing wave-based encounter hook. `DungeonGenerator` creates the room sequence, `Game` instantiates its scenes, and `RoomManager` continues to own combat waves inside rooms that contain that manager.

## Generated Layout

`DungeonGenerator.Generate(ulong seed)` uses a dedicated `System.Random` initialized from `RunConfig.Seed`. Its rules are separate from RNG setup:

- The sequence starts with `Start`.
- It contains four to seven choice rooms.
- Choice rooms are `Combat`, `Elite`, or `Shop`; the first shop has a 20% chance and combat is the common result.
- A shop is followed by combat or elite rather than another shop.
- The sequence ends with `Boss`.

The same numeric seed produces the same room count, order, and types. `Game.cs` loads the matching room scenes from `GamePaths.RoomScenes`, removes the legacy hub instance, and places the generated rooms in order under `World/Dungeon`. Room scenes are currently laid out spatially; portals and automatic room-to-room progression are not implemented yet.

## Current Pieces

### `Room`

`Room` is a `Node2D` with one exported field:

- `RoomId`

It is a lightweight identity marker and is not currently used to drive progression logic.

### `RoomManager`

`RoomManager` is a `Node` that owns the wave lifecycle. It exports:

- `RoomActivationZone` (`Area2D`) - the trigger that starts waves when the player enters
- `Room` (`Node2D`) - the room root, unused by current logic
- `EnemyScene` (`PackedScene`) - defaults to `res://Scenes/Entities/Enemies/Enemy.tscn`

In `_Ready`, `RoomManager`:

- Resolves the activation zone from the export or from the scene by name (`RoomActivationZone`)
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
- Enemies spawn in a ring around the activation zone (radius ~160px, random jitter)
- Spawns enemies under the `Enemies` node in the current scene (fallback: scene root)
- Plays `gravity_well` audio and spawns particle VFX + a "WAVE N STARTED!" floating text

### Wave Clear

- Sets `_waveActive = false`
- Records `RunTracker.RecordRoomCleared()`
- Plays `buy` audio and spawns "ROOM CLEARED!" floating text
- Awards the player `25 * wave` gold bonus via `PlayerStats.AddGold`, tracked by `RunTracker.RecordGoldCollected`
- Increments `_currentWave` for the next encounter

## What This Means For Developers

If you want to change room generation rules, start with `DungeonGenerator`. If you want to add drops, portals, room completion, or next-room movement, extend the generated-room flow in `Game` and the room-specific scene scripts. `RoomManager` remains the place for wave lifecycle behavior.

## Relevant Scenes

- `Scenes/Dungeon/StartRoom/StartRoom.tscn`
- `Scenes/Dungeon/CombatRoom/CombatRoom.tscn`
- `Scenes/Dungeon/EliteRoom/EliteRoom.tscn`
- `Scenes/Dungeon/ShopRoom/ShopRoom.tscn`
- `Scenes/Dungeon/BossRoom/BossRoom.tscn`
- `Scenes/Dungeon/Hub/Hub.tscn` (legacy source scene)

## Current Limitations

- One zone per scene; no multi-room chain
- No boss rooms or room-transition logic (the `BossRoom` and `Rooms` scene folders are empty)
- No enemy spawn manager beyond the wave loop; enemies are placed in scenes normally
</｜DSML｜tool>
<task_progress>
- [x] Explore documentation structure
- [x] Explore source code structure
- [x] Read existing documentation files
- [x] Audit docs against implementation
- [x] Verify remaining details (tres files, core services)
- [x] Update Room-Progression.md
- [ ] Update Shop.md
- [ ] Update Combat.md
- [ ] Update Enemy.md
- [ ] Update Project-Architecture.md
- [ ] Update Scene-Architecture.md
- [ ] Update Player-Architecture.md
- [ ] Update Abilities.md
- [ ] Update UI.md
- [ ] Update Guides (Adding-Shop-Items, Creating-New-Abilities)
- [ ] Update Development (Debugging, Coding-Guidelines)
- [ ] Update Documentation.md index
- [ ] Create Systems/Run.md
- [ ] Create Systems/Core-Services.md
- [ ] Second audit & verification
</task_progress>
