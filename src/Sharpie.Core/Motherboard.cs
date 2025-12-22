using Raylib_cs;

namespace Sharpie.Core;

public class Motherboard : IMotherboard
{
    private readonly Cpu _cpu;
    private readonly Ppu _ppu;
    private readonly Apu _apu;
    private readonly Memory _memory;

    private Texture2D _screenTexture;
    private RenderTexture2D _target;
    private AudioStream _stream;
    private float[] _writeBuffer = new float[441];

    private byte _fontColorReg = 1;
    private byte _fontSizeReg = 0;

    private const int MusicPointerAddress = 0x0004;
    private int _musicStart = 0;

    public Motherboard()
    {
        _memory = new Memory();
        Span<byte> oam = _memory.Slice(Memory.OamStart, 2048);
        oam.Fill(0xFF);
        _cpu = new Cpu(_memory, this);
        _ppu = new Ppu(_memory);
        _apu = new Apu(_memory);
    }

    public void LoadCartridge(string path)
    {
        var loadAttempt = Cartridge.Load(path);
        if (!loadAttempt.HasValue)
            return; // softlock like how the DS froze if you took the cartridge out

        var cart = loadAttempt.Value;
        _cpu.LoadPalette(cart.HeaderPalette);
        for (var i = 0; i < cart.RomData.Length; i++)
        {
            _memory.WriteByte(i, cart.RomData[i]);
        }

        _musicStart = cart.MusicAddress;
    }

    public void SetupDisplay()
    {
        // const int InternalRes = 256;
        // var screenW = Raylib.GetMonitorWidth(0);
        // var screenH = Raylib.GetMonitorHeight(0);
        //
        // var scale = (screenH / InternalRes) - 1;
        // if (scale < 1)
        //     scale = 1;
        //
        // var windowSize = InternalRes * scale;
        // Raylib.InitWindow(windowSize, windowSize, "Sharpie");
        // Raylib.SetTargetFPS(60);
        // _target = Raylib.LoadRenderTexture(InternalRes, InternalRes);
        //
        // var blank = Raylib.GenImageColor(InternalRes, InternalRes, Color.Blank);
        // _screenTexture = Raylib.LoadTextureFromImage(blank);
        // Raylib.UnloadImage(blank);
        //
        // Raylib.SetTextureFilter(_screenTexture, TextureFilter.Point);
        // Raylib.SetTextureFilter(_target.Texture, TextureFilter.Point);
        // Raylib.InitWindow(512, 512, "Sharpie");
        // Raylib.SetTargetFPS(60);
        const int internalRes = 256;

        // 1. Hard-initialize a temporary window to get monitor info
        Raylib.InitWindow(800, 600, "Sharpie Initializing...");

        var screenH = Raylib.GetMonitorHeight(0);
        // Let's get a reasonable scale (80% of screen height)
        int scale = (int)((screenH * 0.8f) / internalRes);
        if (scale < 1)
            scale = 1;

        var windowSize = internalRes * scale;

        // 2. Resize to our calculated beautiful size
        Raylib.SetWindowSize(windowSize, windowSize);
        Raylib.SetWindowTitle("Sharpie Virtual Console");
        Raylib.SetTargetFPS(60);

        _target = Raylib.LoadRenderTexture(internalRes, internalRes);

        // 3. Create the texture that actually holds the PPU bytes
        var blank = Raylib.GenImageColor(internalRes, internalRes, Color.Blank);
        _screenTexture = Raylib.LoadTextureFromImage(blank);
        Raylib.UnloadImage(blank);

        Raylib.SetTextureFilter(_screenTexture, TextureFilter.Point);
        Raylib.SetTextureFilter(_target.Texture, TextureFilter.Point);
        Raylib.SetWindowPosition(1000, 1000);
        // _screenImage = new Image
        // {
        //     Width = 256,
        //     Height = 256,
        //     Mipmaps = 1,
        //     Format = PixelFormat.UncompressedR8G8B8A8,
        // };
        // _screenTexture = Raylib.LoadTextureFromImage(_screenImage);
        // Raylib.SetTextureFilter(_screenTexture, TextureFilter.Point);
    }

    public void UpdateDisplay()
    {
        Raylib.BeginTextureMode(_target);
        _ppu.FillBuffer(0);
        AwaitVBlank();
        _ppu.FlipBuffers();
        Raylib.EndTextureMode();

        // byte[] displaydata = _ppu.GetFrame();
        // unsafe
        // {
        //     fixed (byte* pDisp = displaydata)
        //     {
        //         Raylib.UpdateTexture(_screenTexture, pDisp);
        //     }
        // }
    }

