// 261028_code
// 261028_documentation

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Cheron.Games.Solitaire;

/// <summary>Provides optional synthesized sounds, rejected-move feedback, and a brief win celebration.</summary>
public sealed partial class SolitaireGame
{
    /// <summary>The sound button bounds, shared by drawing and pointer input.</summary>
    private static readonly Rectangle SoundButtonBounds = new(1008, 20, 68, 44);

    /// <summary>The lifetime of rejected-move outlines and instructions.</summary>
    private static readonly TimeSpan InvalidFeedbackDuration = TimeSpan.FromSeconds(1.4);

    /// <summary>The duration of the nonblocking win celebration.</summary>
    private static readonly TimeSpan CelebrationDuration = TimeSpan.FromSeconds(3);

    /// <summary>The module-owned sounds, ordered by <see cref="FeedbackSound"/>.</summary>
    private readonly List<SoundEffect> _feedbackSounds = new();

    /// <summary>The module-owned playback instances, stopped immediately when muted.</summary>
    private readonly List<SoundEffectInstance> _feedbackVoices = new();

    /// <summary>Whether sounds are enabled for this module session, preserved across deals.</summary>
    private bool _soundEnabled = true;

    /// <summary>Whether the audio device could not be initialized or used.</summary>
    private bool _audioUnavailable;

    /// <summary>The time remaining for the current rejected-move feedback.</summary>
    private TimeSpan _invalidTimeRemaining;

    /// <summary>The rejected destination or source card to outline.</summary>
    private Rectangle _invalidBounds;

    /// <summary>Whether the current win has already started its one-shot celebration.</summary>
    private bool _winFeedbackShown;

    /// <summary>The elapsed time since the current win celebration began.</summary>
    private TimeSpan _celebrationElapsed;

    /// <summary>Identifies the cached sound associated with a player-visible outcome.</summary>
    private enum FeedbackSound
    {
        /// <summary>A soft tone for drawing, recycling, moving, and undoing cards.</summary>
        Move,
        /// <summary>A rising pair of tones for advancing a foundation.</summary>
        Foundation,
        /// <summary>A quiet low tone for a rejected move.</summary>
        Invalid,
        /// <summary>A short ascending phrase after the final card settles.</summary>
        Win
    }

    /// <summary>Creates original, short PCM sounds and playback instances without external assets.</summary>
    private void LoadFeedbackSounds()
    {
        try
        {
            AddFeedbackSound([330], 0.07);
            AddFeedbackSound([523.25, 659.25], 0.08);
            AddFeedbackSound([196], 0.1);
            AddFeedbackSound([523.25, 659.25, 783.99, 1046.5], 0.12);
        }
        catch (NoAudioHardwareException exception)
        {
            ReportAudioUnavailable(exception);
        }
    }

    /// <summary>Creates one cached mono sound with smooth attack and release envelopes for each note.</summary>
    /// <param name="frequencies">The note frequencies in hertz.</param>
    /// <param name="noteSeconds">The duration of each note in seconds.</param>
    private void AddFeedbackSound(double[] frequencies, double noteSeconds)
    {
        const int sampleRate = 22050;
        var noteSamples = (int)(sampleRate * noteSeconds);
        var samples = new byte[noteSamples * frequencies.Length * 2];
        for (var noteIndex = 0; noteIndex < frequencies.Length; noteIndex++)
        {
            for (var sampleIndex = 0; sampleIndex < noteSamples; sampleIndex++)
            {
                var progress = (double)sampleIndex / (noteSamples - 1);
                var envelope = Math.Sin(Math.PI * progress);
                var wave = Math.Sin(2 * Math.PI * frequencies[noteIndex] * sampleIndex / sampleRate);
                var sample = (short)(wave * envelope * envelope * short.MaxValue * 0.5);
                var byteIndex = (noteIndex * noteSamples + sampleIndex) * 2;
                samples[byteIndex] = (byte)(sample & 255);
                samples[byteIndex + 1] = (byte)((sample >> 8) & 255);
            }
        }

        var sound = new SoundEffect(samples, sampleRate, AudioChannels.Mono);
        _feedbackSounds.Add(sound);
        var voice = sound.CreateInstance();
        _feedbackVoices.Add(voice);
        voice.Volume = 0.18f;
    }

    /// <summary>Plays one quiet cue, restarting its existing voice rather than accumulating overlapping copies.</summary>
    /// <param name="sound">The outcome to announce.</param>
    private void PlayFeedbackSound(FeedbackSound sound)
    {
        if (!_soundEnabled || _audioUnavailable || _feedbackVoices.Count == 0)
        {
            return;
        }

        try
        {
            var voice = _feedbackVoices[(int)sound];
            voice.Stop();
            voice.Play();
        }
        catch (NoAudioHardwareException exception)
        {
            ReportAudioUnavailable(exception);
        }
    }

    /// <summary>Reports audio failure through diagnostics and the visible sound-unavailable state.</summary>
    /// <param name="exception">The audio-device failure reported by MonoGame.</param>
    private void ReportAudioUnavailable(NoAudioHardwareException exception)
    {
        Trace.TraceWarning($"Solitaire sound unavailable: {exception}");
        _audioUnavailable = true;
        DisposeFeedbackSounds();
    }

