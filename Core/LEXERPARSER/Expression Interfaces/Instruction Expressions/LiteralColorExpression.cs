public class LiteralColorExpression : IExpression
{
    public Lexer.AllColors Value { get; }

    public LiteralColorExpression(string color)
    {
        if (!Enum.TryParse<Lexer.AllColors>(color, out var parsedColor))
            throw new Exception($"Invalid color: {color}");

        Value = parsedColor;
    }

    public int Interpret(Context context)
    {
        // In a real implementation, you'd apply this to the brush color
        Console.WriteLine($"[Interpreter] Setting brush color to: {Value}");
        return 0;
    }

    public override string ToString() => $"ColorLiteral: {Value}";
}
