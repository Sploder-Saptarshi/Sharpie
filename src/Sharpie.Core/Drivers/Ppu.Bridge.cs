namespace Sharpie.Core.Hardware;

internal partial class Ppu
{
    private readonly byte[] _framebuffer = new byte[262144];

    public byte[] GetFrame()
    {
        for (int i = 0; i <= FrameSize - 1; i++)
        {
            var colorIndex = _vRam.ReadByte(i);
            var realIndex = _mobo.ReadByte(Memory.ColorPaletteStart + colorIndex);
            var color = IMotherboard.MasterPalette[realIndex];

            var bufferIndex = i * 4;
            _framebuffer[bufferIndex] = color.R;
            _framebuffer[bufferIndex + 1] = color.G;
            _framebuffer[bufferIndex + 2] = color.B;
            _framebuffer[bufferIndex + 3] = (byte)((realIndex == 0 || realIndex == 16) ? 0 : 255); // Color 16 (aka color 0 in the alternate palette) is also always ignored
        }
        return _framebuffer;
    }
}
