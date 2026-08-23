# Combat

NightFall currently has two combat pipelines: one for the player and one for enemies.

## Player Combat Pipeline

```text
Input
  ↓
PlayerInput.AttackPressed
  ↓
Player.HandleAttack()
  ↓
PlayerCombat.Attack()
  ↓
AttackHitbox.Activate()
  ↓
Collision overlap
  ↓
EnemyStats.TakeDamage()
  ↓
Enemy death / rewards + queue free
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
- Configures the `AttackHitbox` (position + rotation from facing)
- Activates the hitbox

### Player Hit Detection

`AttackHitbox` is an `Area2D` under the player.

It currently:

- Enables/disables its collision shape and monitoring with `Activate()`/`Deactivate()`
- Tracks already hit enemy roots in a hash set so an enemy is hit at most once per swing
- Resolves the hit target to a `Node2D`, then looks up an `EnemyStats` child
- Applies `PlayerStats.AttackDamage`
- On activation plays `slash` audio and spawns a slash-arc VFX
- On hit plays `hit` audio, shows a floating damage number, and triggers a hit flash + screen shake

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
Enemy attack hitbox overlap ("Hurtbox")
  ↓
PlayerStats.TakeDamage()
  ↓
Player death screen
```

### Enemy AI Behavior

`EnemyAi` measures the distance to the player every physics frame:

- Outside `DetectionRange`: stop moving
- Inside `DetectionRange` but outside `AttackRange`: move toward player
- Inside `AttackRange`: stop and attack when `EnemyCombat.CanAttack()` is true

`EnemyCombat` then:

- Stores the facing direction
- Sets attack and cooldown timers
- Positions and rotates the enemy attack hitbox
- Enables hitbox monitoring and visibility

### Enemy Hit Detection

Enemy attacks listen for `AreaEntered` on the enemy attack hitbox, and only damage areas named `Hurtbox`.

When it finds the player's `Hurtbox`, it:

- Gets the player root
- Reads `PlayerStats`
- Multiplies `EnemyStats.AttackDamage` by run modifiers:

| Modifier | Damage Multiplier |
| --- | --- |
| `BloodMoon` | `×1.25` |
| `Fragile` | `×1.50` |

- Calls `PlayerStats.TakeDamage(damage)`
- Plays `player_hurt` audio, shows `-damage` floating text, hit flash, and screen shake

## Collision Layers And Masks

Current scene setup uses explicit layers:

| Object | Layer | Mask |
| --- | --- | --- |
| Player root body | 2 | 5 |
| Player `Hurtbox` | 8 | 64 |
| Player `AttackHitbox` | 32 | 16 |
| Enemy root body | 4 | 3 |
| Enemy `Hurtbox` | 16 | 32 |
| Enemy `AttackHitbox` | 64 | 8 |

Layer names in `project.godot`:

```text
layer_1 = World
layer_2 = Player
layer_3 = Enemy
layer_4 = Player Hurtbox
layer_5 = Enemy Hurtbox
layer_6 = Player Attack
layer_7 = Enemy Attack
```

These values matter because combat scripts assume hitboxes and hurtboxes overlap on the right layers.

## Timing

### Player

- Attack cooldown comes from `PlayerStats.AttackCooldown`
- Attack active duration comes from `PlayerStats.AttackDuration`

### Enemy

- Attack cooldown comes from `EnemyStats.AttackCooldown`
- Attack active duration comes from `EnemyStats.AttackDuration`

## Death Handling

### Player Death

When `PlayerStats.IsDead` becomes true, `Player.Die()`:

- Looks up `DeathScreen` in the current scene tree and casts it to `DeathScreenOverlay`
- Reads run stats from `RunTracker` (rooms cleared, enemies slain, gold collected, run time)
- Calls `ShowDeathScreen(...)` with those stats

`DeathScreenOverlay` pauses the tree and reloads the current scene when retry is pressed.

### Enemy Death

When `EnemyStats.IsDead` becomes true, `Enemy.Die()`:

- Plays `enemy_death` audio
- Records `RunTracker.RecordEnemySlain()`
- Awards 15 gold to the player (30 with `Greed`), tracked by `RunTracker.RecordGoldCollected`
- Plays `gold` audio and shows `+N Gold` floating text
- Spawns death particles
- Calls `QueueFree()`

## Extension Guidance

- Put attack timing in the combat component, not in input code
- Put damage application in the combat or hitbox layer, not in UI
- Add new combat-specific collision rules in the relevant hitbox or combat script
- Rewards/death events are separate concerns from `TakeDamage()`; `Enemy.Die()` already owns death-side rewards