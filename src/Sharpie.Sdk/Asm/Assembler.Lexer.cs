using System.Text.RegularExpressions;

namespace Sharpie.Sdk.Asm;

public static partial class Assembler
{
    private static ushort StartAddress = 0;
    private static IEnumerable<string>? FileContents { get; set; }
    private static Dictionary<string, int> LabelToMemAddr { get; } = new();
    private static Regex Regex { get; } = new("");

    public static void ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"No file by name \"{filePath}\" was found.");
        if (!filePath.EndsWith(".asm"))
            throw new Exception($"File \"{filePath}\" is not in the \".asm\" format.");
        FileContents = File.ReadAllLines(filePath);
        var lineNum = 0;
        foreach (var line in FileContents)
        {
            lineNum++;
            var cleanLine = line;
            var comment = Regex.Match(cleanLine, ";");
            if (comment.Success)
            {
                cleanLine = cleanLine.Remove(comment.Index);
                cleanLine = cleanLine.TrimEnd();
            }

            var labelNameSeparator = Regex.Match(cleanLine, ":");
            if (labelNameSeparator is { Length: > 1 })
                throw new AssemblySyntaxException("Found more than one instance of ':'");
            if (labelNameSeparator.Success)
            {
                var labelName = cleanLine.Remove(labelNameSeparator.Index);
                LabelToMemAddr[labelName] = 0;
            }
        }
    }
}
