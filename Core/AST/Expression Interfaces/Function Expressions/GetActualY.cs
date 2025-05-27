public class GetActualYExpression : FunctionCallExpression
{
    public GetActualYExpression(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.GetActualY, args, loc)
    {
        Type = ExpressionType.Number;
    }

    public override ExpressionType Type { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate()
    {
        // Stub implementation
        Value = 0;  // Will be replaced with actual evaluation later
    }

    public override string ToString() => $"{Name}()";
    public override string DebugPrint()
    => null;
}