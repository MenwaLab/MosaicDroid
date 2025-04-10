// StringExpression.cs

public class StringExpression : IExpression, ILiteralExpression
{
    public string Value { get; }

    public StringExpression(string value)
    {
        // Remove surrounding quotes, if they exist.
        if (value.StartsWith("\"") && value.EndsWith("\""))
            Value = value.Substring(1, value.Length - 2);
        else
            Value = value;
    }

    public int Interpret(Context context)
    {
        // For literal expressions, interpretation might not do much.
        return 0;
    }

    public override string ToString() => $"StringLiteral: {Value}";
}
