using Godot;
using NightFall.Scripts.Run;
using System;
using System.Security.Cryptography;
using System.Text;

namespace NightFall.Scripts.Ui;

public partial class DungeonSetup : Control
{
    [Export] private string _mainMenuScenePath =
        "res://Scenes/UI/MainMenu/MainMenu.tscn";

    [Export] private string _dungeonScenePath =
        "res://Scenes/Game.tscn";

    private LineEdit _seedInput = null!;
    private Label _seedPreview = null!;
    private Label _modifierPreview = null!;

    private Button _backButton = null!;
    private Button _startButton = null!;

    private DungeonSetupAnimation _animation = null!;

    private Button[] _modifierButtons = null!;
    private string[] _modifierNames = null!;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        CacheNodes();
        ConnectSignals();

        _seedInput.Clear();

        UpdatePreview();

        CallDeferred(nameof(FocusStartButton));
        this.AttachJuiceToTree();
    }

    private void CacheNodes()
    {
        _seedInput = GetNode<LineEdit>(
            "CenterContainer/MainLayout/SetupPanel/Content/SeedInput");

        _seedPreview = GetNode<Label>(
            "CenterContainer/MainLayout/PreviewPanel/Content/SeedPreview");

        _modifierPreview = GetNode<Label>(
            "CenterContainer/MainLayout/PreviewPanel/Content/ModifierPreview");

        _backButton = GetNode<Button>(
            "CenterContainer/MainLayout/SetupPanel/Content/Buttons/BackButton");

        _startButton = GetNode<Button>(
            "CenterContainer/MainLayout/SetupPanel/Content/Buttons/StartButton");

        _animation = GetNode<DungeonSetupAnimation>("Animation");

        _modifierButtons =
        [
            GetNode<Button>(
                "CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/BloodMoon"),

            GetNode<Button>(
                "CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/GlassCannon"),

            GetNode<Button>(
                "CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/HardNight"),

            GetNode<Button>(
                "CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/Greed"),

            GetNode<Button>(
                "CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/Fragile")
        ];

        _modifierNames =
        [
            "BLOOD MOON",
            "GLASS CANNON",
            "HARD NIGHT",
            "GREED",
            "FRAGILE"
        ];
    }

    private void ConnectSignals()
    {
        _seedInput.TextChanged += OnSeedChanged;

        _backButton.Pressed += OnBackPressed;
        _startButton.Pressed += OnStartRunPressed;

        foreach (Button modifierButton in _modifierButtons)
            modifierButton.Toggled += OnModifierToggled;
    }

    private void FocusStartButton()
    {
        if (IsInstanceValid(_startButton))
            _startButton.GrabFocus();
    }

    private void OnSeedChanged(string text)
    {
        string filtered = FilterSeed(text);

        if (filtered != text)
        {
            int caretPosition = _seedInput.CaretColumn;

            _seedInput.Text = filtered;

            _seedInput.CaretColumn =
                Mathf.Min(caretPosition, filtered.Length);
        }

        UpdatePreview();
    }

    private void OnModifierToggled(bool _)
    {
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        _seedPreview.Text = string.IsNullOrWhiteSpace(_seedInput.Text) ? "RANDOM / GENERATED ON START" : _seedInput.Text;

        StringBuilder selected = new();

        for (int index = 0; index < _modifierButtons.Length; index++)
        {
            if (!_modifierButtons[index].ButtonPressed) continue;

            if (selected.Length <= 0) return;
            selected.Append("\n");

            selected.Append(_modifierNames[index]);
        }

        _modifierPreview.Text =
            selected.Length == 0
                ? "NO MODIFIERS"
                : selected.ToString();
    }

    private void OnBackPressed()
    {
        if (!CanTransition()) return;

        SetButtonsDisabled(true);

        _animation.PlayExitAnimation(
            Callable.From(() => { GetTree().ChangeSceneToFile(_mainMenuScenePath); })
        );
    }

    private void OnStartRunPressed()
    {
        if (!CanTransition()) return;

        string seedText = _seedInput.Text.Trim();

        if (string.IsNullOrEmpty(seedText)) seedText = GenerateRandomSeed();

        RunConfig config = new(
            seedText,
            ConvertSeedToNumber(seedText),
            _modifierButtons[0].ButtonPressed,
            _modifierButtons[1].ButtonPressed,
            _modifierButtons[2].ButtonPressed,
            _modifierButtons[3].ButtonPressed,
            _modifierButtons[4].ButtonPressed
        );

        RunSession.Start(config);

        SetButtonsDisabled(true);

        _animation.PlayExitAnimation(
            Callable.From(() => { GetTree().ChangeSceneToFile(_dungeonScenePath); })
        );
    }

    private bool CanTransition()
    {
        return !_animation.IsTransitioning;
    }

    private void SetButtonsDisabled(bool disabled)
    {
        _backButton.Disabled = disabled;
        _startButton.Disabled = disabled;

        foreach (Button button in _modifierButtons) button.Disabled = disabled;

        _seedInput.Editable = !disabled;
    }

    private static string FilterSeed(string text)
    {
        StringBuilder builder = new(text.Length);

        foreach (char character in text)
        {
            bool isAsciiLetter =
                character is >= 'A' and <= 'Z'
                || character is >= 'a' and <= 'z';

            bool isAsciiDigit =
                character is >= '0' and <= '9';

            if (!isAsciiLetter && !isAsciiDigit) continue;
            builder.Append(char.ToUpperInvariant(character));
        }

        return builder.ToString();
    }

    private const string SeedCharacters =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private static string GenerateRandomSeed()
    {
        StringBuilder builder = new(8);

        for (int index = 0; index < 8; index++)
        {
            int randomIndex =
                GD.RandRange(0, SeedCharacters.Length - 1);

            builder.Append(SeedCharacters[randomIndex]);
        }

        return builder.ToString();
    }

    private static ulong ConvertSeedToNumber(string seedText)
    {
        byte[] hash =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(seedText)
            );

        return BitConverter.ToUInt64(hash, 0);
    }
}
