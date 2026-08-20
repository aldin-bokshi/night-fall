# Enemy Architecture

Enemies use the same component style as the player: a root entity node owns smaller behavior pieces for movement, AI, combat, and stats.

## Current Composition

```text
Enemy
├── EnemyAi
├── EnemyMovement
├── EnemyCombat
├── EnemyStats
├── AttackHitbox
├── Hurtbox
└── Sprite2D
```

## Responsibilities

### `Enemy`

`Enemy` is the root `CharacterBody2D`.

It:

- Initializes the child components
- Stores a `RoomId` value for future room association
- Updates attack timers in `_PhysicsProcess`
- Frees itself when health reaches zero

### `EnemyAi`

`EnemyAi` is the current behavior driver.

It:

- Caches the enemy root
- Lazily looks up the player through the `"player"` group
- Measures distance to the player
- Chooses between idle, chase, and attack behavior

Current behavior:

- No player in range: stop moving
- Player in detection range: move toward player
- Player in attack range: stop and attack when ready

### `EnemyMovement`

`EnemyMovement` applies movement using `EnemyStats.MoveSpeed`.

It mirrors the player movement component but is enemy-specific.

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

It also directly applies damage to the player when the enemy attack hitbox overlaps the player hurtbox.

## Scene Dependencies

`Enemy.cs` expects the enemy scene to include child nodes with these exact names:

- `EnemyAi`
- `EnemyMovement`
- `EnemyCombat`
- `EnemyStats`
- `AttackHitbox`

The AI also expects the player to be in the `"player"` group.

## Current Enemy Flow

```text
EnemyAi
  ↓
EnemyMovement
  ↓
EnemyCombat
  ↓
EnemyStats
```

The enemy does not currently have:

- Patrol routes
- Fleeing
- State machines
- Spawn management
- Loot drops
- Room reward hooks

## Creating A New Enemy

See [Adding New Enemies](../Guides/Adding-New-Enemies.md).
