public class GetActualXExpression : FunctionCallExpression
{
    public GetActualXExpression(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.GetActualX, args, loc)
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

        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
    {
        return visitor.VisitActualX(this);
    }

    public override string ToString() => $"{Name}()";
}
