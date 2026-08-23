using Godot;
using NightFall.Scripts.Core;
using NightFall.Scripts.Run;
using NightFall.Scripts.Ui;

namespace NightFall.Scripts.Entities.Enemy;

public partial class Enemy : CharacterBody2D
{
    private EnemyMovement _movement = null!;
    private EnemyCombat _combat = null!;
    private EnemyStats _stats = null!;
    private EnemyAi _ai = null!;

    public int RoomId { get; private set; }

    public override void _Ready()
    {
        _movement = GetNode<EnemyMovement>("EnemyMovement");
        _combat = GetNode<EnemyCombat>("EnemyCombat");
        _stats = GetNode<EnemyStats>("EnemyStats");
        _ai = GetNode<EnemyAi>("EnemyAi");

        _movement.Initialize(_stats);
        _combat.Initialize(_stats);
        _ai.Initialize(_stats, _combat,_movement);
    }

    public void Initialize(int roomId)
    {
        RoomId = roomId;
    }

    public override void _PhysicsProcess(double delta)
    {
        _combat.UpdateAttack(delta);
        if (_stats.IsDead) Die();
    }

    private void Die()
    {
        AudioSynthManager.PlayEnemyDeath();
        RunTracker.RecordEnemySlain();

        int goldAmount = 15;
        var runConfig = RunSession.Current;
        if (runConfig is { Greed: true }) goldAmount = (int)(goldAmount * 2.0f);

        if (GetTree().GetFirstNodeInGroup("player") is Player.Player player)
        {
            player.Stats.AddGold(goldAmount);
            RunTracker.RecordGoldCollected(goldAmount);
            AudioSynthManager.PlayGold();
            FloatingText.Spawn(GetParent() ?? this, GlobalPosition + new Vector2(0, -10), $"+{goldAmount} Gold", new Color(0.95f, 0.8f, 0.2f), 14f);
        }

        VfxManager.SpawnParticles(GetParent() ?? this, GlobalPosition, new Color(0.6f, 0.1f, 0.1f), 16);
        QueueFree();
    }
}

