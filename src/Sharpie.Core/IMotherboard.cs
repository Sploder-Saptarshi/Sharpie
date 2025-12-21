public interface IMotherboard
{
    void Run();
    void ClearScreen(byte colorIndex);
    void AwaitVBlank();
    void SetTextAttributes(byte attributes);
    void DrawChar(ushort x, ushort y, ushort charCode);

    void PlayNote(byte channel, byte note);
    void StopChannel(byte channel);

    ushort GetInputState(byte controllerIndex);

    void SwapColor(byte oldIndex, byte newIndex);
    void StopSystem();

    internal static ReadOnlySpan<(byte R, byte G, byte B)> MasterPalette =>
        new (byte R, byte G, byte B)[]
        {
            (0, 0, 0), // 00: #000000 (Transparent Key)
            (255, 255, 255), // 01: #FFFFFF
            (89, 86, 82), // 02: #595652
            (95, 205, 228), // 03: #5FCDE4
            (91, 110, 225), // 04: #5B6EE1
            (153, 229, 80), // 05: #99E550
            (75, 105, 47), // 06: #4B692F
            (251, 242, 54), // 07: #FBF236
            (223, 113, 38), // 08: #DF7126
            (215, 123, 186), // 09: #D77BBA
            (172, 50, 50), // 10: #AC3232
            (238, 195, 154), // 11: #EEC39A
            (102, 57, 49), // 12: #663931
            (69, 40, 60), // 13: #45283C
            (34, 32, 52), // 14: #222034
            (13, 12, 23), // 15: #0D0C17
            (166, 169, 173), // 16: #A6A9AD
            (221, 223, 203), // 17: #DDDFCB
            (186, 215, 195), // 18: #BAD7C3
            (153, 198, 206), // 19: #99C6CE
            (17, 60, 101), // 20: #113C65
            (83, 205, 205), // 21: #53CDCD
            (40, 132, 69), // 22: #288445
            (32, 142, 217), // 23: #208ED9
            (4, 13, 201), // 24: #040DC9
            (180, 150, 208), // 25: #B496D0
            (102, 26, 175), // 26: #661AAF
            (164, 145, 30), // 27: #A4911E
            (82, 75, 36), // 28: #524B24
            (178, 64, 41), // 29: #B24029
            (77, 26, 12), // 30: #4D1A0C
            (0, 0, 0), // 31: #000000 (Solid Black)
        };
}
