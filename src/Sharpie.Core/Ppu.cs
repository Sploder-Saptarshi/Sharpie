namespace Sharpie.Core;

public class Ppu
{
    private const ushort OamStart = Memory.OamStart;
    private const ushort SpriteMemoryStart = 0xBFFF;
    private readonly Memory _systemRam;
    private readonly Memory _vRam;

    public Ppu(Memory systemRam)
    {
        _systemRam = systemRam;
        _vRam = new Memory();
    }

    private byte[] GetSprite(byte index)
    {
        var pixels = new byte[32];
        var pixelIndex = 0;
        for (int i = SpriteMemoryStart - 32 * index; i > SpriteMemoryStart - 32 * (index + 1); i--)
        {
            pixels[pixelIndex] = _systemRam.ReadByte(i);
            pixelIndex++;
        }
        return pixels;
    }
}
