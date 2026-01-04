using Raylib_cs;

namespace Sharpie.Runner.RaylibCs.Impl;

public class RaylibInputHandler : IInputHandler
{
    public byte[] GetInputState()
    {
        byte controller1 = 0;

        if (Raylib.IsKeyDown(KeyboardKey.Up))
            controller1 |= 1;
        if (Raylib.IsKeyDown(KeyboardKey.Down))
            controller1 |= 2;
        if (Raylib.IsKeyDown(KeyboardKey.Left))
            controller1 |= 4;
        if (Raylib.IsKeyDown(KeyboardKey.Right))
            controller1 |= 8;
        if (Raylib.IsKeyDown(KeyboardKey.Z))
            controller1 |= 16;
        if (Raylib.IsKeyDown(KeyboardKey.X))
            controller1 |= 32;
        if (Raylib.IsKeyDown(KeyboardKey.LeftShift))
            controller1 |= 64;
        if (Raylib.IsKeyDown(KeyboardKey.Tab))
            controller1 |= 128;

        byte controller2 = 0;

        return [controller1, controller2];
    }
}
