# Enemy Architecture

Enemies use the same component style as the player: a root entity node owns smaller behavior pieces for movement, AI, combat, and stats.

## Current Composition

```text
Enemy (CharacterBody2D)
├── EnemyAi
├── EnemyMovement
├── EnemyCombat
├── EnemyStats
├── Sprite2D
├── AttackHitbox
└── Hurtbox
```

## Responsibilities

### `Enemy`

`Enemy` is the root `CharacterBody2D` (group `"enemy"`).

It:

- Caches and initializes child components in `_Ready`
- Accepts a `RoomId` via `Initialize(int)` for future room association
- Updates attack timers in `_PhysicsProcess`
- Frees itself with death rewards when health reaches zero

### `EnemyAi`

`EnemyAi` is the behavior driver.

It:

- Caches the enemy root in `_Ready`
- Lazily looks up the player through the `"player"` group
- Measures distance to the player each physics frame
- Chooses between idle, chase, and attack

Current behavior:

- No player in range: stop moving
- Player in detection range: move toward player
- Player in attack range: stop and attack when ready

### `EnemyMovement`

`EnemyMovement` applies movement using `EnemyStats.MoveSpeed`.

It mirrors the player movement component but is enemy-specific, and additionally supports `AddExternalForce()` (used by `GravityWell` pulls). External force is cleared after each `Move()` call.

### `EnemyStats`

`EnemyStats` stores enemy gameplay values.

Current fields:

- `MaxHealth`
- `MoveSpeed`
- `AttackDamage`
- `AttackCooldown`
- `AttackDuration`
- `AttackRange`
- `DetectionRange`

### `EnemyCombat`

`EnemyCombat` owns enemy attack timing and hitbox activation.

It also directly applies damage to the player when the enemy attack hitbox overlaps the player `Hurtbox`. See [Combat](Combat.md) for the full flow including run modifiers.

## Scene Dependencies

`Enemy.cs` expects the enemy scene to include child nodes with these exact names:

- `EnemyAi`
- `EnemyMovement`
- `EnemyCombat`
- `EnemyStats`

`EnemyCombat` additionally resolves the `AttackHitbox` child by name from the enemy root.

The AI expects the player to be in the `"player"` group.

## Current Enemy Flow

```text
EnemyAi (distance check)
  → EnemyMovement.Move
  → EnemyCombat.Attack (when in range)
  → EnemyStats.TakeDamage → IsDead → Enemy.Die()
    → rewards + VFX + QueueFree
```

## Death Flow

When an enemy dies:

- `enemy_death` audio
- `RunTracker.RecordEnemySlain()`
- Gold reward (15, or 30 with `Greed`) with `gold` audio + floating text
- Death particle VFX
- `QueueFree()`

## Wave Spawning

Enemies are spawned by `RoomManager` in waves (see [Room Progression](Room-Progression.md)). There is no other spawn manager yet.

## Current Limitations

The enemy does not currently have:

- Patrol routes
- Fleeing
- Full state machines (distance-based only)
- Loot drops beyond flat gold
- Boss variants

## Creating A New Enemy

See [Adding New Enemies](../Guides/Adding-New-Enemies.md).