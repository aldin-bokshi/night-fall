using System.Collections.Generic;
using System.Linq;
using Godot;
using NightFall.Scripts.Ui;
using NightFall.Scripts.Entities.Player.Abilities;

namespace NightFall.Scripts.Entities.Player;

public partial class Player : CharacterBody2D
{
    private PlayerInput _input = null!;
    private PlayerMovement _movement = null!;
    private PlayerCombat _combat = null!;
    private PlayerStats _stats = null!;
    private AttackHitbox _attackHitbox = null!;
    private Sprite2D _sprite = null!;
    private Ability[] _abilities = [];

    public PlayerStats Stats => _stats;
    public Vector2 MovementInput => _input.MovementInput;
    public Vector2 FacingDirection => _input.FacingDirection;
    public IReadOnlyList<Ability> Abilities => _abilities;

    public override void _Ready()
    {
        _input = GetNode<PlayerInput>("PlayerInput");
        _movement = GetNode<PlayerMovement>("PlayerMovement");
        _combat = GetNode<PlayerCombat>("PlayerCombat");
        _stats = GetNode<PlayerStats>("PlayerStats");
        _attackHitbox = GetNode<AttackHitbox>("AttackHitbox");
        _sprite = GetNode<Sprite2D>("Sprite2D");

        _movement.Initialize(_stats);
        _combat.Initialize(_stats, _attackHitbox);
        _abilities = GetChildren()
            .OfType<Ability>()
            .ToArray();
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateSystems(delta);
        UpdateSprite();

        HandleAbilities();

        HandleAttack();
        HandleMovement();

        if (_stats.IsDead) Die();
    }

    private void UpdateSystems(double delta)
    {
        _combat.UpdateAttack(delta);
    }

    private void UpdateSprite()
    {
        if (_input.FacingDirection.X != 0f) _sprite.FlipH = _input.FacingDirection.X < 0f;
    }

    private void HandleAbilities()
    {
        string? action = _input.AbilityActionPressed;

        if (action == null) return;

        foreach (Ability ability in _abilities)
        {
            if (ability.Data?.InputAction != action) continue;

            ability.Use();
            break;
        }

        _input.ConsumeAbility();
    }

    private void HandleAttack()
    {
        if (!_input.AttackPressed) return;

        if (_input.FacingDirection == Vector2.Zero) return;

        if (!_combat.CanAttack()) return;

        _combat.Attack(_input.FacingDirection);
        _input.ConsumeAttack();
    }

    private void HandleMovement()
    {
        _movement.Move(this, _input.MovementInput);
    }

    private void Die()
    {
        DeathScreenOverlay? deathScreen =
            GetTree().CurrentScene?.FindChild(
                "DeathScreen",
                true,
                false
            ) as DeathScreenOverlay;

        if (deathScreen == null)
        {
            GD.PushError("Player could not find DeathScreen in the current scene.");
            return;
        }

        deathScreen.ShowDeathScreen();
    }
}