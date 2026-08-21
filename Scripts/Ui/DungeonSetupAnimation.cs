using Godot;

namespace NightFall.Scripts.Ui;

public partial class DungeonSetupAnimation : Control
{
    [Export] private float _entryDuration = 0.7f;
    [Export] private float _exitDuration = 0.35f;
    [Export] private float _panelDelay = 0.08f;
    [Export] private float _buttonStagger = 0.055f;

    private Control _mainLayout = null!;
    private Control _setupPanel = null!;
    private Control _previewPanel = null!;

    private ColorRect _topLine = null!;
    private ColorRect _sideRule = null!;
    private ColorRect _horizonGlow = null!;

    private Label _setupEyebrow = null!;
    private Label _setupTitle = null!;
    private ColorRect _titleRule = null!;

    private Label _previewEyebrow = null!;
    private Label _previewTitle = null!;
    private ColorRect _previewRule = null!;

    private Button[] _modifierButtons = null!;
    private Button _backButton = null!;
    private Button _startButton = null!;

    private Label _skipLabel = null!;

    private Vector2 _mainLayoutScale = Vector2.One;
    private Vector2 _setupPanelScale = Vector2.One;
    private Vector2 _previewPanelScale = Vector2.One;

    private Tween? _entryTween;
    private Tween? _skipTween;
    private Tween? _exitTween;

    private bool _entryFinished;
    private bool _skipRequested;

    public bool IsTransitioning { get; private set; }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        CacheNodes();

        PrepareForEntry();

        CallDeferred(nameof(PlayEntryAnimation));
    }

