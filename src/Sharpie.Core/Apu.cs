namespace Sharpie.Core;

public class Apu
{
    private readonly Memory _ram;
    private float[] _phases = new float[8]; // current phase for every oscillator
    private readonly Random _noiseGen = new();
    private const int VolumeDecay = 4;

    public Apu(Memory ram)
    {
        _ram = ram;
    }

    private float GenerateSample(int channel)
    {
        var baseAddr = (ushort)(Memory.AudioRamStart + (channel * 4));
        var control = _ram.ReadByte(baseAddr + 3);
        var volumeByte = _ram.ReadByte(baseAddr + 2);

        if ((control & 0x01) == 0 || volumeByte == 0)
            return 0f;

        var freq = _ram.ReadWord(baseAddr);
        if (freq == 0)
            return 0f;

        var volume = volumeByte / 255f;

        _phases[channel] += freq / 44100f;
        // if (_phases[channel] > 1.0f)
        //     _phases[channel] -= 1.0f;
        _phases[channel] %= 1f;

        // waveform selection
        var waveform = channel switch
        {
            0 or 1 => (_phases[channel] < 0.5f ? 1.0f : -1.0f), // square
            2 or 3 => (MathF.Abs(_phases[channel] * 2 - 1) * 2 - 1) * 2f, // triangle
            4 or 5 => (_phases[channel] * 2 - 1) * 1.5f, // sawtooth
            _ => (float)(_noiseGen.NextDouble() * 2 - 1), // noise
        };

        return waveform * volume;
    }

    internal void FillBuffer(float[] writeBuffer)
    {
        UpdateVolumes();
        for (var i = 0; i < writeBuffer.Length; i++)
        {
            var mixedSample = 0f;
            for (var chan = 0; chan < 8; chan++)
                mixedSample += GenerateSample(chan);
            writeBuffer[i] = (mixedSample / 8f) * 0.25f; // normalized to avoid earrape
        }
    }

    private void UpdateVolumes()
    {
        for (var chan = 0; chan < 8; chan++)
        {
            var baseAddr = Memory.AudioRamStart + (chan * 4);
            var volume = _ram.ReadByte(baseAddr + 2);
            var control = _ram.ReadByte(baseAddr + 3);

            if ((control & 0x01) != 0 && volume > 0)
            {
                var newVol = Math.Max(0, volume - VolumeDecay);
                _ram.WriteByte(baseAddr + 2, (byte)newVol);

                if (newVol == 0)
                    _ram.WriteByte(baseAddr + 3, (byte)(control & ~0x01));
            }
        }
    }
}