    /// <summary>Toggles session-local audio without modifying the board or replaying muted cues.</summary>
    private void ToggleSound()
    {
        if (_audioUnavailable)
        {
            return;
        }

        _soundEnabled = !_soundEnabled;
        if (!_soundEnabled)
        {
            foreach (var voice in _feedbackVoices)
            {
                voice.Stop();
            }
        }
    }

    /// <summary>Releases playback instances before their backing sound resources.</summary>
    private void DisposeFeedbackSounds()
    {
        foreach (var voice in _feedbackVoices)
        {
            voice.Dispose();
        }

        _feedbackVoices.Clear();
        foreach (var sound in _feedbackSounds)
        {
            sound.Dispose();
        }

        _feedbackSounds.Clear();
    }

    /// <summary>Clears transient feedback without changing the session's sound preference.</summary>
    private void ResetFeedback()
    {
        _invalidTimeRemaining = TimeSpan.Zero;
        _invalidBounds = Rectangle.Empty;
        _winFeedbackShown = false;
        _celebrationElapsed = TimeSpan.Zero;
        foreach (var voice in _feedbackVoices)
        {
            voice.Stop();
        }
    }

    /// <summary>Shows brief, nonblocking feedback for an actual rejected player move.</summary>
    /// <param name="position">The attempted destination or double-click position.</param>
    private void ShowInvalidMove(Point position)
    {
        ClearHint();
        _invalidBounds = Rectangle.Empty;
        for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
        {
            var bounds = GetFoundationBounds(foundationIndex);
            if (bounds.Contains(position))
            {
                _invalidBounds = bounds;
                break;
            }
        }

        if (_invalidBounds == Rectangle.Empty)
        {
            for (var columnIndex = 0; columnIndex < _tableau.Count; columnIndex++)
            {
                if (GetTableauBounds(columnIndex).Contains(position))
                {
                    _invalidBounds = GetTableauCardBounds(columnIndex, Math.Max(0, _tableau[columnIndex].Count - 1));
                    break;
                }
            }
        }

        _invalidTimeRemaining = InvalidFeedbackDuration;
        PlayFeedbackSound(FeedbackSound.Invalid);
    }

    /// <summary>Advances feedback and starts a single celebration after the winning transition finishes.</summary>
    /// <param name="elapsed">The elapsed update time, independent of frame rate.</param>
    private void UpdateFeedback(TimeSpan elapsed)
    {
        _invalidTimeRemaining = elapsed >= _invalidTimeRemaining ? TimeSpan.Zero : _invalidTimeRemaining - elapsed;
        if (!_hasWon)
        {
            _winFeedbackShown = false;
            _celebrationElapsed = TimeSpan.Zero;
            return;
        }

        if (!_winFeedbackShown && _cardTransitions.Count == 0)
        {
            _winFeedbackShown = true;
            _celebrationElapsed = TimeSpan.Zero;
            PlayFeedbackSound(FeedbackSound.Win);
        }
        else if (_winFeedbackShown)
        {
            _celebrationElapsed = elapsed >= CelebrationDuration - _celebrationElapsed
                ? CelebrationDuration : _celebrationElapsed + elapsed;
        }
    }

    /// <summary>Draws a theme-aware rejected-move outline, kept static when reduced motion is enabled.</summary>
    private void DrawInvalidFeedback()
    {
        if (_invalidTimeRemaining > TimeSpan.Zero && _invalidBounds != Rectangle.Empty)
        {
            var opacity = _reducedMotion ? 1
                : (float)(_invalidTimeRemaining.TotalSeconds / InvalidFeedbackDuration.TotalSeconds);
            var color = _highContrast ? Color.White : new Color(241, 150, 131);
            DrawTargetOutline(_invalidBounds, color * Math.Min(1, opacity * 2));
        }
    }

    /// <summary>Draws a small, finite gold-and-mint burst without obscuring controls or changing card ownership.</summary>
    private void DrawWinCelebration()
    {
        if (_reducedMotion || !_hasWon || !_winFeedbackShown || _celebrationElapsed >= CelebrationDuration)
        {
            return;
        }

        var seconds = (float)_celebrationElapsed.TotalSeconds;
        var opacity = Math.Clamp(3 - seconds, 0, 1);
        for (var particleIndex = 0; particleIndex < 28; particleIndex++)
        {
            var direction = particleIndex % 2 == 0 ? -1 : 1;
            var speed = 42 + particleIndex * 5;
            var x = 550 + direction * (80 + speed * seconds);
            var y = 410 - (145 + particleIndex % 7 * 18) * seconds + 100 * seconds * seconds;
            if (x < 32 || x > 1062 || y < 82 || y > 714)
            {
                continue;
            }

            var color = particleIndex % 3 == 0 ? new Color(112, 220, 182) : new Color(251, 219, 148);
            Fill(new Rectangle((int)x, (int)y, 4, 6), color * opacity);
        }
    }
}
