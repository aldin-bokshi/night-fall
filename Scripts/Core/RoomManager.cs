using Godot;
namespace NightFall.Scripts.Core;

public partial class RoomManager : Node
{
    [Export] public Area2D? RoomActivationZone { get; private set; }
    [Export] public Node2D? Room { get; private set; }

    public override void _Ready()
    {
        if (RoomActivationZone != null)
            RoomActivationZone.BodyEntered += OnBodyEntered;
    }

    private void ActivateEnemiesInRoom(int _)
    {
        // var enemies = GetEnemiesInRoom(roomId);
        // foreach (var enemy in enemies)
        // {
            // enemy.SetProcess(true);
            // enemy.SetPhysicsProcess(true);
        // }
    }

    private void OnBodyEntered(Node2D body)
    {
        // if (body is Player)
        // {
            // ActivateEnemiesInRoom(Room.RoomId);
        // }
    }
}