using Godot;

namespace NightFall.Scripts.Ui;

public partial class DungeonSetupAnimation : Control
{
    [Export] private float _entryDuration = 0.7f;
    [Export] private float _exitDuration = 0.35f;
    [Export] private float _panelDelay = 0.08f;
    [Export] private float _buttonStagger = 0.055f;

    private static readonly Color TransparentColor = new(1f, 1f, 1f, 0f);
    private static readonly Vector2 MainEntryScale = new(0.97f, 0.97f);
    private static readonly Vector2 SetupEntryScale = new(0.98f, 0.98f);
    private static readonly Vector2 PreviewEntryScale = new(0.96f, 0.96f);
    private static readonly Vector2 ExitScale = new(0.985f, 0.985f);

    private DungeonSetupAnimationNodes _nodes = null!;

    private Vector2 _mainLayoutScale;
    private Vector2 _setupPanelScale;
    private Vector2 _previewPanelScale;

    private Tween? _entryTween;
    private Tween? _skipTween;
    private Tween? _exitTween;

    private bool _entryFinished;
    private bool _skipRequested;

    public bool IsTransitioning { get; private set; }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        _nodes = new DungeonSetupAnimationNodes(this);

        PrepareForEntry();
        CallDeferred(nameof(PlayEntryAnimation));
    }

    public override void _Input(InputEvent @event)
    {
        if (_entryFinished || _skipRequested) return;
        if (@event is not InputEventKey { Pressed: true, Echo: false } keyEvent) return;
        if (keyEvent.Keycode is not (Key.Enter or Key.KpEnter)) return;

        GetViewport().SetInputAsHandled();
        SkipEntry();
    }

    private void PrepareForEntry()
    {
        _mainLayoutScale = _nodes.MainLayout.Scale;
        _setupPanelScale = _nodes.SetupPanel.Scale;
        _previewPanelScale = _nodes.PreviewPanel.Scale;

        SetModulate(
            TransparentColor,
            _nodes.MainLayout,
            _nodes.SetupPanel,
            _nodes.PreviewPanel,
            _nodes.TopLine,
            _nodes.SideRule,
            _nodes.HorizonGlow,
            _nodes.SetupEyebrow,
            _nodes.SetupTitle,
            _nodes.TitleRule,
            _nodes.PreviewEyebrow,
            _nodes.PreviewTitle,
            _nodes.PreviewRule,
            _nodes.BackButton,
            _nodes.StartButton,
            _nodes.SkipLabel);

        _nodes.MainLayout.Scale = MainEntryScale;
        _nodes.SetupPanel.Scale = SetupEntryScale;
        _nodes.PreviewPanel.Scale = PreviewEntryScale;

        SetModulate(TransparentColor, _nodes.ModifierButtons);
    }

    private void PlayEntryAnimation()
    {
        if (_skipRequested) return;

        IsTransitioning = true;
        _entryTween = CreateTween();

        AnimateDecorations(_entryTween);
        AnimateSetupPanel(_entryTween);
        AnimateModifierButtons(_entryTween);
        AnimateActionButtons(_entryTween);
        AnimatePreviewPanel(_entryTween);

        _entryTween.TweenCallback(Callable.From(FinishEntryAnimation));
        StartSkipHintAnimation();
    }

    private void AnimateDecorations(Tween tween)
    {
        tween.SetParallel();

        FadeIn(tween, _nodes.TopLine, _entryDuration * 0.5f);
        FadeIn(tween, _nodes.SideRule, _entryDuration * 0.7f);
        FadeInWithScale(tween, _nodes.MainLayout, _mainLayoutScale, _entryDuration);
        FadeIn(tween, _nodes.HorizonGlow, _entryDuration);

        tween.SetParallel(false);
        tween.TweenInterval(_panelDelay);
    }

    private void AnimateSetupPanel(Tween tween)
    {
        FadeInWithScale(
            tween,
            _nodes.SetupPanel,
            _setupPanelScale,
            _entryDuration * 0.65f);

        FadeInParallel(
            tween,
            0.28f,
            _nodes.SetupEyebrow,
            _nodes.SetupTitle,
            _nodes.TitleRule);

        tween.TweenInterval(0.12f);
    }

    private void AnimateModifierButtons(Tween tween)
    {
        foreach (var button in _nodes.ModifierButtons)
        {
            FadeIn(tween, button, 0.2f);
            tween.TweenInterval(_buttonStagger);
        }

        tween.TweenInterval(0.08f);
    }

    private void AnimateActionButtons(Tween tween)
    {
        FadeInParallel(
            tween,
            0.3f,
            _nodes.BackButton,
            _nodes.StartButton);

        tween.TweenInterval(0.05f);
    }

    private void AnimatePreviewPanel(Tween tween)
    {
        FadeInWithScale(
            tween,
            _nodes.PreviewPanel,
            _previewPanelScale,
            0.4f);

        FadeInParallel(
            tween,
            0.3f,
            _nodes.PreviewEyebrow,
            _nodes.PreviewTitle,
            _nodes.PreviewRule);
    }

    private void StartSkipHintAnimation()
    {
        if (_skipRequested || !IsInstanceValid(_nodes.SkipLabel)) return;

        KillTween(ref _skipTween);

        _nodes.SkipLabel.Modulate = TransparentColor;
        _skipTween = CreateTween();

        _skipTween
            .TweenProperty(
                _nodes.SkipLabel,
                "modulate:a",
                1f,
                0.25f)
            .SetDelay(0.55f);

        _skipTween.TweenProperty(
            _nodes.SkipLabel,
            "modulate:a",
            0.45f,
            0.65f);

        _skipTween.TweenProperty(
            _nodes.SkipLabel,
            "modulate:a",
            1f,
            0.65f);

        _skipTween.SetLoops();
    }

    private void FinishEntryAnimation()
    {
        if (_skipRequested) return;

        _entryFinished = true;
        IsTransitioning = false;
        _entryTween = null;

        FadeOutSkipHint();
    }

    private void SkipEntry()
    {
        if (_entryFinished || _skipRequested) return;

        _skipRequested = true;
        _entryFinished = true;
        IsTransitioning = false;

        KillTween(ref _entryTween);
        KillTween(ref _skipTween);

        SetFinalVisualState();
        FadeOutSkipHint(0.12f);
    }

    private void SetFinalVisualState()
    {
        SetModulate(
            Colors.White,
            _nodes.MainLayout,
            _nodes.SetupPanel,
            _nodes.PreviewPanel,
            _nodes.TopLine,
            _nodes.SideRule,
            _nodes.HorizonGlow,
            _nodes.SetupEyebrow,
            _nodes.SetupTitle,
            _nodes.TitleRule,
            _nodes.PreviewEyebrow,
            _nodes.PreviewTitle,
            _nodes.PreviewRule,
            _nodes.BackButton,
            _nodes.StartButton);

        SetModulate(Colors.White, _nodes.ModifierButtons);

        _nodes.MainLayout.Scale = _mainLayoutScale;
        _nodes.SetupPanel.Scale = _setupPanelScale;
        _nodes.PreviewPanel.Scale = _previewPanelScale;
    }

    private void FadeOutSkipHint(float duration = 0.2f)
    {
        if (!IsInstanceValid(_nodes.SkipLabel)) return;

        KillTween(ref _skipTween);

        CreateTween()
            .TweenProperty(
                _nodes.SkipLabel,
                "modulate:a",
                0f,
                duration);
    }

    public void PlayExitAnimation(Callable callback)
    {
        if (IsTransitioning) return;

        IsTransitioning = true;

        KillTween(ref _skipTween);
        KillTween(ref _exitTween);

        _exitTween = CreateTween();

        AnimateExit(_exitTween);
        _exitTween.TweenCallback(callback);
    }

    private void AnimateExit(Tween tween)
    {
        tween.SetParallel();

        FadeOut(
            tween,
            _nodes.SetupPanel,
            _exitDuration,
            Tween.TransitionType.Quad);

        FadeOut(
            tween,
            _nodes.PreviewPanel,
            _exitDuration * 0.9f,
            Tween.TransitionType.Quad);

        FadeOut(tween, _nodes.TopLine, _exitDuration * 0.8f);
        FadeOut(tween, _nodes.SideRule, _exitDuration * 0.8f);
        FadeOut(tween, _nodes.HorizonGlow, _exitDuration * 0.8f);

        tween.TweenProperty(
                _nodes.MainLayout,
                "scale",
                ExitScale,
                _exitDuration)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.In);

        tween.SetParallel(false);
    }

    private static void FadeIn(
        Tween tween,
        CanvasItem target,
        float duration) =>
        tween.TweenProperty(
            target,
            "modulate:a",
            1f,
            duration);

    private static void FadeOut(
        Tween tween,
        CanvasItem target,
        float duration,
        Tween.TransitionType transition = Tween.TransitionType.Linear) =>
        tween.TweenProperty(
                target,
                "modulate:a",
                0f,
                duration)
            .SetTrans(transition)
            .SetEase(Tween.EaseType.In);

    private static void FadeInWithScale(
        Tween tween,
        Control target,
        Vector2 finalScale,
        float duration)
    {
        tween.SetParallel();

        FadeIn(tween, target, duration);

        tween.TweenProperty(
                target,
                "scale",
                finalScale,
                duration)
            .SetTrans(Tween.TransitionType.Quart)
            .SetEase(Tween.EaseType.Out);

        tween.SetParallel(false);
    }

    private static void FadeInParallel(
        Tween tween,
        float duration,
        params CanvasItem[] targets)
    {
        tween.SetParallel();

        foreach (var target in targets) FadeIn(tween, target, duration);

        tween.SetParallel(false);
    }

    private static void SetModulate(
        Color color,
        params CanvasItem[] targets)
    {
        foreach (var target in targets) target.Modulate = color;
    }

    private static void SetModulate(Color color, Button[] buttons)
    {
        foreach (var button in buttons) button.Modulate = color;
    }

    private static void KillTween(ref Tween? tween)
    {
        tween?.Kill();
        tween = null;
    }
}