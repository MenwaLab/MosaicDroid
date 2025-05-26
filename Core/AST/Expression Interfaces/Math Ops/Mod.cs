// File: Expressions/ModulusExpression.cs
public class ModulusExpression : BinaryExpression
{
    public override ExpressionType Type { get; set; }
    public override object? Value { get; set; }

    public ModulusExpression(CodeLocation location) : base(location) { }

    public override void Evaluate()
    {
        Left.Evaluate();
        Right.Evaluate();
        // Note: cast to int for modulus semantics
        Value = (double)((int)(double)Left.Value! % (int)(double)Right.Value!);
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool okL = Left.CheckSemantic(context, scope, errors);
        bool okR = Right.CheckSemantic(context, scope, errors);

        if (Left.Type != ExpressionType.Number || Right.Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid,
                "Operands must be numeric for modulus."));
            Type = ExpressionType.ErrorType;
            return false;
        }

        Type = ExpressionType.Number;
        return okL && okR;
    }

    public override string ToString()
    {
        if (Value == null)
            return $"({Left} % {Right})";
        return Value.ToString()!;
    }
}
