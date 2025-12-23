namespace Sharpie.Core;

public class Apu
{
    private readonly Memory _ram;
    private readonly float[] _phases = new float[8]; // current phase for every oscillator
    private readonly float[] _volumes = new float[8];
    private readonly float[] _lastNoiseValues = new float[8];
    private readonly float[] _prevPhases = new float[8];
    private readonly Random _noiseGen = new();
    private const int VolumeDecay = 4;

    public Apu(Memory ram)
    {
        _ram = ram;
    }

    private float GenerateSample(int channel)
    {
        var baseAddr = (ushort)(Memory.AudioRamStart + (channel * 4));
        var volume = _volumes[channel];

        if (volume <= 0.001f)
            return 0f;

        var freq = _ram.ReadWord(baseAddr);
        if (freq == 0)
            return 0f;

        _phases[channel] += freq / 44100f;
        if (_phases[channel] >= 1f)
            _phases[channel] -= 1f;
        var phase = _phases[channel];
        var oldPhase = _prevPhases[channel];

        // waveform selection
        var waveform = channel switch
        {
            0 or 1 => Square(phase),
            2 or 3 => Triangle(phase),
            4 or 5 => Sawtooth(phase),
            _ => Noise(channel, phase, oldPhase),
        };

        _lastNoiseValues[channel] = waveform;
        _prevPhases[channel] = phase;
        return waveform * volume;
    }

    private float Noise(int channel, float phase, float oldPhase)
    {
        return (phase < oldPhase ? _noiseGen.NextSingle() * 2 - 1 : _lastNoiseValues[channel]);
    }

    private static float Sawtooth(float phase)
    {
        return (phase * 2f - 1f) * 0.6f;
    }

    private static float Triangle(float phase)
    {
        var value = 0f;
        if (phase < 0.25f)
            value = phase * 4f;
        else if (phase < 0.75f)
            value = 2f - (phase * 4f);
        else
            value = (phase * 4f) - 4f;
        return value * 1.1f;
    }

    private static float Square(float phase)
    {
        return (phase < 0.5f ? 1f : -1f) * 0.4f;
    }

    internal void FillBuffer(float[] writeBuffer)
    {
        for (var i = 0; i < writeBuffer.Length; i++)
        {
            var mixedSample = 0f;
            var activeChannels = 0;
            for (var chan = 0; chan < 8; chan++)
            {
                var sample = GenerateSample(chan);
                if (MathF.Abs(sample) > 0)
                    activeChannels++;
                mixedSample += sample;
            }
            UpdateVolumes();
            var divisor = MathF.Max(1.5f, activeChannels * 0.5f);
            writeBuffer[i] = (mixedSample / divisor) * 0.5f; // normalized with AGC to avoid earrape
        }
    }

    internal void ResetPhase(int channel)
    {
        if (channel >= 0 && channel < _phases.Length)
            _phases[channel] = 0f;
    }

    internal void ClearPhases()
    {
        Array.Clear(_phases);
    }

    private void UpdateVolumes()
    {
        for (var chan = 0; chan < 8; chan++)
        {
            var baseAddr = Memory.AudioRamStart + (chan * 4);
            var targetVolByte = _ram.ReadByte(baseAddr + 2);
            var control = _ram.ReadByte(baseAddr + 3);

            float targetVol = ((control & 0x01) != 0) ? (targetVolByte / 255f) : 0f;

            // Smooth the volume change (linear interpolation)
            // Adjust these values to change how "snappy" the notes are
            float attackSpeed = 0.1f; // Higher = faster fade in
            float releaseSpeed = 0.05f; // Higher = faster fade out

            if (_volumes[chan] < targetVol)
                _volumes[chan] = Math.Min(targetVol, _volumes[chan] + attackSpeed);
            else if (_volumes[chan] > targetVol)
                _volumes[chan] = Math.Max(targetVol, _volumes[chan] - releaseSpeed);
        }
    }
}
