# Room Progression

The room system is only partially implemented in the current repository.

## Current Pieces

### `Room`

`Room` is a `Node2D` with one exported field:

- `RoomId`

It appears to exist as a future room identity marker.

### `RoomManager`

`RoomManager` exports:

- `RoomActivationZone`
- `Room`

In `Hub.tscn`, the activation zone connects its `body_entered` signal to `RoomManager.OnBodyEntered`.

## Current Behavior

`RoomManager` currently only subscribes to the activation zone and contains a placeholder body-entered handler.

It does not yet:

- Activate enemies
- Mark the room complete
- Advance room progression
- Reward the player
- Spawn the next room

## What This Means For Developers

If you are looking for the active room progression system, it is not implemented yet.

The current code only establishes the scene hook that future room activation logic can use.

## Relevant Scene

- `Scenes/Dungeon/Hub/Hub.tscn`

## Planned Future Work

Future room logic will likely need to live in `RoomManager` or a dedicated progression service, but that architecture is not active yet.
