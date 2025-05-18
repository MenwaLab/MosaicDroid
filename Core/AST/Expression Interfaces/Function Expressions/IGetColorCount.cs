public class GetColorCountExpression : FunctionCallExpression
{
    public GetColorCountExpression(IReadOnlyList<Expression> args, CodeLocation loc)
        : base(TokenValues.GetColorCount, args, loc)
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

    public override string ToString() =>
        $"{TokenValues.GetColorCount}({string.Join(", ", Args)})";
}