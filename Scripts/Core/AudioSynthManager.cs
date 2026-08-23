using System;
using System.Collections.Generic;
using Godot;

namespace NightFall.Scripts.Core;

public partial class AudioSynthManager : Node
{
    public static AudioSynthManager? Instance { get; private set; }

    public static float MasterVolume { get; set; } = 1.0f;
    public static float SfxVolume { get; set; } = 1.0f;
    public static float MusicVolume { get; set; } = 1.0f;
    public static bool ScreenShakeEnabled { get; set; } = true;

    private readonly Dictionary<string, AudioStreamWav> _sfxCache = [];

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            QueueFree();
        }
    }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        PrecacheSounds();
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void PrecacheSounds()
    {
        _sfxCache["ui_hover"] = CreateToneWav(800f, 600f, 0.03f, 0.2f, false);
        _sfxCache["ui_click"] = CreateToneWav(400f, 150f, 0.06f, 0.4f, false);
        _sfxCache["slash"] = CreateNoiseWav(0.12f, 0.5f, 600f, 150f);
        _sfxCache["hit"] = CreateNoiseWav(0.10f, 0.6f, 250f, 80f);
        _sfxCache["player_hurt"] = CreateToneWav(180f, 60f, 0.20f, 0.7f, true);
        _sfxCache["enemy_death"] = CreateNoiseWav(0.25f, 0.6f, 400f, 40f);
        _sfxCache["blink"] = CreateToneWav(300f, 900f, 0.15f, 0.5f, false);
        _sfxCache["gravity_well"] = CreateToneWav(120f, 350f, 0.30f, 0.6f, true);
        _sfxCache["gold"] = CreateChimeWav(
            [987.77f, 1318.51f],
            0.12f,
            0.4f
        );
        _sfxCache["buy"] = CreateChimeWav(
            [523.25f, 659.25f, 783.99f],
            0.22f,
            0.5f
        );
    }

    public static void EnsureInstance(Node owner)
    {
        if (Instance != null || !GodotObject.IsInstanceValid(owner))
            return;

        var tree = owner.GetTree();

        if (tree == null)
            return;

        var root = tree.Root;

        var existing = root.GetNodeOrNull<AudioSynthManager>("AudioSynthManager");

        if (existing != null)
        {
            Instance = existing;
            return;
        }

        var manager = new AudioSynthManager
        {
            Name = "AudioSynthManager"
        };

        Instance = manager;

        root.CallDeferred(Node.MethodName.AddChild, manager);
    }

    public static void PlaySfx(string soundName)
    {
        var instance = Instance;

        if (instance == null ||
            !GodotObject.IsInstanceValid(instance) ||
            !instance.IsInsideTree() ||
            MasterVolume <= 0.01f ||
            SfxVolume <= 0.01f)
        {
            return;
        }

        if (!instance._sfxCache.TryGetValue(soundName, out var stream))
            return;

        var player = new AudioStreamPlayer
        {
            Stream = stream,
            VolumeDb = Mathf.LinearToDb(
                Mathf.Clamp(MasterVolume * SfxVolume, 0.0001f, 1.0f)
            ),
            ProcessMode = ProcessModeEnum.Always
        };

        instance.AddChild(player);
        player.Play();

        player.Finished += player.QueueFree;
    }

    public static void PlayUiHover() => PlaySfx("ui_hover");
    public static void PlayUiClick() => PlaySfx("ui_click");
    public static void PlaySlash() => PlaySfx("slash");
    public static void PlayHit() => PlaySfx("hit");
    public static void PlayPlayerHurt() => PlaySfx("player_hurt");
    public static void PlayEnemyDeath() => PlaySfx("enemy_death");
    public static void PlayBlink() => PlaySfx("blink");
    public static void PlayGravityWell() => PlaySfx("gravity_well");
    public static void PlayGold() => PlaySfx("gold");
    public static void PlayBuy() => PlaySfx("buy");

    private static AudioStreamWav CreateToneWav(
        float startFreq,
        float endFreq,
        float duration,
        float volume,
        bool addSubHarmonic)
    {
        const int sampleRate = 22050;

        int numSamples = Mathf.Max(1, (int)(sampleRate * duration));
        byte[] pcmData = new byte[numSamples * 2];

        double phase = 0;

        for (int i = 0; i < numSamples; i++)
        {
            float t = (float)i / numSamples;
            float freq = Mathf.Lerp(startFreq, endFreq, t);

            phase += 2.0 * Math.PI * freq / sampleRate;

            float sampleVal = (float)Math.Sin(phase);

            if (addSubHarmonic)
            {
                sampleVal =
                    0.7f * sampleVal +
                    0.3f * (float)Math.Sin(phase * 0.5);
            }

            float envelope = 1.0f - t;
            envelope *= envelope;

            short sample16 = (short)(
                sampleVal *
                envelope *
                volume *
                32767f
            );

            WriteSample(pcmData, i, sample16);
        }

        return new AudioStreamWav
        {
            Format = AudioStreamWav.FormatEnum.Format16Bits,
            MixRate = sampleRate,
            Data = pcmData
        };
    }

    private static AudioStreamWav CreateNoiseWav(
        float duration,
        float volume,
        float startCutoff,
        float endCutoff)
    {
        const int sampleRate = 22050;

        int numSamples = Mathf.Max(1, (int)(sampleRate * duration));
        byte[] pcmData = new byte[numSamples * 2];

        Random rand = new();
        float filteredVal = 0;

        for (int i = 0; i < numSamples; i++)
        {
            float t = (float)i / numSamples;

            float cutoff = Mathf.Lerp(
                startCutoff,
                endCutoff,
                t
            );

            float alpha = Mathf.Clamp(
                2.0f * (float)Math.PI * cutoff / sampleRate,
                0.01f,
                0.99f
            );

            float rawNoise =
                (float)rand.NextDouble() * 2.0f - 1.0f;

            filteredVal += alpha * (rawNoise - filteredVal);

            float envelope = (1.0f - t) * (1.0f - t);

            short sample16 = (short)(
                filteredVal *
                envelope *
                volume *
                32767f
            );

            WriteSample(pcmData, i, sample16);
        }

        return new AudioStreamWav
        {
            Format = AudioStreamWav.FormatEnum.Format16Bits,
            MixRate = sampleRate,
            Data = pcmData
        };
    }

    private static AudioStreamWav CreateChimeWav(
        float[] freqs,
        float duration,
        float volume)
    {
        const int sampleRate = 22050;

        if (freqs.Length == 0)
        {
            throw new ArgumentException(
                "At least one frequency is required.",
                nameof(freqs)
            );
        }

        int numSamples = Mathf.Max(1, (int)(sampleRate * duration));
        byte[] pcmData = new byte[numSamples * 2];

        int samplesPerNote = Mathf.Max(
            1,
            numSamples / freqs.Length
        );

        for (int i = 0; i < numSamples; i++)
        {
            int noteIndex = Math.Min(
                i / samplesPerNote,
                freqs.Length - 1
            );

            float freq = freqs[noteIndex];

            float tInNote =
                (float)(i % samplesPerNote) / samplesPerNote;

            double phase =
                2.0 *
                Math.PI *
                freq *
                (i / (double)sampleRate);

            float sampleVal = (float)Math.Sin(phase);
            float envelope = 1.0f - tInNote;

            short sample16 = (short)(
                sampleVal *
                envelope *
                volume *
                32767f
            );

            WriteSample(pcmData, i, sample16);
        }

        return new AudioStreamWav
        {
            Format = AudioStreamWav.FormatEnum.Format16Bits,
            MixRate = sampleRate,
            Data = pcmData
        };
    }

    private static void WriteSample(
        byte[] pcmData,
        int sampleIndex,
        short sample)
    {
        int offset = sampleIndex * 2;

        pcmData[offset] = (byte)(sample & 0xFF);
        pcmData[offset + 1] = (byte)((sample >> 8) & 0xFF);
    }
}