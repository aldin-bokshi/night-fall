using Godot;

namespace NightFall.Scripts.Entities.Player.Abilities;

// Duplicate this file to create a new ability.
public partial class AbilityTemplate : Ability
{
    // Exported settings belong here.
    // Example:
    // [Export] public float Damage { get; set; } = 30f;

    private Player? _player;

    public override void _Ready()
    {
        base._Ready();

        // Put Player references here if the ability needs them.
        _player = GetParent<Player>();
    }

    public override bool Use()
    {
        if (!IsReady)
            return false;

        // Put targeting logic here.
        // Example: pick a direction, find a target, or search nearby enemies.

        // Put ability-specific gameplay here.
        // Example: deal damage, spawn VFX, move the player, heal, etc.

        // If this ability needs different cooldown behavior, adjust it here
        // before or instead of calling StartCooldown().
        StartCooldown();
        return true;
    }
}
