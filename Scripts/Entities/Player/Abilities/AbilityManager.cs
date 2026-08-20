using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace NightFall.Scripts.Entities.Player.Abilities;

public partial class AbilityManager : Node
{
    public const int AbilitySlotCount = 4;

    private static readonly Key[] AbilityKeys =
    [
        Key.Shift,
        Key.Q,
        Key.E,
        Key.R
    ];

    private static readonly string[] AbilityActions =
    [
        "ability_1",
        "ability_2",
        "ability_3",
        "ability_4"
    ];

    [Export] public int MaxAbilities { get; set; } = AbilitySlotCount;

    private Ability[] _abilities = [];
    private bool _limitWarningShown;

    public IReadOnlyList<Ability> Abilities
    {
        get
        {
            RefreshAbilities();
            return _abilities;
        }
    }

    public override void _Ready()
    {
        RefreshAbilities();
    }

    public static bool TryGetAbilityAction(int slotIndex, out string action)
    {
        if (slotIndex < 0 || slotIndex >= AbilityActions.Length)
        {
            action = string.Empty;
            return false;
        }

        action = AbilityActions[slotIndex];
        return true;
    }

    public static bool TryGetAbilityKey(int slotIndex, out Key key)
    {
        if (slotIndex < 0 || slotIndex >= AbilityKeys.Length)
        {
            key = Key.None;
            return false;
        }

        key = AbilityKeys[slotIndex];
        return true;
    }

    public static string GetAbilityKeyLabel(int slotIndex)
    {
        return TryGetAbilityKey(slotIndex, out Key key)
            ? key.ToString().ToUpperInvariant()
            : string.Empty;
    }

    /// <summary>
    /// Finds all Ability nodes that are direct children of the manager.
    /// </summary>
    public void RefreshAbilities()
    {
        int maxActiveAbilities = Math.Min(MaxAbilities, AbilitySlotCount);

        Ability[] discoveredAbilities =
        [
            .. GetChildren()
                .OfType<Ability>()
        ];

        if (discoveredAbilities.Length > maxActiveAbilities)
        {
            if (!_limitWarningShown)
            {
                GD.PushWarning(
                    $"AbilityManager can only assign {maxActiveAbilities} ability slots. " +
                    $"Only the first {maxActiveAbilities} abilities will be active."
                );

                _limitWarningShown = true;
            }
        }
        else
        {
            _limitWarningShown = false;
        }

        _abilities = discoveredAbilities.Take(maxActiveAbilities).ToArray();
    }

    /// <summary>
    /// Attempts to activate the ability assigned to a slot.
    /// </summary>
    public bool TryUseAbility(int slotIndex)
    {
        if (slotIndex < 0) return false;

        RefreshAbilities();

        if (slotIndex >= _abilities.Length)
            return false;

        return _abilities[slotIndex].Use();
    }

    /// <summary>
    /// Adds an ability under the manager if there is an available slot.
    /// </summary>
    public bool TryAddAbility(Ability? ability)
    {
        if (ability == null) return false;

        RefreshAbilities();

        if (_abilities.Contains(ability))
            return true;

        if (_abilities.Length >= MaxAbilities)
            return false;

        if (ability.GetParent() != this)
        {
            if (ability.GetParent() != null)
                ability.Reparent(this);
            else
                AddChild(ability);
        }

        RefreshAbilities();
        return _abilities.Contains(ability);
    }

    /// <summary>
    /// Removes an ability from the manager and frees it.
    /// </summary>
    public bool RemoveAbility(Ability? ability)
    {
        if (ability == null || ability.GetParent() != this) return false;

        RemoveChild(ability);
        ability.QueueFree();
        RefreshAbilities();
        return true;
    }

    public bool CanAddAbility() => _abilities.Length < MaxAbilities;

    public int GetAbilityCount() => _abilities.Length;

    public bool IsAbilityLimitReached() => _abilities.Length >= MaxAbilities;
}
