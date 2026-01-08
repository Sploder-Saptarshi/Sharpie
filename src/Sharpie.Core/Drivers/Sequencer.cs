using Sharpie.Core.Hardware;

namespace Sharpie.Core.Drivers;

internal class Sequencer
{
    private readonly IMotherboard _mobo;
    private int _cursor = 0;
    private int _delayFrames = 0;
    public bool Enabled { get; set; } = false;

    public Sequencer(IMotherboard mobo)
    {
        _mobo = mobo;
        if (Instance == null)
            Instance = this;
    }

    public void Reset()
    {
        Enabled = false;
        _cursor = 0;
        _delayFrames = 0;
    }

    public static Sequencer? Instance { get; private set; }

    public void LoadSong(int startAddr)
    {
        _cursor = startAddr;
        _delayFrames = 0;
        Enabled = true;
    }

    public void Step()
    {
        if (!Enabled)
            return;
        if (_delayFrames > 0)
        {
            _delayFrames--;
            return;
        }

        while (Enabled && _delayFrames == 0)
        {
            var channel = _mobo.ReadByte(_cursor);
            var note = _mobo.ReadByte(_cursor + 1);
            var duration = _mobo.ReadByte(_cursor + 2);
            var instrument = _mobo.ReadByte(_cursor + 3);

            if (channel == 0xFF) // END
            {
                Enabled = false;
                _mobo.StopAllSounds();
                break;
            }
            else if (channel == 0xFE) // GOTO
            {
                var stepsBack = (ushort)(duration | (instrument << 8));
                _cursor -= 4 * stepsBack; // move the cursor 4 bytes back per step (since each packet is four bytes)
                continue;
            }
            else if (note == 0)
            {
                _mobo.StopChannel(channel);
            }
            else
            {
                _mobo.PlayNote(channel, note, instrument);
            }

            _delayFrames = duration;
            _cursor += 4;
        }
    }
}
