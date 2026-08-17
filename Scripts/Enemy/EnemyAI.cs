using Godot;

namespace NightFall.Scripts.Enemy;

public partial class EnemyAi : Node
{
    private EnemyStats _stats;

    public float DetectionRange => _stats?.DetectionRange ?? 0f;
    public float AttackRange => _stats?.AttackRange ?? 0f;

    public void Initialize(EnemyStats stats)
    {
        _stats = stats;
    }
}
