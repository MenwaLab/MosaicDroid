public class IsCanvasColorExpression : FunctionCallExpression
{
    public IsCanvasColorExpression(IReadOnlyList<Expression> args, CodeLocation loc)
        : base(TokenValues.IsCanvasColor, args, loc)
    {
        Type = ExpressionType.Number;
    }

    public override ExpressionType Type { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate() => Value = 0; // Stub

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool ok = base.CheckSemantic(context, scope, errors);
        if (!ok) return false;

        return ColorValidationHelper.ValidateColorArgument(Args, 0, Args[0].Location, errors);
    }

    public string Color     => ((ColorLiteralExpression)Args[0]).Value!.ToString()!;
    public Expression OffsetX => Args[1];
    public Expression OffsetY => Args[2];
            public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
    {
        return visitor.VisitCanvasColor(this);
    }
    public override string ToString() =>
        $"{TokenValues.IsCanvasColor}({string.Join(", ", Args)})";
}