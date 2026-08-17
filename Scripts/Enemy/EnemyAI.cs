using Godot;

namespace NightFall.Scripts.Enemy;

public partial class EnemyAi : Node
{
    private EnemyStats _stats;

    public void Initialize(EnemyStats stats)
    {
        _stats = stats;
    }
}
