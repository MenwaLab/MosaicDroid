namespace MosaicDroid.Core
{
    public class AssignExpression : StatementNode
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
            if (!ok) return false;

            if (ValueExpr.Type == ExpressionType.ErrorType)
            {
                ErrorHelpers.InvalidAssign(errors, Location, VariableName);
                return false;
            }

            context.SetVariableType(VariableName, ValueExpr.Type);
            return ok;
        }
        public override void Accept(IStmtVisitor visitor)
              => visitor.VisitAssign(this);
        public override string ToString() => $"{VariableName} <- {ValueExpr}";
    }
}