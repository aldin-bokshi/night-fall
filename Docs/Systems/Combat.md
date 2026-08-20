# Combat

NightFall currently has two combat pipelines: one for the player and one for enemies.

## Player Combat Pipeline

```text
Input
  ↓
PlayerInput.AttackPressed
  ↓
Player.CanAttack()
  ↓
PlayerCombat.Attack()
  ↓
AttackHitbox.Activate()
  ↓
Collision overlap
  ↓
EnemyStats.TakeDamage()
  ↓
Enemy death / queue free
```

### Player Attack Flow

`PlayerInput` sets `AttackPressed` when the `attack` action is pressed.

`Player` only starts an attack when:

- The attack button was pressed
- The facing direction is not zero
- `PlayerCombat.CanAttack()` returns true

`PlayerCombat` then:

- Sets the attack cooldown timer
- Sets the attack duration timer
- Configures the `AttackHitbox`
- Activates the hitbox

### Player Hit Detection

`AttackHitbox` is an `Area2D` under the player.

It currently:

- Enables and disables its collision shape with `Activate()` and `Deactivate()`
- Stores already hit enemy roots in a hash set
- Applies damage when it detects an enemy `Area2D` or `Node2D`

The hitbox expects the enemy root to contain an `EnemyStats` node.

### Player Damage Source

Player attack damage comes from `PlayerStats.AttackDamage`.

## Enemy Combat Pipeline

```text
EnemyAi
  ↓
EnemyCombat.CanAttack()
  ↓
EnemyCombat.Attack()
  ↓
Enemy attack hitbox overlap
  ↓
PlayerStats.TakeDamage()
  ↓
Player death screen
```

### Enemy Attack Flow

`EnemyAi` measures the distance to the player every physics frame.

Current behavior:

- Outside `DetectionRange`, the enemy stops moving
- Inside `DetectionRange` but outside `AttackRange`, the enemy moves toward the player
- Inside `AttackRange`, the enemy stops and attacks if `EnemyCombat.CanAttack()` is true

`EnemyCombat` then:

- Stores the facing direction
- Sets attack and cooldown timers
- Positions and rotates the attack hitbox
- Enables hitbox monitoring and visibility

### Enemy Hit Detection

Enemy attacks do not reuse the player’s `AttackHitbox` script.

Instead, `EnemyCombat` listens for `AreaEntered` on the enemy attack hitbox and checks whether the target area is named `Hurtbox`.

When it finds the player hurtbox, it:

- Gets the player root
- Gets `PlayerStats`
- Calls `TakeDamage(AttackDamage)`

## Collision Layout

The current scene setup uses explicit collision layers and masks:

### Player

- Root body layer: `2`
- Root body mask: `5`
- Hurtbox layer: `8`
- Hurtbox mask: `64`
- Attack hitbox layer: `32`
- Attack hitbox mask: `16`

### Enemy

- Root body layer: `4`
- Root body mask: `3`
- Hurtbox layer: `16`
- Hurtbox mask: `32`
- Attack hitbox layer: `64`
- Attack hitbox mask: `8`

These values are important because the combat scripts assume the hitboxes and hurtboxes will overlap on the correct layers.

## Timing

### Player

- Attack cooldown comes from `PlayerStats.AttackCooldown`
- Attack active duration comes from `PlayerStats.AttackDuration`

### Enemy

- Attack cooldown comes from `EnemyStats.AttackCooldown`
- Attack active duration comes from `EnemyStats.AttackDuration`

## Death Handling

### Player Death

When `PlayerStats.IsDead` becomes true, `Player` looks for a `DeathScreen` node in the current scene and shows it.

`DeathScreenOverlay` pauses the tree and reloads the current scene when the retry button is pressed.

### Enemy Death

When `EnemyStats.IsDead` becomes true, `Enemy` simply calls `QueueFree()`.

There is no reward, loot, or death animation pipeline yet.

## Extension Guidance

- Put attack timing in the combat component, not in input code
- Put damage application in the combat or hitbox layer, not in UI
- Add new combat-specific collision rules in the relevant hitbox or combat script
- If combat needs rewards or death events later, that should be added as a separate system, not hidden inside `TakeDamage()`