public override void _Input(InputEvent @event)
{
    if (_entryFinished || _skipRequested)
        return;

    if (@event is InputEventKey { Pressed: true, Echo: false } keyEvent &&
        (keyEvent.Keycode == Key.Enter ||
         keyEvent.Keycode == Key.KpEnter))
    {
        GetViewport().SetInputAsHandled();

        SkipEntry();
    }
}

    private void CacheNodes()
    {
        _mainLayout = GetNode<Control>(
            "../CenterContainer/MainLayout"
        );

        _setupPanel = GetNode<Control>(
            "../CenterContainer/MainLayout/SetupPanel"
        );

        _previewPanel = GetNode<Control>(
            "../CenterContainer/MainLayout/PreviewPanel"
        );

        _topLine = GetNode<ColorRect>(
            "../TopLine"
        );

        _sideRule = GetNode<ColorRect>(
            "../SideRule"
        );

        _horizonGlow = GetNode<ColorRect>(
            "../HorizonGlow"
        );

        _setupEyebrow = GetNode<Label>(
            "../CenterContainer/MainLayout/SetupPanel/Content/Eyebrow"
        );

        _setupTitle = GetNode<Label>(
            "../CenterContainer/MainLayout/SetupPanel/Content/Title"
        );

        _titleRule = GetNode<ColorRect>(
            "../CenterContainer/MainLayout/SetupPanel/Content/TitleRule"
        );

        _previewEyebrow = GetNode<Label>(
            "../CenterContainer/MainLayout/PreviewPanel/Content/Eyebrow"
        );

        _previewTitle = GetNode<Label>(
            "../CenterContainer/MainLayout/PreviewPanel/Content/Title"
        );

        _previewRule = GetNode<ColorRect>(
            "../CenterContainer/MainLayout/PreviewPanel/Content/Rule"
        );

        _backButton = GetNode<Button>(
            "../CenterContainer/MainLayout/SetupPanel/Content/Buttons/BackButton"
        );

        _startButton = GetNode<Button>(
            "../CenterContainer/MainLayout/SetupPanel/Content/Buttons/StartButton"
        );

        _modifierButtons =
        [
            GetNode<Button>(
                "../CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/BloodMoon"
            ),

            GetNode<Button>(
                "../CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/GlassCannon"
            ),

            GetNode<Button>(
                "../CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/HardNight"
            ),

            GetNode<Button>(
                "../CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/Greed"
            ),

            GetNode<Button>(
                "../CenterContainer/MainLayout/SetupPanel/Content/ModifierPanel/ModifierMargin/ModifierList/Fragile"
            )
        ];

        /*
         * IMPORTANT:
         * This assumes DungeonSetupAnimation is a child of
         * DungeonSetup, just like the rest of your animation
         * setup currently is.
         *
         * If SkipHint is a direct child of DungeonSetupAnimation
         * instead, change this to "SkipHint".
         */
        _skipLabel = GetNode<Label>(
            "../SkipHint"
        );
    }

    private void PrepareForEntry()
    {
        _mainLayoutScale = _mainLayout.Scale;
        _setupPanelScale = _setupPanel.Scale;
        _previewPanelScale = _previewPanel.Scale;

        _mainLayout.Modulate = Transparent();
        _mainLayout.Scale = new Vector2(0.97f, 0.97f);

        _setupPanel.Modulate = Transparent();
        _setupPanel.Scale = new Vector2(0.98f, 0.98f);

        _previewPanel.Modulate = Transparent();
        _previewPanel.Scale = new Vector2(0.96f, 0.96f);

        _setupEyebrow.Modulate = Transparent();
        _setupTitle.Modulate = Transparent();
        _titleRule.Modulate = Transparent();

        _previewEyebrow.Modulate = Transparent();
        _previewTitle.Modulate = Transparent();
        _previewRule.Modulate = Transparent();

        _topLine.Modulate = Transparent();
        _sideRule.Modulate = Transparent();
        _horizonGlow.Modulate = Transparent();

        foreach (Button button in _modifierButtons)
            button.Modulate = Transparent();

        _backButton.Modulate = Transparent();
        _startButton.Modulate = Transparent();

        _skipLabel.Modulate = Transparent();
    }

    private void PlayEntryAnimation()
    {
        if (_skipRequested)
            return;

        IsTransitioning = true;
        _entryFinished = false;

        _entryTween = CreateTween();

        Tween tween = _entryTween;

        tween.SetParallel(true);

        // Top accent line.
        tween.TweenProperty(
            _topLine,
            "modulate:a",
            1f,
            _entryDuration * 0.5f
        );

        // Left decorative rule.
        tween.TweenProperty(
            _sideRule,
            "modulate:a",
            1f,
            _entryDuration * 0.7f
        );

        // Main layout fade + scale.
        tween.TweenProperty(
                _mainLayout,
                "modulate:a",
                1f,
                _entryDuration
            )
            .SetTrans(Tween.TransitionType.Quart)
            .SetEase(Tween.EaseType.Out);

        tween.TweenProperty(
                _mainLayout,
                "scale",
                _mainLayoutScale,
                _entryDuration
            )
            .SetTrans(Tween.TransitionType.Quart)
            .SetEase(Tween.EaseType.Out);

        // Bottom glow.
        tween.TweenProperty(
            _horizonGlow,
            "modulate:a",
            1f,
            _entryDuration
        );

        tween.SetParallel(false);

        // Give the overall layout a little breathing room.
        tween.TweenInterval(_panelDelay);

        // Setup panel.
        tween.TweenProperty(
            _setupPanel,
            "modulate:a",
            1f,
            _entryDuration * 0.65f
        );

        tween.TweenProperty(
                _setupPanel,
                "scale",
                _setupPanelScale,
                _entryDuration * 0.65f
            )
            .SetTrans(Tween.TransitionType.Quart)
            .SetEase(Tween.EaseType.Out);

        // Setup heading elements.
        tween.SetParallel(true);

        tween.TweenProperty(
            _setupEyebrow,
            "modulate:a",
            1f,
            0.28f
        );

        tween.TweenProperty(
            _setupTitle,
            "modulate:a",
            1f,
            0.35f
        );

        tween.TweenProperty(
            _titleRule,
            "modulate:a",
            1f,
            0.3f
        );

        tween.SetParallel(false);

        tween.TweenInterval(0.12f);

        // Modifier buttons appear one after another.
        foreach (var t in _modifierButtons)
        {
            tween.SetParallel(true);

            tween.TweenProperty(
                t,
                "modulate:a",
                1f,
                0.2f
            );

            tween.SetParallel(false);

            tween.TweenInterval(_buttonStagger);
        }

        tween.TweenInterval(0.08f);

        // Bottom buttons.
        tween.SetParallel(true);

        tween.TweenProperty(
            _backButton,
            "modulate:a",
            1f,
            0.25f
        );

        tween.TweenProperty(
            _startButton,
            "modulate:a",
            1f,
            0.3f
        );

        tween.SetParallel(false);

        tween.TweenInterval(0.05f);

        // Preview panel.
        tween.TweenProperty(
            _previewPanel,
            "modulate:a",
            1f,
            0.4f
        );

        tween.TweenProperty(
                _previewPanel,
                "scale",
                _previewPanelScale,
                0.4f
            )
            .SetTrans(Tween.TransitionType.Quart)
            .SetEase(Tween.EaseType.Out);

        // Preview heading.
        tween.SetParallel(true);

        tween.TweenProperty(
            _previewEyebrow,
            "modulate:a",
            1f,
            0.25f
        );

        tween.TweenProperty(
            _previewTitle,
            "modulate:a",
            1f,
            0.3f
        );

        tween.TweenProperty(
            _previewRule,
            "modulate:a",
            1f,
            0.25f
        );

        tween.SetParallel(false);

        // Entry finished.
        tween.TweenCallback(
            Callable.From(FinishEntryAnimation)
        );

        StartSkipHintAnimation();
    }

    private void StartSkipHintAnimation()
    {
        if (_skipRequested || !IsInstanceValid(_skipLabel))
            return;

        _skipLabel.Modulate = Transparent();

        _skipTween?.Kill();

        _skipTween = CreateTween();

        _skipTween.TweenProperty(
                _skipLabel,
                "modulate:a",
                1f,
                0.25f
            )
            .SetDelay(0.55f);

        _skipTween.TweenProperty(
            _skipLabel,
            "modulate:a",
            0.45f,
            0.65f
        );

        _skipTween.TweenProperty(
            _skipLabel,
            "modulate:a",
            1f,
            0.65f
        );

        _skipTween.SetLoops();
    }

    private void FinishEntryAnimation()
    {
        if (_skipRequested)
            return;

        _entryFinished = true;
        IsTransitioning = false;

        _entryTween = null;

        if (IsInstanceValid(_skipLabel))
        {
            _skipTween?.Kill();
            _skipTween = null;

            Tween fade = CreateTween();

            fade.TweenProperty(
                _skipLabel,
                "modulate:a",
                0f,
                0.2f
            );
        }
    }

    private void SkipEntry()
    {
        if (_entryFinished || _skipRequested)
            return;

        _skipRequested = true;
        _entryFinished = true;

        IsTransitioning = false;

        // Stop the current animations.
        _entryTween?.Kill();
        _entryTween = null;

        _skipTween?.Kill();
        _skipTween = null;

        // Immediately put every animated element
        // into its final visual state.

        _mainLayout.Modulate = Colors.White;
        _mainLayout.Scale = _mainLayoutScale;

        _setupPanel.Modulate = Colors.White;
        _setupPanel.Scale = _setupPanelScale;

        _previewPanel.Modulate = Colors.White;
        _previewPanel.Scale = _previewPanelScale;

        _topLine.Modulate = Colors.White;
        _sideRule.Modulate = Colors.White;
        _horizonGlow.Modulate = Colors.White;

        _setupEyebrow.Modulate = Colors.White;
        _setupTitle.Modulate = Colors.White;
        _titleRule.Modulate = Colors.White;

        _previewEyebrow.Modulate = Colors.White;
        _previewTitle.Modulate = Colors.White;
        _previewRule.Modulate = Colors.White;

        foreach (Button button in _modifierButtons)
            button.Modulate = Colors.White;

        _backButton.Modulate = Colors.White;
        _startButton.Modulate = Colors.White;

        // Fade the skip hint away rather than making it pop out.
        if (IsInstanceValid(_skipLabel))
        {
            Tween tween = CreateTween();

            tween.TweenProperty(
                _skipLabel,
                "modulate:a",
                0f,
                0.12f
            );
        }
    }

    public void PlayExitAnimation(Callable callback)
    {
        if (IsTransitioning)
            return;

        IsTransitioning = true;

        _skipTween?.Kill();
        _skipTween = null;

        _exitTween?.Kill();

        _exitTween = CreateTween();

        Tween tween = _exitTween;

        tween.SetParallel(true);

        tween.TweenProperty(
                _setupPanel,
                "modulate:a",
                0f,
                _exitDuration
            )
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.In);

        tween.TweenProperty(
                _previewPanel,
                "modulate:a",
                0f,
                _exitDuration * 0.9f
            )
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.In);

        tween.TweenProperty(
                _mainLayout,
                "scale",
                new Vector2(0.985f, 0.985f),
                _exitDuration
            )
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.In);

        tween.TweenProperty(
            _topLine,
            "modulate:a",
            0f,
            _exitDuration * 0.8f
        );

        tween.TweenProperty(
            _sideRule,
            "modulate:a",
            0f,
            _exitDuration * 0.8f
        );

        tween.TweenProperty(
            _horizonGlow,
            "modulate:a",
            0f,
            _exitDuration * 0.8f
        );

        tween.SetParallel(false);

        tween.TweenCallback(
            callback
        );
    }

    private static Color Transparent()
    {
        return new Color(1f, 1f, 1f, 0f);
    }
}