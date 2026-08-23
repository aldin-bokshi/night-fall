# NightFall Developer Docs

NightFall is a Godot 4.7 Mono C# top-down roguelike in active development. These docs describe the current implementation, not the intended end state.

## What This Documentation Covers

- Project structure and runtime ownership
- Player, enemy, combat, abilities, shop, room, and UI systems
- Important scene relationships
- Development conventions and troubleshooting notes
- Guides for adding or extending major gameplay systems

## Start Here

- [Project Architecture](Architecture/Project-Architecture.md)
- [Scene Architecture](Architecture/Scene-Architecture.md)
- [Player Architecture](Architecture/Player-Architecture.md)
- [Abilities](Systems/Abilities.md)
- [Combat](Systems/Combat.md)
- [Enemy Architecture](Systems/Enemy.md)
- [Shop](Systems/Shop.md)
- [Room Progression](Systems/Room-Progression.md)
- [UI Architecture](Systems/UI.md)
- [Coding Guidelines](Development/Coding-Guidelines.md)
- [Debugging Guide](Development/Debugging.md)

## Common Changes

- Want to create an ability? Read [Creating New Abilities](Guides/Creating-New-Abilities.md) and [Abilities](Systems/Abilities.md).
- Want to modify player movement or attacks? Read [Player Architecture](Architecture/Player-Architecture.md) and [Combat](Systems/Combat.md).
- Want to add an enemy? Read [Enemy Architecture](Systems/Enemy.md) and [Adding New Enemies](Guides/Adding-New-Enemies.md).
- Want to change combat rules or hit detection? Read [Combat](Systems/Combat.md).
- Want to add or edit shop items? Read [Shop](Systems/Shop.md) and [Adding Shop Items](Guides/Adding-Shop-Items.md).
- Want to change menus or overlays? Read [UI Architecture](Systems/UI.md) and [Scene Architecture](Architecture/Scene-Architecture.md).

## Source Of Truth

These docs are based on:

- `Scripts/`
- `Scenes/`
- `Assets/`
- `Data/`
- `project.godot`
- `NightFall.csproj`

When source code changes, update the relevant documentation at the same time.
