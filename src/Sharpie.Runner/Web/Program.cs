using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using Raylib_cs;
using Sharpie.Runner.Web.Impl;

namespace Sharpie.Runner.Web;

public partial class Program
{
    private static RaylibVideoOutput video = null!;
    private static RaylibAudioOutput audio = null!;
    private static RaylibInputHandler input = null!;
    private static RaylibDebugOutpug logger = null!;
    private static SharpieConsole emulator = null!;
    private static byte[]? romBytes = null;

    /// <summary>
    /// Application entry point
    /// </summary>
    public static void Main()
    {
        video = new RaylibVideoOutput();
        audio = new RaylibAudioOutput();
        input = new RaylibInputHandler();
        logger = new RaylibDebugOutpug(20);

        var biosBytes = BiosLoader.GetEmbeddedBiosBinary();
        
        emulator = new SharpieConsole(video, audio, input, logger);
        emulator.LoadBios(biosBytes);
    }

    /// <summary>
    /// Updates frame - called by JavaScript requestAnimationFrame
    /// </summary>
    [JSExport]
    public static void UpdateFrame()
    {
        // Handle file drops
        if (Raylib.IsFileDropped() && emulator.IsInBootMode)
        {
            try
            {
                unsafe
                {
                    var droppedFiles = Raylib.LoadDroppedFiles();
                    if (droppedFiles.Count > 0)
                    {
                        var cartridgeFile = Marshal.PtrToStringUTF8((IntPtr)droppedFiles.Paths[0]);
                        if (cartridgeFile != null && cartridgeFile.EndsWith(".shr"))
                        {
                            romBytes = File.ReadAllBytes(cartridgeFile);
                            if (romBytes != null)
                            {
                                emulator.LoadCartridge(romBytes);
                            }
                        }
                        Raylib.UnloadDroppedFiles(droppedFiles);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading ROM: {e.Message}");
            }
        }

        emulator.Step();
        video.HandleFramebuffer(emulator.GetVideoBuffer());
        logger.LogAll();
    }
}
