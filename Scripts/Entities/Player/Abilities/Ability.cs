using Godot;

namespace NightFall.Scripts.Entities.Player.Abilities;

public partial class Ability : Node
{
    [Export] public AbilityData? Data { get; set; }

    private float _cooldownRemaining;

    public bool IsReady => _cooldownRemaining <= 0f;

    public float CooldownRemaining => _cooldownRemaining;

    public float CooldownDuration => Data?.CooldownDuration ?? 0f;

    public float CooldownProgress
    {
        get
        {
            if (CooldownDuration <= 0f) return 100f;

            return Mathf.Clamp(
                1f - (_cooldownRemaining / CooldownDuration),
                0f,
                1f
            ) * 100f;
        }
    }

    public override void _Process(double delta)
    {
        if (_cooldownRemaining <= 0f) return;

        _cooldownRemaining -= (float)delta;

        if (_cooldownRemaining <= 0f) _cooldownRemaining = 0f;
    }

    public virtual bool Use()
    {
        if (!IsReady) return false;

        StartCooldown();
        return true;
    }

    public void StartCooldown()
    {
        if (Data == null) return;

        _cooldownRemaining = Mathf.Max(0f, Data.CooldownDuration);
    }

    public void StartCooldown(float duration)
    {
        _cooldownRemaining = Mathf.Max(0f, duration);
    }

    public void SetCooldownDuration(float duration)
    {
        if (Data == null) return;

        Data.CooldownDuration = Mathf.Max(0f, duration);
    }

    public void ResetCooldown()
    {
        _cooldownRemaining = 0f;
    }
}