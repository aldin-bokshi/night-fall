# Adding New Enemies

This guide matches the current enemy architecture.

## Current Pattern

Enemies are `CharacterBody2D` scenes composed from small scripts.

```text
Enemy
├── EnemyAi
├── EnemyMovement
├── EnemyCombat
├── EnemyStats
├── AttackHitbox
└── Hurtbox
```

## Recommended Workflow

### 1. Duplicate The Enemy Scene

Use `Scenes/Entities/Enemies/Enemy.tscn` as the template.

### 2. Keep The Expected Node Names

The current scripts expect these names:

- `EnemyAi`
- `EnemyMovement`
- `EnemyCombat`
- `EnemyStats`
- `AttackHitbox`
- `Hurtbox`

If you rename them, update the scripts too.

### 3. Adjust Stats In The Inspector

Tune the exported values on `EnemyStats`:

- Health
- Move speed
- Attack damage
- Attack cooldown
- Attack duration
- Attack range
- Detection range

### 4. Assign Collision Layers Correctly

The combat pipeline depends on collision setup.

Match the current enemy scene unless you are intentionally changing the interaction model.

### 5. Decide Whether The Enemy Uses The Existing AI

The current `EnemyAi` is distance-based:

- idle outside detection range
- chase in detection range
- attack in attack range

If your new enemy needs different behavior, replace or extend that script.

### 6. Spawn Or Place The Enemy In A Scene

Two options exist today:

- **Wave spawning (active runs)**: `RoomManager` spawns `Scenes/Entities/Enemies/Enemy.tscn` in waves. If you duplicate the enemy scene, either update `RoomManager`'s `EnemyScene` export or keep the original path.
- **Manual placement**: place the scene directly in a test level (`Scenes/Dungeon/Dev/TestWorld.tscn`) or instantiate it from your own scene logic.

## Targeting And Damage

Enemy attacks currently damage the player by:

- Detecting an area named `Hurtbox`
- Getting the player root
- Reading `PlayerStats`
- Calling `TakeDamage`

If your enemy uses a different hurtbox structure, update the combat code accordingly.

## Testing

Test in:

- `Scenes/Dungeon/Dev/TestWorld.tscn`

That scene already contains both a player and an enemy.

## Current Limitations

The enemy system does not yet include:

- Patrol routes
- Boss variants
- Loot drops beyond flat gold

Enemy death already grants gold, tracks `RunTracker`, plays audio, and spawns VFX. Spawn waves exist via `RoomManager`; see [Room Progression](../Systems/Room-Progression.md).
