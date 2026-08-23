using Godot;

namespace NightFall.Scripts.Run;

public partial class RunTracker : Node
{
    public static RunTracker? Instance { get; private set; }

    public int RoomsCleared { get; private set; }
    public int EnemiesSlain { get; private set; }
    public int GoldCollected { get; private set; }
    public ulong StartTimeMs { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
        StartTimeMs = Time.GetTicksMsec();
        RoomsCleared = 0;
        EnemiesSlain = 0;
        GoldCollected = 0;
    }

    public override void _ExitTree()
    {
        if (Instance == this) Instance = null;
    }

    public static void ResetTracker()
    {
        if (Instance == null) return;

        Instance.RoomsCleared = 0;
        Instance.EnemiesSlain = 0;
        Instance.GoldCollected = 0;
        Instance.StartTimeMs = Time.GetTicksMsec();
    }

    public static void RecordEnemySlain()
    {
        if (Instance == null) return;

        Instance.EnemiesSlain++;
    }

    public static void RecordRoomCleared()
    {
        if (Instance == null) return;

        Instance.RoomsCleared++;
    }

    public static void RecordGoldCollected(int amount)
    {
        if (Instance == null) return;

        Instance.GoldCollected += amount;
    }

    public static float GetRunTimeSeconds()
    {
        if (Instance == null) return 0f;

        return (Time.GetTicksMsec() - Instance.StartTimeMs) / 1000f;
    }
}