    public void SetupAudio()
    {
        Raylib.SetAudioStreamBufferSizeDefault(1764);
        Raylib.InitAudioDevice();
        // 44100Hz 32bit float mono
        _stream = Raylib.LoadAudioStream(44100, 32, 1);
        _writeBuffer = new float[1764]; // 10ms of audio at a time
        Raylib.PlayAudioStream(_stream);
        Raylib.SetMasterVolume(1f);
    }

    public unsafe void UpdateAudio()
    {
        if (!Raylib.IsAudioStreamProcessed(_stream))
            return;

        _apu.FillBuffer(_writeBuffer);
        fixed (float* pBuffer = _writeBuffer)
        {
            Raylib.UpdateAudioStream(_stream, pBuffer, 1764);
        }
    }

    public void AwaitVBlank()
    {
        _ppu.VBlank();
    }

    public void ClearScreen(byte colorIndex)
    {
        _ppu.FillBuffer(colorIndex);
    }

    public void DrawChar(ushort x, ushort y, ushort charCode)
    {
        _ppu.BlitCharacter(x, y, _fontColorReg, IMotherboard.GetCharacter(charCode));
    }

    public ushort GetInputState(byte controllerIndex)
    {
        ushort state = 0;

        if (Raylib.IsKeyDown(KeyboardKey.Up))
            state |= 1;
        if (Raylib.IsKeyDown(KeyboardKey.Down))
            state |= 2;
        if (Raylib.IsKeyDown(KeyboardKey.Left))
            state |= 4;
        if (Raylib.IsKeyDown(KeyboardKey.Right))
            state |= 8;
        if (Raylib.IsKeyDown(KeyboardKey.Z))
            state |= 16;
        if (Raylib.IsKeyDown(KeyboardKey.X))
            state |= 32;
        if (Raylib.IsKeyDown(KeyboardKey.LeftShift))
            state |= 64;
        if (Raylib.IsKeyDown(KeyboardKey.Tab))
            state |= 128;

        return state;
    }

    public void PlayNote(byte channel, byte note)
    {
        throw new NotImplementedException();
    }

    public void SetTextAttributes(byte attributes)
    {
        _fontColorReg = (byte)(attributes & 0x0F);
        _fontSizeReg = (byte)((attributes >> 4) & 0x0F);
    }

    public void StopChannel(byte channel)
    {
        throw new NotImplementedException();
    }

    public void StopSystem()
    {
        _cpu.Halt();
    }

    public void SwapColor(byte oldIndex, byte newIndex)
    {
        _memory.WriteByte(Memory.ColorPaletteStart + oldIndex, newIndex);
    }

    private unsafe void RenderBufferAsTexture(byte[] bufferData)
    {
        AwaitVBlank();
        _ppu.FlipBuffers();
        fixed (byte* pPixels = bufferData)
        {
            Raylib.UpdateTexture(_screenTexture, pPixels);
        }
        Raylib.BeginTextureMode(_target);
        Raylib.DrawTexture(_screenTexture, 0, 0, Color.White);
        Raylib.EndTextureMode();
    }

    public void Run()
    {
        SetupDisplay();
        SetupAudio();
        var windowSize = _target.Texture.Width * ((Raylib.GetScreenHeight() / 256));
        _memory.WriteByte(Memory.ColorPaletteStart + 1, 10);
        while (!Raylib.WindowShouldClose())
        {
            for (var i = 0; i < 16000; i++)
                _cpu.Cycle();

            //UpdateDisplay();
            UpdateAudio();
            DrawChar(10, 10, 0);

            Raylib.BeginTextureMode(_target);
            Raylib.ClearBackground(Color.Black);
            var frameData = _ppu.GetFrame();
            RenderBufferAsTexture(frameData);
            Raylib.EndTextureMode();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkGray);
            var sourceRec = new Rectangle(0, 0, _target.Texture.Width, -_target.Texture.Height);
            var destRec = new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            Raylib.DrawTexturePro(
                _target.Texture,
                sourceRec,
                destRec,
                System.Numerics.Vector2.Zero,
                0f,
                Color.White
            );
            Raylib.EndDrawing();
        }

        Cleanup();
    }

    private void Cleanup()
    {
        Raylib.UnloadTexture(_screenTexture);
        Raylib.UnloadRenderTexture(_target);
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }
}
