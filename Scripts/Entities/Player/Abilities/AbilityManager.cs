using System.Collections.Generic;
using System.Linq;
using Godot;

namespace NightFall.Scripts.Entities.Player.Abilities;

public partial class AbilityManager : Node
{
    [Export] public int MaxAbilities { get; set; } = 4;

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

    /// <summary>
    /// Finds all Ability nodes that are direct children of the manager.
    /// </summary>
    public void RefreshAbilities()
    {
        Ability[] discoveredAbilities =
        [
            .. GetChildren()
                .OfType<Ability>()
        ];

        if (discoveredAbilities.Length > MaxAbilities)
        {
            if (!_limitWarningShown)
            {
                GD.PushWarning(
                    $"AbilityManager has more than {MaxAbilities} abilities. " +
                    $"Only the first {MaxAbilities} will be active."
                );

                _limitWarningShown = true;
            }
        }
        else
        {
            _limitWarningShown = false;
        }

        _abilities = discoveredAbilities.Take(MaxAbilities).ToArray();
    }

    /// <summary>
    /// Attempts to activate the ability assigned to an input action.
    /// </summary>
    public bool TryUseAbility(string action)
    {
        if (string.IsNullOrWhiteSpace(action)) return false;

        RefreshAbilities();

        Ability? ability = FindAbility(action);

        return ability?.Use() ?? false;
    }

    /// <summary>
    /// Finds an ability using its InputMap action.
    /// </summary>
    public Ability? FindAbility(string action)
    {
        if (string.IsNullOrWhiteSpace(action)) return null;

        foreach (Ability ability in _abilities)
        {
            if (ability.Data?.InputAction == action)
                return ability;
        }

        return null;
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
