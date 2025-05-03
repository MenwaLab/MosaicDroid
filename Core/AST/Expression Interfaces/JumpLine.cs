// ASTNodes/JumpLineExpression.cs

public class JumpLineExpression : ASTNode
{
    public JumpLineExpression(CodeLocation loc) : base(loc) {}

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        // JumpLine is always semantically valid (it just separates statements).
        return true;
    }

    public override string ToString() => "<JumpLine>";
}
