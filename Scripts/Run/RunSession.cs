namespace NightFall.Scripts.Run;

public static class RunSession
{
    public static RunConfig? Current { get; set; }

    public static void Start(RunConfig config)
    {
        Current = config;
    }

    public static void Clear()
    {
        Current = null;
    }
}