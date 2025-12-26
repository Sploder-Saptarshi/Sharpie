namespace Sharpie.Tests.Assembler;

public class Lexing
{
    private readonly Sharpie.Sdk.Asm.Assembler TestAssembler = new();

    [Theory]
    [InlineData("LabelOnly:")]
    [InlineData("WithArgs: LDI 10, 10")]
    [InlineData("LDI 10, 10")]
    public void Lexer_HandlesLineCorrectly(string content)
    {
        TestAssembler.ReadRawAssembly(content);
    }
}
