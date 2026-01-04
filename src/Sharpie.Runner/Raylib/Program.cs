using Sharpie.Core.Hardware;
using Sharpie.Runner.RaylibCs.Impl;

var video = new RaylibVideoOutput();
var audio = new RaylibAudioOutput();
var input = new RaylibInputHandler();

var biosBytes = File.ReadAllBytes("artifacts/runner/raylib/bios.bin");

var emulator = new Motherboard(video, audio, input);
emulator.LoadBios(biosBytes);

while (!video.ShouldCloseWindow())
{
    emulator.Step();
    video.HandleFramebuffer(emulator.GetVideoBuffer());
}

video.Cleanup();
audio.Cleanup();
