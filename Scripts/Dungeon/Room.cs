using Godot;

namespace NightFall.Scripts.Dungeon;

public partial class Room : Node2D
{
    [Export] public int RoomId { get; set; }

    [Export] public int WidthInTiles { get; set; } = 20;

    [Export] public int HeightInTiles { get; set; } = 16;
}