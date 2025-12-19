namespace Sharpie.Core;

public class Ppu
{
    private const ushort OamStart = Memory.OamStart;
    private const ushort SpriteMemoryStart = 0xBFFF;
    private int _currentBuffer = 0;
    private ushort DisplayStart => (ushort)(_currentBuffer == 0 ? 0x0000 : 0x8000);
    private ushort RenderStart => (ushort)(_currentBuffer == 0 ? 0x8000 : 0x0000);
    private readonly Memory _systemRam;
    private readonly Memory _vRam;
    private readonly byte[] _spriteBuffer = new byte[64]; // here so the GC doesn't cry

    public Ppu(Memory systemRam)
    {
        _systemRam = systemRam;
        _vRam = new Memory();
    }

    private void GetSprite(byte index, byte attributes)
    {
        var flipH = (attributes & 0x01) != 0;
        var flipV = (attributes & 0x02) != 0;

        var spriteStartAddr = SpriteMemoryStart - (32 * (index + 1));
        for (int row = 0; row < 8; row++)
        {
            var realRow = flipV ? (7 - row) : row;
            for (int column = 0; column < 4; column++) // 4 bytes per row because of indexed color
            {
                var packed = _systemRam.ReadByte(spriteStartAddr + (row * 4) + column);
                var pixel1 = (byte)((packed >> 4) & 0x0F);
                var pixel2 = (byte)(packed & 0x0F);

                var realColumn1 = flipH ? (7 - column * 2) : (column * 2);
                var realColumn2 = flipH ? (7 - column * 2 + 1) : (column * 2 + 1); // pemdas amirite

                _spriteBuffer[realRow * 8 + realColumn1] = pixel1;
                _spriteBuffer[realRow * 8 + realColumn2] = pixel2;
            }
        }
    }

    public void FlipBuffers() => _currentBuffer = 1 - _currentBuffer;

    public void WriteBackBuffer(int offset, byte value) =>
        _vRam.WriteByte(RenderStart + offset, value);

    public void VBlank()
    {
        for (int oamIndex = OamStart; oamIndex < OamStart + 512; oamIndex += 4)
        {
            var x = _systemRam.ReadByte(oamIndex);
            var y = _systemRam.ReadByte(oamIndex + 1); // TODO: Write to software framebuffer/raylib util
            var spriteId = _systemRam.ReadByte(oamIndex + 2);
            var attributes = _systemRam.ReadByte(oamIndex + 3);

            GetSprite(spriteId, attributes);
            for (int row = 0; row < 8; row++)
            {
                var displayY = y + row;
                if (displayY >= 256)
                    break;

                var rowBaseVramSlot = (displayY * 128) + (x / 2);
                for (int column = 0; column < 8; column += 2)
                {
                    var pixelIndex = (row * 8) + column;
                    var pixel1 = _spriteBuffer[pixelIndex];
                    var pixel2 = _spriteBuffer[pixelIndex + 1];
                    var packed = (byte)((pixel1 << 4) | pixel2);
                    WriteBackBuffer(rowBaseVramSlot + (column / 2), packed);
                }
            }
        }
    }
}
