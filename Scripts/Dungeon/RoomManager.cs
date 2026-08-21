using System.Collections.Generic;
using Godot;
using NightFall.Scripts.Core;
using NightFall.Scripts.Entities.Enemy;
using NightFall.Scripts.Entities.Player;
using NightFall.Scripts.Run;
using NightFall.Scripts.Ui;

namespace NightFall.Scripts.Dungeon;

public partial class RoomManager : Node
{
    [Export] public Area2D? RoomActivationZone { get; set; }
    [Export] public Node2D? Room { get; set; }
    [Export] public PackedScene? EnemyScene { get; set; }

    private bool _waveActive;
    private readonly List<Enemy> _activeEnemies = [];
    private int _currentWave = 1;

    public override void _Ready()
    {
        RoomActivationZone ??= GetNodeOrNull<Area2D>(".") ?? GetNodeOrNull<Area2D>("RoomActivationZone");
        EnemyScene ??= GD.Load<PackedScene>("res://Scenes/Entities/Enemies/Enemy.tscn");

        if (RoomActivationZone != null)
        {
            RoomActivationZone.BodyEntered += OnBodyEntered;
        }
    }

    public override void _Process(double delta)
    {
        if (!_waveActive) return;

        _activeEnemies.RemoveAll(e => !GodotObject.IsInstanceValid(e) || e.IsQueuedForDeletion());

        if (_activeEnemies.Count == 0)
        {
            OnWaveCleared();
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (_waveActive) return;
        if (body is not Player) return;

        StartWave();
    }

    private void StartWave()
    {
        _waveActive = true;
        _activeEnemies.Clear();

        AudioSynthManager.PlayGravityWell();

        var runConfig = RunSession.Current;
        int spawnCount = 3 + (_currentWave - 1);
        if (runConfig != null && runConfig.HardNight)
        {
            spawnCount += 2;
        }

        Vector2 origin = RoomActivationZone?.GlobalPosition ?? (GetParent() as Node2D)?.GlobalPosition ?? Vector2.Zero;
        Node parent = GetTree().CurrentScene?.FindChild("Enemies", true, false) ?? GetTree().CurrentScene ?? this;

        for (int i = 0; i < spawnCount; i++)
        {
            if (EnemyScene == null) break;

            Enemy enemy = EnemyScene.Instantiate<Enemy>();
            float angle = (float)(i * 2.0 * Mathf.Pi / spawnCount);
            Vector2 spawnOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (160f + GD.RandRange(-20, 40));

            enemy.GlobalPosition = origin + spawnOffset;
            parent.AddChild(enemy);
            _activeEnemies.Add(enemy);

            VfxManager.SpawnParticles(parent, enemy.GlobalPosition, new Color(0.8f, 0.2f, 0.2f), 12);
        }

        FloatingText.Spawn(parent, origin, $"WAVE {_currentWave} STARTED!", new Color(1.0f, 0.3f, 0.2f), 20f);
    }

    private void OnWaveCleared()
    {
        _waveActive = false;
        RunTracker.RecordRoomCleared();

        Node parent = GetTree().CurrentScene ?? this;
        Vector2 pos = RoomActivationZone?.GlobalPosition ?? Vector2.Zero;

        AudioSynthManager.PlayBuy();
        FloatingText.Spawn(parent, pos, "ROOM CLEARED!", new Color(0.3f, 1.0f, 0.4f), 24f);

        var player = GetTree().GetFirstNodeInGroup("player") as Player;
        if (player != null && player.Stats != null)
        {
            int bonusGold = 25 * _currentWave;
            player.Stats.AddGold(bonusGold);
            RunTracker.RecordGoldCollected(bonusGold);
            FloatingText.Spawn(parent, pos + new Vector2(0, 30), $"+{bonusGold} ROOM BONUS GOLD!", new Color(0.95f, 0.8f, 0.2f), 16f);
        }

        _currentWave++;
    }
}