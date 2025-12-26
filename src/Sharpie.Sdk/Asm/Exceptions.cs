namespace Sharpie.Sdk.Asm;

public class AssemblySyntaxException : System.Exception
{
    public AssemblySyntaxException() { }

    public AssemblySyntaxException(string message)
        : base(message) { }

    public AssemblySyntaxException(string message, int lineNumber)
        : this($"Syntax Error at line {lineNumber}: {message}") { }

    public AssemblySyntaxException(string message, System.Exception inner)
        : base(message, inner) { }
}
