# Player Architecture

The player is built from several focused node components under `Scenes/Entities/Player/Player.tscn`. The root `Player` node coordinates them each frame.

## Current Composition

```text
Player
├── PlayerInput
├── PlayerMovement
├── PlayerCombat
├── PlayerStats
├── AbilityManager
│   └── BlinkAbility
│   └── GravityWellAbility
├── AttackHitbox
├── Hurtbox
├── Sprite2D
└── Camera2D
```

## Responsibilities

### `Player`

`Player` is the root entity node.

It does three main things:

- Caches child components in `_Ready`
- Orchestrates movement, combat, abilities, sprite facing, and death checks in `_PhysicsProcess`
- Finds and shows the death screen when health reaches zero

`Player` should stay an orchestrator, not become the home for all movement and combat logic.

### `PlayerInput`

`PlayerInput` polls input every `_Process` frame.

It currently tracks:

- Movement vector from `move_left`, `move_right`, `move_up`, and `move_down`
- Facing direction, updated from the latest movement input
- `attack`
- `ability_1` through `ability_4`, which map to slots `0` through `3`

It stores input as requests, not as gameplay execution.

### `PlayerMovement`

`PlayerMovement` applies movement using `PlayerStats.MoveSpeed`.

It is responsible for:

- Normalizing movement input
- Setting `CharacterBody2D.Velocity`
- Calling `MoveAndSlide`

### `PlayerCombat`

`PlayerCombat` owns the player attack timer and cooldown timer.

It:

- Decides whether an attack can start
- Configures the attack hitbox
- Activates and deactivates the hitbox
- Tracks attack duration and cooldown

### `PlayerStats`

`PlayerStats` stores current player gameplay values.

Current fields:

- `MaxHealth`
- `MoveSpeed`
- `AttackDamage`
- `AttackCooldown`
- `AttackDuration`
- `AttackRange`

It also tracks:

- `Health`
- `Gold`

The gold helpers already exist, but no gameplay system currently spends gold during a purchase flow.

### `AbilityManager`

`AbilityManager` owns the player ability nodes.

Current behavior:

- Treats direct child `Ability` nodes as the active ability list
- Limits the list to the fixed four ability slots
- Maps slot order to input actions and displayed keys
- Calls `Use()` on the selected slot
- Can add or remove ability nodes at runtime

### `Abilities`

Abilities are self-contained gameplay nodes under `AbilityManager`.

The current in-scene abilities are `BlinkAbility` and `GravityWellAbility`.

## Frame Flow

The player loop currently looks like this:

```text
PlayerInput._Process
  ↓
Player._PhysicsProcess
  ├── Update combat timers
  ├── Flip the sprite based on facing
  ├── Trigger ability if a slot key was pressed
  ├── Trigger attack if attack was pressed and combat is ready
  ├── Move the character
  └── Check for death
```

## Data Flow

### Movement

```text
Input action state
  ↓
PlayerInput.MovementInput
  ↓
PlayerMovement
  ↓
PlayerStats.MoveSpeed
  ↓
CharacterBody2D.Velocity
```

### Combat

```text
Input attack press
  ↓
PlayerInput.AttackPressed
  ↓
Player._PhysicsProcess
  ↓
PlayerCombat.Attack
  ↓
AttackHitbox
  ↓
EnemyStats.TakeDamage
```

### Abilities

```text
Ability key press
  ↓
PlayerInput.AbilitySlotPressed
  ↓
Player.TryUseAbility(slot)
  ↓
AbilityManager.TryUseAbility(slot)
  ↓
Ability.Use
```

## Extension Rules

- Movement-related behavior belongs in `PlayerMovement`
- Attack timing and hitbox control belong in `PlayerCombat`
- Shared or per-ability logic belongs in the ability classes
- Player HUD updates belong in UI code, not in `Player`

## Current Limitations

- The player death flow currently looks for a node named `DeathScreen` in the active scene tree
- Gold exists on `PlayerStats`, but shop purchasing is not wired yet
- The player scene currently instantiates two abilities
