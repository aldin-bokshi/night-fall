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

    public int AbilityCount => _abilities.Length;

    public bool AbilityLimitReached => _abilities.Length >= MaxAbilities;

    public override void _Ready()
    {
        RefreshAbilities();
    }

    public static bool TryGetAbilityAction(
        int slotIndex,
        out string action)
    {
        if (slotIndex < 0 || slotIndex >= AbilityActions.Length)
        {
            action = string.Empty;
            return false;
        }

        action = AbilityActions[slotIndex];
        return true;
    }

    public static bool TryGetAbilityKey(
        int slotIndex,
        out Key key)
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
        if (!TryGetAbilityKey(slotIndex, out Key key))
        {
            return string.Empty;
        }

        return key.ToString().ToUpperInvariant();
    }

    /// <summary>
    /// Finds all Ability nodes that are direct children of the manager.
    /// </summary>
    public void RefreshAbilities()
    {
        int maxActiveAbilities = Math.Min(
            MaxAbilities,
            AbilitySlotCount);

        List<Ability> discoveredAbilities = [];

        foreach (Node child in GetChildren())
        {
            if (child is Ability ability)
            {
                discoveredAbilities.Add(ability);
            }
        }

        if (discoveredAbilities.Count > maxActiveAbilities)
        {
            ShowAbilityLimitWarning(maxActiveAbilities);
        }
        else
        {
            _limitWarningShown = false;
        }

        int activeAbilityCount = Math.Min(
            discoveredAbilities.Count,
            maxActiveAbilities);

        _abilities =
        [
            .. discoveredAbilities
                .GetRange(0, activeAbilityCount)
        ];
    }

    private void ShowAbilityLimitWarning(int maxActiveAbilities)
    {
        if (_limitWarningShown)
        {
            return;
        }

        GD.PushWarning(
            $"AbilityManager can only assign {maxActiveAbilities} ability slots. " +
            $"Only the first {maxActiveAbilities} abilities will be active.");

        _limitWarningShown = true;
    }

    /// <summary>
    /// Attempts to activate the ability assigned to a slot.
    /// </summary>
    public bool TryUseAbility(int slotIndex)
    {
        if (slotIndex < 0)
        {
            return false;
        }

        RefreshAbilities();

        if (slotIndex >= _abilities.Length)
        {
            return false;
        }

        return _abilities[slotIndex].Use();
    }

    /// <summary>
    /// Adds an ability under the manager if there is an available slot.
    /// </summary>
    public bool TryAddAbility(Ability? ability)
    {
        if (ability == null)
        {
            return false;
        }

        RefreshAbilities();

        if (_abilities.Contains(ability))
        {
            return true;
        }

        if (_abilities.Length >= MaxAbilities)
        {
            return false;
        }

        Node? parent = ability.GetParent();

        if (parent != this)
        {
            if (parent != null)
            {
                ability.Reparent(this);
            }
            else
            {
                AddChild(ability);
            }
        }

        RefreshAbilities();

        return _abilities.Contains(ability);
    }

    /// <summary>
    /// Removes an ability from the manager and frees it.
    /// </summary>
    public bool RemoveAbility(Ability? ability)
    {
        if (ability == null)
        {
            return false;
        }

        if (ability.GetParent() != this)
        {
            return false;
        }

        RemoveChild(ability);
        ability.QueueFree();

        RefreshAbilities();

        return true;
    }
}