# Room Progression

The room system is implemented as a wave-based enemy encounter driven by `RoomManager`. There is no multi-room layout or room-chain generation yet; a single `Hub` scene contains the active wave hook.

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

If you want to change how rooms progress (drops, portals, next-room spawning, boss rooms), `RoomManager` is the place to start. The current `Scenes/Dungeon/Hub/Hub.tscn` uses one `RoomActivationZone` and no boss/next-room logic.

## Relevant Scene

- `Scenes/Dungeon/Hub/Hub.tscn`

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