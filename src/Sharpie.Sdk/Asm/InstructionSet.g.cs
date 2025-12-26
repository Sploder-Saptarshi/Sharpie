// auto-generated
namespace Sharpie.Sdk.Asm;

public static class InstructionSet
{

    private static Dictionary<string, (int Length, int Hex, int RequiredWords)> OpcodeTable = new()
    {
        { "NOP", (1, 0, 0) },
        { "MOV", (2, 1, 2) },
        { "LDM", (3, 16, 2) },
        { "LDI", (3, 32, 2) },
        { "STM", (3, 48, 2) },
        { "ADD", (2, 64, 2) },
        { "SUB", (2, 65, 2) },
        { "MUL", (2, 66, 2) },
        { "DIV", (2, 67, 2) },
        { "MOD", (2, 68, 2) },
        { "AND", (2, 69, 2) },
        { "OR", (2, 70, 2) },
        { "XOR", (2, 71, 2) },
        { "SHL", (2, 72, 2) },
        { "SHR", (2, 73, 2) },
        { "CMP", (2, 74, 2) },
        { "ADC", (2, 75, 2) },
        { "INC", (2, 80, 1) },
        { "DEC", (2, 81, 1) },
        { "NOT", (2, 82, 1) },
        { "NEG", (2, 83, 1) },
        { "IADD", (4, 96, 2) },
        { "ISUB", (4, 97, 2) },
        { "IMUL", (4, 98, 2) },
        { "IDIV", (4, 99, 2) },
        { "IMOD", (4, 100, 2) },
        { "IAND", (4, 101, 2) },
        { "IOR", (4, 102, 2) },
        { "IXOR", (4, 103, 2) },
        { "DINC", (3, 104, 1) },
        { "DDEC", (3, 105, 1) },
        { "DADD", (4, 106, 2) },
        { "DSUB", (4, 107, 2) },
        { "DMOV", (4, 108, 2) },
        { "DSET", (5, 109, 2) },
        { "JMP", (3, 112, 1) },
        { "JEQ", (3, 113, 1) },
        { "JNE", (3, 114, 1) },
        { "JGT", (3, 115, 1) },
        { "JLT", (3, 116, 1) },
        { "CALL", (3, 117, 1) },
        { "RET", (1, 118, 0) },
        { "PUSH", (2, 119, 1) },
        { "POP", (2, 120, 1) },
        { "RND", (4, 128, 2) },
        { "SONG", (1, 160, 1) },
        { "DRAW", (3, 240, 3) },
        { "CLS", (2, 241, 1) },
        { "VBLNK", (1, 242, 0) },
        { "PLAY", (4, 243, 2) },
        { "STOP", (2, 244, 1) },
        { "INPUT", (2, 245, 2) },
        { "TEXT", (4, 247, 3) },
        { "ATTR", (2, 248, 3) },
        { "SWC", (2, 249, 2) },
        { "FLPH", (2, 250, 1) },
        { "FLPV", (2, 251, 1) },
        { "MUTE", (1, 252, 0) },
        { "PREFIX", (1, 254, 0) },
        { "HALT", (1, 255, 0) },
    };

    public static int? GetOpcodeLength(string name)
        => OpcodeTable.ContainsKey(name) ? OpcodeTable[name].Length : null;

    public static int? GetOpcodeHex(string name)
        => OpcodeTable.ContainsKey(name) ? OpcodeTable[name].Hex : null;

    public static int? GetOpcodeWords(string name)
        => OpcodeTable.ContainsKey(name) ? OpcodeTable[name].RequiredWords : null;

    public static bool IsValidOpcode(string name)
        => OpcodeTable.ContainsKey(name);
}
