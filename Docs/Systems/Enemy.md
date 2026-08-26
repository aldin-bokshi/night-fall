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

`EnemyStats.Variant` selects a shared-code preset without changing the enemy scene composition:

| Variant | Health | Speed | Damage | Gameplay role |
| --- | ---: | ---: | ---: | --- |
| `Standard` | 100 | 150 | 20 | Baseline melee enemy |
| `Fast` | 65 | 260 | 14 | Quickly closes distance and attacks more often |
| `Tank` | 240 | 90 | 30 | Slow, durable threat with a heavier attack |

`FastEnemy.tscn` and `TankEnemy.tscn` inherit `Enemy.tscn` and only select the corresponding enum value. New variants should follow this pattern: add a preset to `EnemyStats`, then create a thin inherited scene if a distinct spawnable scene is needed. The base `Enemy`, AI, movement, combat, collision, and death code remains shared.

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

Enemies are spawned by `RoomManager` in waves (see [Room Progression](Room-Progression.md)). Wave one uses the standard scene. Later waves choose standard, fast, or tank scenes using a deterministic roll derived from the active run seed and wave number, so the same run produces the same variant pattern. There is no other spawn manager yet.

## Current Limitations

The enemy does not currently have:

- Patrol routes
- Fleeing
- Full state machines (distance-based only)
- Loot drops beyond flat gold
- ranged attacks or projectile-specific combat
- boss-specific behavior beyond the available stat variant pattern

## Creating A New Enemy

See [Adding New Enemies](../Guides/Adding-New-Enemies.md).