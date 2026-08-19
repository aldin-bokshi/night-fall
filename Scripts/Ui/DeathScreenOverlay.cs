using Godot;

namespace NightFall.Scripts.Ui;

public partial class DeathScreenOverlay : CanvasLayer
{
    [Export] private float _fadeDuration = 2.5f;
    [Export] private float _finalRedOpacity = 0.65f;

    private ColorRect _topRedOverlay = null!;
    private ColorRect _bottomRedOverlay = null!;
    private Control _content = null!;

    private Label _title = null!;
    private Label _quote = null!;
    private Label _roomsCleared = null!;
    private Label _enemiesSlain = null!;
    private Label _goldCollected = null!;
    private Label _time = null!;
    private Button _retryButton = null!;

    private static readonly string[] DeathQuotes =
    [
        "You died. The enemies remain deeply unimpressed.",
        "That probably could have gone better.",
        "Skill issue.",
        "The night won this round.",
        "You were doing great. Until you weren't.",
        "Have you considered not getting hit?",
        "Death is just a very aggressive loading screen.",
        "At least you looked cool for a second.",
        "The enemies would like to thank you for your contribution.",
        "You fought bravely. Unfortunately, they fought better.",
        "Perhaps violence wasn't the answer.",
        "Your health bar has left the chat.",
        "You have successfully discovered the death mechanic.",
        "That was definitely intentional.",
        "The floor appears to be stronger than expected.",
        "Maybe try dodging next time.",
        "You lasted longer than someone else would have.",
        "Congratulations! You found the Game Over screen.",
        "The darkness sends its regards.",
        "You have been promoted to spectator."
    ];

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        _topRedOverlay = GetNode<ColorRect>("TopRedOverlay");
        _bottomRedOverlay = GetNode<ColorRect>("BottomRedOverlay");
        _content = GetNode<Control>("Content");

        _title = GetNode<Label>("Content/VBoxContainer/Title");
        _quote = GetNode<Label>("Content/VBoxContainer/Quote");
        _roomsCleared = GetNode<Label>(
            "Content/VBoxContainer/RoomsCleared"
        );
        _enemiesSlain = GetNode<Label>(
            "Content/VBoxContainer/EnemiesSlain"
        );
        _goldCollected = GetNode<Label>(
            "Content/VBoxContainer/GoldCollected"
        );
        _time = GetNode<Label>(
            "Content/VBoxContainer/Time"
        );
        _retryButton = GetNode<Button>(
            "Content/VBoxContainer/RetryButton"
        );

        Hide();

        _retryButton.Pressed += OnRetryPressed;
    }

    public void ShowDeathScreen(
        int roomsCleared = 0,
        int enemiesSlain = 0,
        int goldCollected = 0,
        float time = 0f
    )
    {
        GetTree().Paused = true;

        Show();

        SetPlaceholderStats(
            roomsCleared,
            enemiesSlain,
            goldCollected,
            time
        );

        SelectRandomQuote();
        StartDeathAnimation();
    }

    private void SetPlaceholderStats(
        int roomsCleared,
        int enemiesSlain,
        int goldCollected,
        float time
    )
    {
        _roomsCleared.Text =
            $"Rooms Cleared        {roomsCleared}";

        _enemiesSlain.Text =
            $"Enemies Slain        {enemiesSlain}";

        _goldCollected.Text =
            $"Gold Collected       {goldCollected}";

        _time.Text =
            $"Time                 {FormatTime(time)}";
    }

    private void SelectRandomQuote()
    {
        int index = GD.RandRange(0, DeathQuotes.Length - 1);

        _quote.Text = $"\"{DeathQuotes[index]}\"";
    }

    private void StartDeathAnimation()
    {
        float screenHeight =
            GetViewport().GetVisibleRect().Size.Y;

        _topRedOverlay.Modulate =
            new Color(1f, 1f, 1f, 0f);

        _bottomRedOverlay.Modulate =
            new Color(1f, 1f, 1f, 0f);

        _topRedOverlay.OffsetBottom = 0f;
        _bottomRedOverlay.OffsetTop = 0f;

        _content.Modulate =
            new Color(1f, 1f, 1f, 0f);

        Tween tween = CreateTween();

        tween.SetParallel(true);

        tween.TweenProperty(
                _topRedOverlay,
                "offset_bottom",
                screenHeight / 2f,
                _fadeDuration
            )
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.InOut);

        tween.TweenProperty(
                _bottomRedOverlay,
                "offset_top",
                -screenHeight / 2f,
                _fadeDuration
            )
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.InOut);

        tween.TweenProperty(
                _topRedOverlay,
                "modulate:a",
                _finalRedOpacity,
                _fadeDuration
            )
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.InOut);

        tween.TweenProperty(
                _bottomRedOverlay,
                "modulate:a",
                _finalRedOpacity,
                _fadeDuration
            )
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.InOut);

        tween.SetParallel(false);

        tween.TweenCallback(
            Callable.From(ShowDeathContent)
        );
    }

    private void ShowDeathContent()
    {
        Tween tween = CreateTween();

        tween.TweenProperty(
                _content,
                "modulate:a",
                1f,
                0.8f
            )
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);

        tween.TweenCallback(
            Callable.From(() => _retryButton.GrabFocus())
        );
    }

    private void OnRetryPressed()
    {
        GetTree().Paused = false;

        GetTree().ReloadCurrentScene();
    }

    private static string FormatTime(float seconds)
    {
        int totalSeconds =
            Mathf.Max(0, Mathf.FloorToInt(seconds));

        int minutes = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;

        return $"{minutes:00}:{remainingSeconds:00}";
    }
}