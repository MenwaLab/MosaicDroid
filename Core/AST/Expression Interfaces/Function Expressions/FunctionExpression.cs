public abstract class FunctionCallExpression : Expression
{
    protected readonly CallNode _callNode;

    protected FunctionCallExpression(CallNode callNode)
      : base(callNode.Location)
    {
        _callNode = callNode;
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        // Reuse CallNode's semantic checking
        return _callNode.CheckSemantic(context, scope, errors);
    }
}