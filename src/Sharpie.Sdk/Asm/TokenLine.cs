public struct TokenLine
{
    public string? Label { get; set; }
    public string? Opcode { get; set; }
    public string[]? Args { get; set; }

    public TokenLine()
    {
        (Label, Opcode, Args) = (null, null, null);
    }

    /// Returns whether all properties are null, effectively determining if a non-empty line was processed.
    public bool ArePropertiesNull() => Label == null && Opcode == null && Args == null;

    public override string ToString()
    {
        var str = $"Token line: Label = {Label} | Opcode = {Opcode} |";

        if (Args == null)
            str += "Args = null";
        else
            for (int i = 0; i < Args.Length; i++)
                str += $"\nArgs[{i}] = {Args[i]}";

        return str;
    }

    public IEnumerable<string> AllTokens()
    {
        List<string> en = [Label ?? "", Opcode ?? ""];

        foreach (string arg in Args ?? [""])
        {
            en.Add(arg);
        }
        return en;
    }
}
