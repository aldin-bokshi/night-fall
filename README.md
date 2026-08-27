# NightFall

NightFall is a **Godot 4.7 Mono C# top-down roguelike** currently in active development.

The project is built around modular gameplay systems, data-driven content, and a scene-based architecture. The current codebase includes player combat and abilities, enemy AI, a shop system, dungeon generation, room progression, and developer tooling.

## Current Systems

* **Player**

  * Movement
  * Combat and attacks
  * Dash
  * Player statistics
  * Ability system
  * Health and death handling

* **Abilities**

  * Ability slots and input bindings
  * Cooldowns
  * Ability UI
  * Blink
  * Gravity Well
  * Ability VFX and audio hooks

* **Enemies**

  * Enemy statistics
  * Player targeting
  * Chase behavior
  * Combat and attacks
  * Enemy death handling

* **Shop**

  * Data-driven shop items loaded from JSON
  * Randomized item selection
  * Item display
  * Stat upgrades
  * Reroll system

* **Dungeon**

  * Seed-based dungeon generation
  * Grid-based room positioning
  * Room size and overlap checks
  * Multiple room types
  * Room progression
  * Shop and boss sections
  * Scene-driven room instantiation

* **UI**

  * HUD
  * Ability UI
  * Shop UI
  * Pause screen
  * Death screen
  * Main menu
  * Run configuration

## Quick Start

### Requirements

* Godot **4.7 Mono**
* .NET SDK **8 or later**
* Git

### Running the Project

1. Clone the repository.
2. Open the project in **Godot 4.7 Mono**.
3. Make sure the required .NET SDK is installed.
4. Import the project and allow Godot to generate its project files.
5. Run the project from the editor.
6. The project starts on the main menu.
7. Choose **Start**, configure a run, and enter gameplay.

The main gameplay scene is:

```text
Scenes/Game.tscn
```
## Controls

| Key        | Action    |
| ---------- | --------- |
| WASD       | Move      |
| Left Click | Attack    |
| Left Shift | Ability 1 |
| Q          | Ability 2 |
| E          | Ability 3 |
| R          | Ability 4 |
| Esc        | Pause     |


## Project Structure

```text
NightFall/
├── Data/
│   └── Shop/
├── Docs/
├── Scenes/
│   ├── Dungeon/
│   ├── Game.tscn
│   ├── Player/
│   ├── Shop/
│   └── UI/
├── Scripts/
│   ├── Abilities/
│   ├── Dungeon/
│   ├── Enemies/
│   ├── Player/
│   ├── Shop/
│   └── UI/
├── tools/
└── NightFall.sln
```

The exact structure may change as systems are refactored.

## Documentation

Developer documentation lives in [`Docs/README.md`](Docs/README.md).

The documentation covers the project's architecture and provides guides for working on individual systems.

If you are modifying gameplay or adding a new system, **start with the developer documentation**.

## Development

NightFall uses a modular C# architecture where major gameplay responsibilities are separated into dedicated systems.

For example:

```text
Player
├── PlayerInput
├── PlayerMovement
├── PlayerCombat
├── PlayerDash
└── PlayerStats
```

Gameplay content is increasingly data-driven where appropriate, with systems such as the shop loading content from JSON rather than hard-coding individual entries.

The project also includes development checks and static analysis to help maintain code quality.

## Current Development Status

NightFall is still under active development. Some systems are complete enough for gameplay testing while others are still being expanded or refactored.

### Working

* Player movement
* Player combat
* Player stats
* Player dash
* Ability system
* Blink ability
* Gravity Well ability
* Enemy movement and targeting
* Enemy combat
* Enemy death
* HUD
* Pause UI
* Death screen
* Main menu
* Data-driven shop item loading
* Shop item generation
* Dungeon generation foundations
* Grid-based room positioning

### In Progress

* Full dungeon room progression
* Room activation and transitions
* Complete shop purchasing flow
* Dungeon generation refinement
* Additional abilities
* Additional enemy types
* Gameplay balancing
* Content expansion

## Development Fixtures

The project contains standalone scenes used for development and testing.

For example:

```text
Scenes/Dungeon/Dev/TestWorld.tscn
```

`TestWorld.tscn` is a developer fixture and is not the primary gameplay flow.

The main playable flow begins from the main menu and currently launches:

```text
Scenes/Game.tscn
```

## Built With

* **Godot 4.7 Mono**
* **C#**
* **.NET**
* **Git**
* **GitHub**

## License

This project is for educational and personal use.
