using Godot;
namespace NightFall.Core;

public partial class RoomManager : Node
{
    [Export] public Area2D? RoomActivationZone { get; private set; }
    [Export] public Node2D? Room { get; private set; }

    public override void _Ready()
    {
        if (RoomActivationZone == null)
        {
            GD.PushWarning("RoomManager.RoomActivationZone is not assigned.");
            return;
        }

        RoomActivationZone.BodyEntered += OnBodyEntered;
    }

    // private void ActivateEnemiesInRoom(int roomId)
    // {
    //     var enemies = GetEnemiesInRoom(roomId);
    //     foreach (var enemy in enemies)
    //     {
    //         enemy.SetProcess(true);
    //         enemy.SetPhysicsProcess(true);
    //     }
    // }

    private void OnBodyEntered(Node2D body)
    {
        // if (body is Player)
        // {
        //     ActivateEnemiesInRoom(Room.RoomId);
        // }
    }
}