using Godot;

namespace NightFall.Scripts.Entities.Player.Abilities;

[GlobalClass]
public partial class AbilityData : Resource
{
    [Export] public string AbilityName { get; set; } = "ABILITY";
    [Export] public Texture2D? Icon { get; set; }

    [Export(PropertyHint.Range, "0,60,0.1")]
    public float CooldownDuration { get; set; } = 1f;

    [Export] public string InputDisplay { get; set; } = "KEY";

    [Export] public string InputAction { get; set; } = "ability_1";
}