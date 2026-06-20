using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ubiquitous.GameInterface.Audio
{
    /// <summary>
    /// Generates and plays procedural fantasy-themed background music
    /// </summary>
    /// <remarks>
    /// This class creates ambient fantasy music using synthesized sine wave tones with
    /// pentatonic scales, harmonies, chord progressions, and ethereal patterns suitable for
    /// fantasy game environments.
    /// </remarks>
    public class FantasyMusicGenerator : IDisposable
    {
        private readonly MediaPlayer _mediaPlayer;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _musicGenerationTask;
        private bool _isPlaying;
        private readonly Random _random;

        // Fantasy music scales
        private readonly int[] _pentatonicScale = { 0, 2, 4, 7, 9 }; // C, D, E, G, A
        private readonly int[] _minorPentatonic = { 0, 3, 5, 7, 10 }; // C, Eb, F, G, Bb
        private readonly int _baseNote = 60; // Middle C (261.63 Hz)
        
        // Chord progressions (intervals from root)
        private readonly int[][] _chordProgressions = new int[] []
        {
            new int[] { 0, 4, 7 },      // Major triad
            new int[] { 0, 3, 7 },      // Minor triad
            new int[] { 0, 4, 7, 11 },  // Major 7th
            new int[] { 0, 3, 7, 10 },  // Minor 7th
            new int[] { 0, 5, 7 }       // Sus4
        };
        
        // Audio generation parameters
        private const int SampleRate = 44100;
        private const int BitsPerSample = 16;
        private const int Channels = 2; // Stereo for richer sound
        
        // Musical pattern state
        private int _currentMelodyIndex = 0;
        private int[] _currentScale;
        private readonly bool _isUpbeat = true; // Fantasy music should be upbeat

        /// <summary>
        /// Initializes a new instance of the <see cref="FantasyMusicGenerator"/> class
        /// </summary>
        public FantasyMusicGenerator()
        {
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.Volume = 0.3; // Slightly higher volume for upbeat music
            _random = new Random();
            _isPlaying = false;
            _currentScale = _pentatonicScale;
        }

        /// <summary>
        /// Starts playing procedurally generated fantasy music
        /// </summary>
        public void Start()
        {
            if (_isPlaying)
                return;

            _isPlaying = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _musicGenerationTask = Task.Run(() => GenerateMusicLoop(_cancellationTokenSource.Token));
        }

        /// <summary>
        /// Stops playing the fantasy music
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
            _cancellationTokenSource?.Cancel();
            _mediaPlayer.Stop();
        }

        /// <summary>
        /// Main loop for generating continuous fantasy music
        /// </summary>
        private async Task GenerateMusicLoop(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && _isPlaying)
                {
                    // For upbeat music, prefer major pentatonic scale
                    if (_random.Next(100) < 10) // 10% chance to switch (less frequent for consistency)
                    {
                        _currentScale = _pentatonicScale; // Prefer major for upbeat feel
                    }
                    
                    // Choose musical pattern - favor rhythmic and melodic for upbeat music
                    var patternType = _random.Next(100);
                    
                    if (patternType < 35) // 35% - Melodic arpeggio
                    {
                        await PlayArpeggioWithHarmony(cancellationToken);
                    }
                    else if (patternType < 60) // 25% - Chord progression
                    {
                        await PlayChordProgression(cancellationToken);
                    }
                    else if (patternType < 75) // 15% - Ethereal drone
                    {
                        await PlayDroneWithMelody(cancellationToken);
                    }
                    else // 25% - Rhythmic pattern (more frequent for upbeat feel)
                    {
                        await PlayRhythmicPattern(cancellationToken);
                    }
                    
                    // Shorter pauses for more energetic feel
                    await Task.Delay(_random.Next(400, 1200), cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when stopping
            }
            catch (Exception)
            {
                // Silently handle any audio errors
            }
        }

        /// <summary>
        /// Plays an arpeggio pattern with accompanying harmony
        /// </summary>
        private async Task PlayArpeggioWithHarmony(CancellationToken cancellationToken)
        {
            var phraseLength = _random.Next(6, 10); // Longer phrases for upbeat feel
            var baseMelodyNote = GetScaleNote(_random.Next(-6, 7));
            
            for (int i = 0; i < phraseLength && !cancellationToken.IsCancellationRequested; i++)
            {
                // Main melody note
                var melodyNote = GetScaleNote(_currentMelodyIndex % _currentScale.Length);
                
                // Harmony note (third or fifth above)
                var harmonyInterval = _random.Next(2) == 0 ? 2 : 3; // Third or fifth in scale
                var harmonyNote = GetScaleNote((_currentMelodyIndex + harmonyInterval) % _currentScale.Length);
                
                // Play melody and harmony together
                await PlayMultipleNotes(
                    new[] { melodyNote, harmonyNote },
                    new[] { 0.7, 0.4 }, // Brighter, louder for upbeat feel
                    _random.Next(400, 700), // Faster tempo
                    cancellationToken);
                
                _currentMelodyIndex++;
                await Task.Delay(_random.Next(150, 400), cancellationToken); // Quicker succession
            }
        }

        /// <summary>
        /// Plays a chord progression
        /// </summary>
        private async Task PlayChordProgression(CancellationToken cancellationToken)
        {
            var numChords = _random.Next(4, 7); // More chords for upbeat progression
            
            for (int i = 0; i < numChords && !cancellationToken.IsCancellationRequested; i++)
            {
                var rootNote = GetScaleNote(_random.Next(_currentScale.Length));
                // Prefer major and major 7th chords for upbeat feel
                var chordIndex = _random.Next(100) < 70 ? 0 : _random.Next(_chordProgressions.Length);
                var chord = _chordProgressions[chordIndex];
                
                var notes = new int[chord.Length];
                var volumes = new double[chord.Length];
                
                for (int j = 0; j < chord.Length; j++)
                {
                    notes[j] = rootNote + chord[j];
                    volumes[j] = 0.5 - (j * 0.05); // Brighter volume
                }
                
                await PlayMultipleNotes(notes, volumes, _random.Next(1000, 1500), cancellationToken); // Faster chord changes
                await Task.Delay(_random.Next(50, 200), cancellationToken); // Shorter gaps
            }
        }

        /// <summary>
        /// Plays an ethereal drone with floating melody notes
        /// </summary>
        private async Task PlayDroneWithMelody(CancellationToken cancellationToken)
        {
            var droneNote = GetScaleNote(0) - 12; // Drone one octave below
            var droneDuration = _random.Next(3000, 5000);
            
            // Start drone note (this will play asynchronously)
            var droneTask = PlaySingleNote(droneNote, 0.2, droneDuration, cancellationToken);
            
            // Play melody notes over the drone
            var melodyNotes = _random.Next(4, 8);
            var noteInterval = droneDuration / (melodyNotes + 1);
            
            for (int i = 0; i < melodyNotes && !cancellationToken.IsCancellationRequested; i++)
            {
                await Task.Delay(noteInterval, cancellationToken);
                
                var melodyNote = GetScaleNote(_random.Next(_currentScale.Length)) + _random.Next(-12, 13);
                await PlaySingleNote(melodyNote, 0.4, _random.Next(600, 1200), cancellationToken);
            }
            
            await droneTask; // Wait for drone to finish
        }

        /// <summary>
        /// Plays a rhythmic pattern with varied timing
        /// </summary>
        private async Task PlayRhythmicPattern(CancellationToken cancellationToken)
        {
            // More energetic pattern for upbeat music
            var pattern = new[] { 1, 1, 0, 1, 1, 0, 1, 0 }; // More notes, energetic rhythm
            var baseNote = GetScaleNote(_random.Next(_currentScale.Length));
            var beatDuration = _random.Next(180, 320); // Faster tempo
            
            foreach (var beat in pattern)
            {
                if (cancellationToken.IsCancellationRequested) break;
                
                if (beat == 1)
                {
                    // Play note with slight variation
                    var note = baseNote + _random.Next(-2, 3) * 2; // Slight pitch variation
                    await PlaySingleNote(note, 0.6, beatDuration, cancellationToken); // Louder notes
                }
                else
                {
                    await Task.Delay(beatDuration, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Plays a single note
        /// </summary>
        private async Task PlaySingleNote(int midiNote, double volume, int durationMs, CancellationToken cancellationToken)
        {
            try
            {
                var frequency = GetFrequencyFromMidiNote(midiNote);
                var wavData = GenerateSineWave(new[] { frequency }, new[] { volume }, durationMs);
                
                var tempFile = Path.Combine(Path.GetTempPath(), $"fantasy_note_{Guid.NewGuid()}.wav");
                await File.WriteAllBytesAsync(tempFile, wavData, cancellationToken);
                
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        _mediaPlayer.Open(new Uri(tempFile));
                        _mediaPlayer.Play();
                    }
                });
                
                await Task.Delay(durationMs, cancellationToken);
                
                try
                {
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                // Silently handle errors to keep music flowing
            }
        }

        /// <summary>
        /// Plays multiple notes simultaneously (harmony/chord)
        /// </summary>
        private async Task PlayMultipleNotes(int[] midiNotes, double[] volumes, int durationMs, CancellationToken cancellationToken)
        {
            try
            {
                var frequencies = new double[midiNotes.Length];
                for (int i = 0; i < midiNotes.Length; i++)
                {
                    frequencies[i] = GetFrequencyFromMidiNote(midiNotes[i]);
                }
                
                var wavData = GenerateSineWave(frequencies, volumes, durationMs);
                
                var tempFile = Path.Combine(Path.GetTempPath(), $"fantasy_chord_{Guid.NewGuid()}.wav");
                await File.WriteAllBytesAsync(tempFile, wavData, cancellationToken);
                
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        _mediaPlayer.Open(new Uri(tempFile));
                        _mediaPlayer.Play();
                    }
                });
                
                await Task.Delay(durationMs, cancellationToken);
                
                try
                {
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                // Silently handle errors to keep music flowing
            }
        }

        /// <summary>
        /// Gets a note from the current scale at various octaves
        /// </summary>
        private int GetScaleNote(int scaleIndex)
        {
            var adjustedIndex = scaleIndex;
            var octaveOffset = 0;
            
            // Handle negative indices
            while (adjustedIndex < 0)
            {
                adjustedIndex += _currentScale.Length;
                octaveOffset -= 12;
            }
            
            // Handle indices beyond scale length
            while (adjustedIndex >= _currentScale.Length)
            {
                adjustedIndex -= _currentScale.Length;
                octaveOffset += 12;
            }
            
            return _baseNote + _currentScale[adjustedIndex] + octaveOffset;
        }

        /// <summary>
        /// Converts a MIDI note number to frequency in Hz
        /// </summary>
        private double GetFrequencyFromMidiNote(int midiNote)
        {
            // A4 (MIDI note 69) = 440 Hz
            // Formula: frequency = 440 * 2^((midiNote - 69) / 12)
            return 440.0 * Math.Pow(2.0, (midiNote - 69) / 12.0);
        }

        /// <summary>
        /// Generates a sine wave WAV file data for multiple frequencies (harmony/chord)
        /// </summary>
        private byte[] GenerateSineWave(double[] frequencies, double[] volumes, int durationMs)
        {
            int numSamples = (int)(SampleRate * durationMs / 1000.0);
            
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                // WAV file header
                writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
                writer.Write(36 + numSamples * Channels * BitsPerSample / 8); // File size - 8
                writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
                
                // Format chunk
                writer.Write(new char[4] { 'f', 'm', 't', ' ' });
                writer.Write(16); // Chunk size
                writer.Write((short)1); // Audio format (1 = PCM)
                writer.Write((short)Channels);
                writer.Write(SampleRate);
                writer.Write(SampleRate * Channels * BitsPerSample / 8); // Byte rate
                writer.Write((short)(Channels * BitsPerSample / 8)); // Block align
                writer.Write((short)BitsPerSample);
                
                // Data chunk
                writer.Write(new char[4] { 'd', 'a', 't', 'a' });
                writer.Write(numSamples * Channels * BitsPerSample / 8);
                
                // Generate sine wave samples with multiple frequencies mixed
                for (int i = 0; i < numSamples; i++)
                {
                    double t = i / (double)SampleRate;
                    
                    // Mix all frequencies together
                    double amplitudeLeft = 0.0;
                    double amplitudeRight = 0.0;
                    
                    for (int f = 0; f < frequencies.Length; f++)
                    {
                        double wave = Math.Sin(2 * Math.PI * frequencies[f] * t) * volumes[f];
                        
                        // Add slight stereo separation for depth
                        double stereoOffset = (f % 2 == 0) ? 0.1 : -0.1;
                        amplitudeLeft += wave * (1.0 + stereoOffset);
                        amplitudeRight += wave * (1.0 - stereoOffset);
                    }
                    
                    // Apply envelope (fade in/out) for smoother, more ethereal sound
                    double envelope = 1.0;
                    double fadeLength = 0.08; // 8% fade
                    
                    if (i < numSamples * fadeLength)
                    {
                        envelope = i / (numSamples * fadeLength);
                    }
                    else if (i > numSamples * (1 - fadeLength))
                    {
                        envelope = (numSamples - i) / (numSamples * fadeLength);
                    }
                    
                    // Add reverb effect by mixing in delayed versions
                    double reverbLeft = 0.0;
                    double reverbRight = 0.0;
                    int reverbDelay = (int)(SampleRate * 0.08); // 80ms delay
                    
                    if (i > reverbDelay)
                    {
                        double reverbT = (i - reverbDelay) / (double)SampleRate;
                        for (int f = 0; f < frequencies.Length; f++)
                        {
                            double reverbWave = Math.Sin(2 * Math.PI * frequencies[f] * reverbT) * volumes[f] * 0.25;
                            reverbLeft += reverbWave;
                            reverbRight += reverbWave;
                        }
                    }
                    
                    // Add subtle chorus effect for magical quality
                    double chorusOffset = Math.Sin(2 * Math.PI * 1.5 * t) * 0.03; // Subtle vibrato
                    
                    amplitudeLeft = (amplitudeLeft + reverbLeft) * envelope * 0.35 * (1.0 + chorusOffset);
                    amplitudeRight = (amplitudeRight + reverbRight) * envelope * 0.35 * (1.0 - chorusOffset);
                    
                    // Clamp to prevent clipping
                    amplitudeLeft = Math.Max(-1.0, Math.Min(1.0, amplitudeLeft));
                    amplitudeRight = Math.Max(-1.0, Math.Min(1.0, amplitudeRight));
                    
                    // Write stereo samples
                    short sampleLeft = (short)(amplitudeLeft * short.MaxValue);
                    short sampleRight = (short)(amplitudeRight * short.MaxValue);
                    writer.Write(sampleLeft);
                    writer.Write(sampleRight);
                }
                
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Disposes resources used by the music generator
        /// </summary>
        public void Dispose()
        {
            Stop();
            _cancellationTokenSource?.Dispose();
            _mediaPlayer.Close();
        }
    }
}
