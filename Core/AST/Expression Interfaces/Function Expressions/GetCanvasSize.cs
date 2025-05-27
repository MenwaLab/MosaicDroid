public class GetCanvasSize : FunctionCallExpression
{
    public override string DebugPrint()
    => null;
    public GetCanvasSize(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.GetCanvasSize, args, loc)
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
}