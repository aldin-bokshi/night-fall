# Coding Guidelines

These guidelines reflect the patterns already used in the NightFall codebase.

## Structure

- Keep gameplay systems small and focused
- Prefer a root entity plus components over a single large class
- Keep UI code separate from gameplay code
- Keep data/configuration separate from behavior

## Namespaces And File Layout

- Follow the folder structure in namespaces where practical
- Current namespaces use the `NightFall.Scripts...` and `NightFall.Data...` roots
- Keep one primary class per file
- Match scene node names and C# class names carefully, because the code uses `GetNode(...)` heavily

## Godot Conventions

- Use `[Export]` for scene wiring and tunable gameplay values
- Use nullable exports when a node may not be assigned in the editor
- Use `ProcessModeEnum.Always` for pause menus and overlays that must keep working while paused
- Prefer scene composition for runtime wiring

## Input

- Define input actions in `project.godot`
- Read input through the action system, not by checking raw keys in gameplay code
- Keep the meaning of an input action in one place

## Nullability And Safety

- The project enables nullable reference types
- Prefer guard clauses when a required node or resource is missing
- Use `GetNodeOrNull(...)` when a reference may legitimately be absent
- Log clear errors with `GD.PushError`, `GD.PushWarning`, or `GD.Print` when useful

## Gameplay Boundaries

- Movement logic belongs in movement components
- Attack timing belongs in combat components
- Ability effects belong in ability classes
- UI should read state instead of duplicating gameplay rules

## Data

- Put configuration in resources or data files when the value is not behavior
- Use JSON for simple catalog data when the project already does that
- Keep stat names and data keys consistent between code and data files
- `PlayerStats.ApplyUpgrade` is the single interpreter for shop `statUpgrades` keys (`max_health`, `damage`, `move_speed`, `attack_speed`, `defense`, `lifesteal`, `luck`, `dash_cooldown`); add new supported keys there, not in UI code

## Practical Rule Of Thumb

If a feature needs to answer more than one of these questions, it probably wants its own class:

- How does it move?
- How does it attack?
- How does it store stats?
- How does it display itself?
- How does it respond to input?

## Current Project-Specific Notes

- `PlayerInput` polls actions and stores requests
- `Player` orchestrates player systems
- `AbilityManager` only manages direct child abilities
- `PlayerUi` rebuilds its ability widgets from the manager state
- `EnemyAi` currently performs distance-based chase and attack logic without a full state machine
