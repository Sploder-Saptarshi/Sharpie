namespace Sharpie.Core;

public class Sequencer
{
    private readonly Memory _memory;
    private int _musicStart = 0;
    private int _cursor = 0;
    private int _delayFrames = 0;
    public bool Enabled { get; set; } = false;

    public Sequencer(Memory memory)
    {
        _memory = memory;
    }

    public void LoadSong(int startAddr)
    {
        _musicStart = startAddr;
        _cursor = 0;
        _delayFrames = 0;
        Enabled = true;
    }

    public void Step(IMotherboard mobo)
    {
        if (!Enabled || _musicStart == 0)
            return;
        if (_delayFrames > 0)
        {
            _delayFrames--;
            return;
        }

        while (_delayFrames == 0)
        {
            var currentAddr = (ushort)(_musicStart + _cursor);

            var duration = _memory.ReadByte(currentAddr);

            // 0xFF = LOOP TO START
            if (duration == 0xFF)
            {
                _cursor = 0;
                // break to avoid infinite loops if next byte is also 0xFF
                break;
            }

            // 0xFE = STOP SEQUENCER (End of Song)
            if (duration == 0xFE)
            {
                Enabled = false;
                return;
            }

            var note = _memory.ReadByte(currentAddr + 1);
            var channel = _memory.ReadByte(currentAddr + 2);

            if (note == 0)
                mobo.StopChannel(channel);
            else
                mobo.PlayNote(channel, note);

            _delayFrames = duration;
            _cursor += 3;
        }
    }
}
