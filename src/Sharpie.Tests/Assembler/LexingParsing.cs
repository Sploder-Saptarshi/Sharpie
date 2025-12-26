namespace Sharpie.Tests.Assembler;

public class LexingParsing
{
    [Fact]
    public void StringAndLabel_ShouldAlignCorrecty()
    {
        var asm = new Sharpie.Sdk.Asm.Assembler();
        string code = ".STR 0 0 \"A\"\nLoop:\nJMP Loop";

        asm.ReadRawAssembly(code);
        asm.Compile();

        Assert.Equal(4, asm.LabelToMemAddr["Loop"]);

        Assert.Equal(0x04, asm.Rom[5]);
        Assert.Equal(0x00, asm.Rom[6]);
    }
}
