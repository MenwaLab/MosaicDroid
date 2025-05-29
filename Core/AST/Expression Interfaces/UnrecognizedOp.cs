public class NoOpExpression : Expression
{

    public NoOpExpression(CodeLocation location) : base(location) { }

    public override ExpressionType Type { get; set; } = ExpressionType.ErrorType;
    public override object? Value { get; set; }

    public override void Evaluate() { /* no runtime effect */ }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        // Always valid, but carries no value
        return true;
    }
public override TResult Accept<TResult>(IExprVisitor<TResult> v)
  => v.VisitNoOp(this);

    public override string ToString() => "<no-op>";
}