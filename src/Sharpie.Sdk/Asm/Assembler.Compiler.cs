namespace Sharpie.Sdk.Asm;

public partial class Assembler
{
    public readonly byte[] Rom = new byte[Meta.Constants.MaxRomSize];

    public void Compile()
    {
        Console.WriteLine("Assembler: Compiling file...");
        CurrentAddress = 0;
        int lineNum = 0;
        foreach (var token in Tokens)
        {
            lineNum = token.SourceLine!.Value;

            if (token.Args == null)
                throw new AssemblySyntaxException(
                    $"Expected arguments for opcode {token.Opcode}",
                    lineNum
                );

            if (token.Opcode!.StartsWith('.'))
            {
                switch (token.Opcode.ToUpper())
                {
                    case ".ORG":
                        CurrentAddress = ParseWord(token.Args[0], lineNum);
                        break;

                    case ".SPRITE":
                        var spriteIndex = ParseByte(token.Args[0], lineNum);
                        var target = CalculateSpriteAddress(spriteIndex);

                        if (target >= Rom.Length)
                        {
                            throw new SharpieRomSizeException(
                                $"Sprite #{spriteIndex} is out of bounds (Addr: {target})"
                            );
                        }

                        CurrentAddress = target;
                        break;

                    case ".DB":
                    case ".BYTES":
                    case ".DATA":
                        foreach (var arg in token.Args)
                        {
                            WriteToRom(ParseByte(arg, lineNum));
                            CurrentAddress++;
                        }
                        break;

                    case ".DW":
                    case ".WORDS":
                        foreach (var arg in token.Args)
                        {
                            WriteToRom(ParseWord(arg, lineNum));
                            CurrentAddress += 2;
                        }
                        break;
                }
                continue;
            }

            var pattern = InstructionSet.GetOpcodePattern(token.Opcode);
            var opHex = InstructionSet.GetOpcodeHex(token.Opcode);
            var length = InstructionSet.GetOpcodeLength(token.Opcode);
            var isFam = InstructionSet.IsOpcodeFamily(token.Opcode);
            var argIndex = 0;

            if (isFam)
            {
                var arg = token.Args[argIndex];
                argIndex++;
                int regIdx = ParseRegister(arg, lineNum);
                WriteToRom((byte)(opHex | regIdx));
            }
            else
            {
                WriteToRom((byte)opHex);
            }
            CurrentAddress++;

            for (int p = isFam ? 1 : 0; p < pattern.Length; p++)
            {
                var cmd = pattern[p];

                switch (cmd)
                {
                    case 'R':
                        if (p + 1 < pattern.Length && pattern[p + 1] == 'R')
                        {
                            byte rA = ParseRegister(token.Args[argIndex++], lineNum);
                            byte rB = ParseRegister(token.Args[argIndex++], lineNum);
                            WriteToRom((byte)(rA << 4 | rB));
                            p++;
                        }
                        else
                        {
                            WriteToRom(ParseRegister(token.Args[argIndex++], lineNum));
                        }
                        CurrentAddress++;
                        break;

                    case 'W':
                        WriteToRom(ParseWord(token.Args[argIndex++], lineNum));
                        CurrentAddress += 2;
                        break;
                    case 'B':
                        WriteToRom(ParseByte(token.Args[argIndex++], lineNum));
                        CurrentAddress++;
                        break;
                    default:
                        throw new AssemblySyntaxException(
                            $"The SDK definition of the {token.Opcode} instruction contains a bug.\n"
                                + "Please contact the developer at https://github.com/ChristosMaragkos"
                        );
                }
            }
        }
    }

    private void WriteToRom(byte value, int offset = 0)
    {
        var realAddr = CurrentAddress + offset;
        if (realAddr >= Meta.Constants.MaxRomSize) // >= because MaxRomSize will throw as an index
            throw new SharpieRomSizeException(CurrentAddress);

        Rom[realAddr] = value;
    }

    private void WriteToRom(string? opcode) =>
        WriteToRom((byte)InstructionSet.GetOpcodeHex(opcode)!);

    private void WriteToRom(ushort value)
    {
        var low = (byte)(value & 0x00FF);
        var high = (byte)((value & 0xFF00) >> 8); // low endian

        WriteToRom(low);
        WriteToRom(high, 1);
    }
}
