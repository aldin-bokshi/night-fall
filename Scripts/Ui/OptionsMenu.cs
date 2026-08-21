using Godot;
using NightFall.Scripts.Core;

namespace NightFall.Scripts.Ui;

public partial class OptionsMenu : CanvasLayer
{
    private HSlider _masterSlider = null!;
    private HSlider _sfxSlider = null!;
    private HSlider _musicSlider = null!;
    private CheckButton _shakeCheck = null!;
    private CheckButton _fullscreenCheck = null!;
    private Button _closeButton = null!;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        _masterSlider = GetNode<HSlider>("Panel/Content/VolumeContainer/MasterRow/MasterSlider");
        _sfxSlider = GetNode<HSlider>("Panel/Content/VolumeContainer/SfxRow/SfxSlider");
        _musicSlider = GetNode<HSlider>("Panel/Content/VolumeContainer/MusicRow/MusicSlider");
        _shakeCheck = GetNode<CheckButton>("Panel/Content/TogglesContainer/ShakeCheck");
        _fullscreenCheck = GetNode<CheckButton>("Panel/Content/TogglesContainer/FullscreenCheck");
        _closeButton = GetNode<Button>("Panel/Content/CloseButton");

        _masterSlider.Value = AudioSynthManager.MasterVolume * 100f;
        _sfxSlider.Value = AudioSynthManager.SfxVolume * 100f;
        _musicSlider.Value = AudioSynthManager.MusicVolume * 100f;
        _shakeCheck.ButtonPressed = AudioSynthManager.ScreenShakeEnabled;
        _fullscreenCheck.ButtonPressed = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen;

        _masterSlider.ValueChanged += OnMasterChanged;
        _sfxSlider.ValueChanged += OnSfxChanged;
        _musicSlider.ValueChanged += OnMusicChanged;
        _shakeCheck.Toggled += OnShakeToggled;
        _fullscreenCheck.Toggled += OnFullscreenToggled;
        _closeButton.Pressed += OnClosePressed;

        this.AttachJuiceToTree();
        Hide();
    }

    public void Open()
    {
        Show();
        _closeButton.GrabFocus();
    }

    private void OnMasterChanged(double value)
    {
        AudioSynthManager.MasterVolume = (float)(value / 100.0);
    }

    private void OnSfxChanged(double value)
    {
        AudioSynthManager.SfxVolume = (float)(value / 100.0);
    }

    private void OnMusicChanged(double value)
    {
        AudioSynthManager.MusicVolume = (float)(value / 100.0);
    }

    private void OnShakeToggled(bool toggled)
    {
        AudioSynthManager.ScreenShakeEnabled = toggled;
    }

    private void OnFullscreenToggled(bool toggled)
    {
        DisplayServer.WindowSetMode(toggled ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
    }

    private void OnClosePressed()
    {
        Hide();
    }
}
