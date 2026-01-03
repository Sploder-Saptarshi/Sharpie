public interface IAudioOutput
{
    void Initialize(int sampleRate);
    void HandleAudioBuffer(float[] audioBuffer);
    void Cleanup();
}
