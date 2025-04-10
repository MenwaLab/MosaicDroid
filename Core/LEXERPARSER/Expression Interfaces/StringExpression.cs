public class StringExpression : IExpression
{
    // This property holds the literal (unquoted) string value.
    public string Value { get; }

    public StringExpression(string value)
    {
        // Optionally remove the surrounding quotes if they are still present.
        if (value.StartsWith("\"") && value.EndsWith("\"") && value.Length >= 2)
            Value = value.Substring(1, value.Length - 2);
        else
            Value = value;
    }

    // Since literal strings are generally not meant for arithmetic evaluation,
    // we return 0 here. Alternatively, you could throw an exception if 
    // Interpret is called on a literal string.
    public int Interpret(Context context)
    {
        return 0;
    }
    
    public override string ToString() => $"LiteralString: \"{Value}\"";
}
