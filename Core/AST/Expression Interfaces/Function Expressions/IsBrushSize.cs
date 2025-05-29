public class IsBrushSizeExpression : FunctionCallExpression
{
    public IsBrushSizeExpression(IReadOnlyList<Expression> args, CodeLocation loc)
        : base(TokenValues.IsBrushSize, args, loc)
    {
        Type = ExpressionType.Boolean;
    }

    public override ExpressionType Type { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate() => Value = 0; // Stub

    public override string ToString() => $"{TokenValues.IsBrushSize}({Args[0]})";
public int Size => (int)((Number)Args[0]).Value!;
    public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitBrushSize(this);
        }  
}