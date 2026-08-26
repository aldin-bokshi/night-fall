namespace NightFall.Scripts.Dungeon;

public sealed record DungeonRoom(
    RoomType Type,
    GridPosition Position,
    int Width,
    int Height
);