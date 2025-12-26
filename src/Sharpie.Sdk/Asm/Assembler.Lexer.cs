using System.Text.RegularExpressions;

namespace Sharpie.Sdk.Asm;

public partial class Assembler
{
    private int StartAddress = 0; // always start at address 0x0000
    private int CurrentAddress;
    private static readonly char[] CommonDelimiters = [','];

    private IEnumerable<string>? FileContents { get; set; }
    private Dictionary<string, int> LabelToMemAddr { get; } = new();
    private Regex Regex { get; } = new("");
    public List<TokenLine> Tokens { get; } = new();

    public void ReadFile()
    {
        if (FileContents == null)
            throw new NullReferenceException("File contents are null. Check your file path.");

        var lineNum = 0;
        string cleanLine;
        TokenLine tokenLine;
        foreach (var line in FileContents!)
        {
            tokenLine = new TokenLine();
            lineNum++;
            cleanLine = line;
            RemoveComment(ref cleanLine);
            if (IsLineEmpty(cleanLine))
                continue;

            RemoveLabel(ref cleanLine, ref tokenLine, lineNum);
            if (IsLineEmpty(cleanLine))
                continue;

            Tokenize(cleanLine, ref tokenLine, lineNum);

            Console.WriteLine(tokenLine.ToString());
            if (tokenLine.ArePropertiesNull())
                continue;
            Tokens.Add(tokenLine);
        }

        return;
        bool IsLineEmpty(string line) => string.IsNullOrWhiteSpace(line);
    }

    /// Mostly used for unit tests
    public void ReadRawAssembly(string assemblyCode)
    {
        FileContents = assemblyCode.Split('\n');
        ReadFile();
    }

    public void LoadFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"No file by name \"{filePath}\" was found.");
        if (!filePath.EndsWith(".asm"))
            throw new Exception($"File \"{filePath}\" is not in the \".asm\" format.");
        FileContents = File.ReadAllLines(filePath);
        ReadFile();
    }

    private static void RemoveComment(ref string line)
    {
        if (!line.Contains(';'))
            return;
        var comment = Regex.Match(line, ";");
        if (comment.Success)
        {
            line = line.Remove(comment.Index);
            line = line.Trim();
        }
    }

    private void RemoveLabel(ref string line, ref TokenLine tokenLine, int lineNumber)
    {
        if (line.Split(':').Length > 2)
            throw new AssemblySyntaxException("Unexpected token: \":\"", lineNumber);

        var labelRegex = Regex.Match(line, ":");
        if (labelRegex.Success)
        {
            var colonIndex = labelRegex.Index;
            var label = line.Substring(0, colonIndex).Trim();
            LabelToMemAddr[label] = CurrentAddress;
            tokenLine.Label = label;
            line = line.Substring(colonIndex + 1).Trim();
        }
    }

    private void Tokenize(string line, ref TokenLine tokenLine, int lineNumber)
    {
        var args = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var opcode = args[0];
        for (var i = 0; i < args.Length; i++)
            args[i] = args[i].TrimEnd(CommonDelimiters).Trim();

        if (!InstructionSet.IsValidOpcode(opcode))
            throw new AssemblySyntaxException($"Invalid Opcode: {opcode}", lineNumber);

        if (args.Length != InstructionSet.GetOpcodeWords(opcode))
            throw new AssemblySyntaxException(
                $"Invalid argument count for opcode {opcode}: expected {InstructionSet.GetOpcodeLength(opcode)} but found {args.Length}"
            );

        tokenLine.Opcode = opcode;
        tokenLine.Args = args.Skip(1).ToArray();
        CurrentAddress += InstructionSet.GetOpcodeLength(opcode)!.Value;
    }
}
