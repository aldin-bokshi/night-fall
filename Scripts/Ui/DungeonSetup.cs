using System.Text;
using Godot;
using NightFall.Scripts.Core;
using NightFall.Scripts.Run;

namespace NightFall.Scripts.Ui;

public partial class DungeonSetup : Control
{
    private const string SeedCharacters =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private const int RandomSeedLength = 8;

    private LineEdit _seedInput = null!;
    private Label _seedPreview = null!;
    private Label _modifierPreview = null!;

    private Button _backButton = null!;
    private Button _startButton = null!;

    private DungeonSetupAnimation _animation = null!;

    private Button[] _modifierButtons = null!;
    private string[] _modifierNames = null!;

    private bool CanTransition =>
        !_animation.IsTransitioning;

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
        {
            modifierButton.Toggled += OnModifierToggled;
        }
    }

    private void FocusStartButton()
    {
        if (!IsInstanceValid(_startButton))
        {
            return;
        }

        _startButton.GrabFocus();
    }

    private void OnSeedChanged(string _)
    {
        UpdatePreview();
    }

    private void OnModifierToggled(bool _)
    {
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        UpdateSeedPreview();
        UpdateModifierPreview();
    }

    private void UpdateSeedPreview()
    {
        if (string.IsNullOrEmpty(_seedInput.Text))
        {
            _seedPreview.Text =
                "RANDOM / GENERATED ON START";

            return;
        }

        _seedPreview.Text = _seedInput.Text;
    }

    private void UpdateModifierPreview()
    {
        StringBuilder selected = new();

        for (int index = 0;
             index < _modifierButtons.Length;
             index++)
        {
            if (!_modifierButtons[index].ButtonPressed)
            {
                continue;
            }

            if (selected.Length > 0)
            {
                selected.Append('\n');
            }

            selected.Append(_modifierNames[index]);
        }

        if (selected.Length == 0)
        {
            _modifierPreview.Text = "NO MODIFIERS";
            return;
        }

        _modifierPreview.Text = selected.ToString();
    }

    private void OnBackPressed()
    {
        if (!CanTransition)
        {
            return;
        }

        SetButtonsDisabled(true);

        _animation.PlayExitAnimation(
            Callable.From(ReturnToMainMenu));
    }

    private void ReturnToMainMenu()
    {
        GetTree().ChangeSceneToFile(
            GamePaths.MainMenu);
    }

    private void OnStartRunPressed()
    {
        if (!CanTransition)
        {
            return;
        }

        string seedText = _seedInput.Text;

        if (string.IsNullOrEmpty(seedText))
        {
            seedText = GenerateRandomSeed();
        }

        RunConfig config = CreateRunConfig(seedText);

        RunSession.Start(config);

        SetButtonsDisabled(true);

        _animation.PlayExitAnimation(
            Callable.From(StartGame));
    }

    private RunConfig CreateRunConfig(string seedText)
    {
        return RunConfig.Create(
            seedText,
            _modifierButtons[0].ButtonPressed,
            _modifierButtons[1].ButtonPressed,
            _modifierButtons[2].ButtonPressed,
            _modifierButtons[3].ButtonPressed,
            _modifierButtons[4].ButtonPressed);
    }

    private void StartGame()
    {
        GetTree().ChangeSceneToFile(
            GamePaths.GameScene);
    }

    private void SetButtonsDisabled(bool disabled)
    {
        _backButton.Disabled = disabled;
        _startButton.Disabled = disabled;

        foreach (Button button in _modifierButtons)
        {
            button.Disabled = disabled;
        }

        _seedInput.Editable = !disabled;
    }

    private static string GenerateRandomSeed()
    {
        StringBuilder builder =
            new(RandomSeedLength);

        for (int index = 0;
             index < RandomSeedLength;
             index++)
        {
            int randomIndex = GD.RandRange(
                0,
                SeedCharacters.Length - 1);

            builder.Append(
                SeedCharacters[randomIndex]);
        }

        return builder.ToString();
    }
}