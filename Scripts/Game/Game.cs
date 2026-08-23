using System.Collections.Generic;
using Godot;
using NightFall.Scripts.Core;
using NightFall.Scripts.Dungeon;
using NightFall.Scripts.Entities.Player;
using NightFall.Scripts.Run;

namespace NightFall.Scripts.Game;

public partial class Game : Node
{
    [Export] private NodePath _worldPath = "World";
    [Export] private NodePath _uiPath = "UI";
    [Export] private NodePath _dungeonPath = "World/Dungeon";
    [Export] private NodePath _playerPath = "World/Player";
    [Export] private NodePath _hudPath = "UI/HUD";

    public RunConfig CurrentRun { get; private set; } = null!;
    public Node2D? World { get; private set; }
    public CanvasLayer? Ui { get; private set; }
    public Node2D? Dungeon { get; private set; }
    public Player? Player { get; private set; }
    public CanvasLayer? Hud { get; private set; }
    public RunTracker RunTracker { get; private set; } = null!;
    public IReadOnlyList<RoomType> DungeonLayout { get; private set; } = [];

    public override void _EnterTree()
    {
        CurrentRun = RunSession.Current ?? CreateFallbackRun();
    }

    public override void _Ready()
    {
        World = GetNodeOrNull<Node2D>(_worldPath);
        Ui = GetNodeOrNull<CanvasLayer>(_uiPath);
        Dungeon = GetNodeOrNull<Node2D>(_dungeonPath);
        Player = GetNodeOrNull<Player>(_playerPath);
        Hud = GetNodeOrNull<CanvasLayer>(_hudPath);

        AudioSynthManager.EnsureInstance(this);

        RunTracker = new RunTracker();
        AddChild(RunTracker);

        GenerateDungeon();
        ApplyRunModifiers();
    }

    private void GenerateDungeon()
    {
        DungeonLayout = DungeonGenerator.Generate(CurrentRun.Seed);

        GD.Print(
            $"Dungeon seed: {CurrentRun.SeedText} ({CurrentRun.Seed}), " +
            $"rooms: {DungeonLayout.Count}, " +
            $"layout: {string.Join(" -> ", DungeonLayout)}");

        if (Dungeon == null)
        {
            return;
        }

        foreach (Node child in Dungeon.GetChildren())
        {
            child.Free();
        }

        for (int index = 0; index < DungeonLayout.Count; index++)
        {
            RoomType roomType = DungeonLayout[index];
            PackedScene? roomScene = GD.Load<PackedScene>(GetRoomScenePath(roomType));

            if (roomScene == null)
            {
                GD.PushError($"Dungeon room scene could not be loaded: {roomType}");
                continue;
            }

            Node2D room = roomScene.Instantiate<Node2D>();
            room.Name = $"{roomType}Room{index + 1}";
            room.Position = new Vector2(index * 700f - 320f, -180f);
            Dungeon.AddChild(room);
        }
    }

    private static string GetRoomScenePath(RoomType roomType)
    {
        return roomType switch
        {
            RoomType.Start => GamePaths.RoomScenes.StartRoom,
            RoomType.Combat => GamePaths.RoomScenes.CombatRoom,
            RoomType.Elite => GamePaths.RoomScenes.EliteRoom,
            RoomType.Shop => GamePaths.RoomScenes.ShopRoom,
            RoomType.Boss => GamePaths.RoomScenes.BossRoom,
            _ => GamePaths.RoomScenes.HubRoom
        };
    }

    private void ApplyRunModifiers()
    {
        if (Player != null)
        {
            var stats = Player.Stats;
            if (CurrentRun.GlassCannon)
            {
                stats.AttackDamage *= 2.0f;
                stats.MaxHealth = Mathf.Max(25f, stats.MaxHealth * 0.5f);
                stats.TakeDamage(0); // sync current health
            }
        }

        if (CurrentRun.BloodMoon && World != null)
        {
            CanvasModulate bloodTint = new()
            {
                Color = new Color(0.85f, 0.45f, 0.45f, 1.0f)
            };
            World.AddChild(bloodTint);
        }
    }

    private static RunConfig CreateFallbackRun()
    {
        GD.Randomize();

        ulong seed =
            ((ulong)GD.Randi() << 32)
            | GD.Randi();

        GD.PushWarning(
            "Game loaded without an active RunSession. Using a fallback run."
        );

        return new RunConfig(
            "EDITOR",
            seed,
            false,
            false,
            false,
            false,
            false
        );
    }
}
