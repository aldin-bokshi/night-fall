# NightFall

NightFall is a Godot 4.7 Mono C# top-down roguelike in active development.

The current codebase focuses on:

- Player movement, combat, stats, and abilities
- Enemy chase and attack behavior
- A data-driven shop display
- HUD, pause, and death overlays
- Scene-driven dungeon and menu flow

## Quick Start

1. Open the project in Godot 4.7 with the .NET 8 SDK installed.
2. Run the project from the editor.
3. The project starts on the main menu.
4. Choose Start, configure a run, and start the active gameplay scene.

## Controls

| Key | Action |
|-----|--------|
| WASD | Move |
| Left Click | Attack |
| Left Shift | Blink ability |
| Q | Gravity Well |
| Esc | Pause |

## Documentation

Developer docs live in [`Docs/README.md`](Docs/README.md).

If you are changing gameplay, that is the best place to start.

## Current Notes

- The current playable scene launched by the main menu is `Scenes/Game.tscn`
- `Scenes/Dungeon/Dev/TestWorld.tscn` remains a standalone developer fixture
- The player currently has two abilities instantiated in-scene: `BlinkAbility` and `GravityWellAbility`
- The shop currently displays items from JSON, but purchase handling is not implemented yet
- Room activation and progression are still partial

## Built With

- Godot 4.7
- C#
- .NET 8
- Git / GitHub

## License

This project is for educational and personal use.
