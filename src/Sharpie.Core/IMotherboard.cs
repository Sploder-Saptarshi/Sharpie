public interface IMotherboard
{
    void ClearScreen(byte colorIndex);
    void AwaitUtilBlank();
    void SetTextAttributes(byte attributes);
    void DrawChar(ushort x, ushort y, ushort charCode);

    void PlayNote(byte channel, byte note);
    void StopChannel(byte channel);

    ushort GetInputState(byte controllerIndex);

    void SwapColor(byte oldIndex, byte newIndex);
}
