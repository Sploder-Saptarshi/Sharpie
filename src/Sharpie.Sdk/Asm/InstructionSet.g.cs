// auto-generated
namespace Sharpie.Sdk.Asm;

public static class InstructionSet
{

    private static Dictionary<string, (int Length, int Hex)> OpcodeTable = new()
    {
        { "NOP", (1, 0) },
        { "MOV", (2, 1) },
        { "LDM", (3, 16) },
        { "LDI", (3, 32) },
        { "STM", (3, 48) },
        { "ADD", (2, 64) },
        { "SUB", (2, 65) },
        { "MUL", (2, 66) },
        { "DIV", (2, 67) },
        { "MOD", (2, 68) },
        { "AND", (2, 69) },
        { "OR", (2, 70) },
        { "XOR", (2, 71) },
        { "SHL", (2, 72) },
        { "SHR", (2, 73) },
        { "CMP", (2, 74) },
        { "ADC", (2, 75) },
        { "INC", (2, 80) },
        { "DEC", (2, 81) },
        { "NOT", (2, 82) },
        { "NEG", (2, 83) },
        { "IADD", (4, 96) },
        { "ISUB", (4, 97) },
        { "IMUL", (4, 98) },
        { "IDIV", (4, 99) },
        { "IMOD", (4, 100) },
        { "IAND", (4, 101) },
        { "IOR", (4, 102) },
        { "IXOR", (4, 103) },
        { "DINC", (3, 104) },
        { "DDEC", (3, 105) },
        { "DADD", (4, 106) },
        { "DSUB", (4, 107) },
        { "DMOV", (4, 108) },
        { "DSET", (5, 109) },
        { "JMP", (3, 112) },
        { "JEQ", (3, 113) },
        { "JNE", (3, 114) },
        { "JGT", (3, 115) },
        { "JLT", (3, 116) },
        { "CALL", (3, 117) },
        { "RET", (1, 118) },
        { "PUSH", (2, 119) },
        { "POP", (2, 120) },
        { "RND", (3, 128) },
        { "SONG", (1, 160) },
        { "DRAW", (3, 240) },
        { "CLS", (2, 241) },
        { "VBLNK", (1, 242) },
        { "PLAY", (4, 243) },
        { "STOP", (2, 244) },
        { "INPUT", (2, 245) },
        { "TEXT", (4, 247) },
        { "ATTR", (2, 248) },
        { "SWC", (2, 249) },
        { "FLPH", (2, 250) },
        { "FLPV", (2, 251) },
        { "MUTE", (1, 252) },
        { "PREFIX", (1, 254) },
        { "HALT", (1, 255) },
    };

    public static int GetOpcodeLength(string name)
        => OpcodeTable[name].Length;
    public static int GetOpcodeHex(string name)
        => OpcodeTable[name].Hex;
}
