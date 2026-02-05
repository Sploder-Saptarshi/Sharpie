using Sharpie.Core.Drivers;

namespace Sharpie.Runner.RaylibCs.Impl;

public class RaylibSaveHandler : ISaveHandler
{
    public string? SavePath { get; set; }

    public void SaveToDisk(ReadOnlySpan<byte> saveRam, bool append = false)
    {
        if (string.IsNullOrWhiteSpace(SavePath))
            throw new ArgumentNullException(nameof(SavePath), "No save path defined.");
        var directoryName =
            Path.GetDirectoryName(SavePath) ?? throw new ArgumentNullException(nameof(SavePath));
        Directory.CreateDirectory(directoryName);

        if (!append)
            File.WriteAllBytes(SavePath, saveRam);
        else
        {
            var existing = File.Open(SavePath, FileMode.Append);
            if (!existing.CanWrite)
            {
                Console.WriteLine($"Missing necessary file permissions to write to {SavePath}");
                return;
            }

            existing.Write(saveRam);
        }
        Console.WriteLine($"Successfully wrote save data to {SavePath}");
    }
}
