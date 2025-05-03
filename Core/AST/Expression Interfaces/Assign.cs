// ASTNodes/AssignExpression.cs

public class AssignExpression : ASTNode
{
    public string VariableName { get; }
    public Expression ValueExpr { get; }

    public AssignExpression(string variableName, Expression expr, CodeLocation loc)
        : base(loc)
    {
        VariableName = variableName;
        ValueExpr = expr;
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool ok = ValueExpr.CheckSemantic(context, scope, errors);
        if (ValueExpr.Type == ExpressionType.ErrorType)
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, $"Cannot assign invalid expression to '{VariableName}'"));
            return false;
        }
        return ok;
    }

    public override string ToString() => $"{VariableName} <- {ValueExpr}";
}
