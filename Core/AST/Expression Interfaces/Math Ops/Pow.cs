// File: Expressions/PowerExpression.cs
public class PowerExpression : BinaryExpression
{
    public override ExpressionType Type { get; set; }
    public override object? Value { get; set; }

    public PowerExpression(CodeLocation location) : base(location) { }

    public override void Evaluate()
    {
        Left.Evaluate();
        Right.Evaluate();
        Value = Math.Pow((double)Left.Value!, (double)Right.Value!);
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool okL = Left.CheckSemantic(context, scope, errors);
        bool okR = Right.CheckSemantic(context, scope, errors);

        if (Left.Type != ExpressionType.Number || Right.Type != ExpressionType.Number)
        {
            ErrorHelpers.InvalidOperands(errors,Location, "exponentiation");
            Type = ExpressionType.ErrorType;
            return false;
        }

        Type = ExpressionType.Number;
        return okL && okR;
    }

    public override string ToString()
    {
        if (Value == null)
            return $"({Left} ** {Right})";
        return Value.ToString()!;
    }
    public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
    {
        return visitor.VisitPow(this);
    }
}
