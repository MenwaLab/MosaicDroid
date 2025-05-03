public class ProgramExpression : ASTNode
{
    public List<ASTNode> Statements { get; } = new();
    public List<CompilingError> Errors { get; } = new();

    public ProgramExpression(CodeLocation loc) : base(loc) {}

    public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
    {
        // 1. Scan once to collect all labels:
        foreach(var stmt in Statements)
            if(stmt is LabelExpression lbl)
                ctx.DeclareLabel(lbl.Name, lbl.Location, errors);
        // 2. Then check semantics of each statement in order:
        bool ok = true;
        foreach(var stmt in Statements)
            ok &= stmt.CheckSemantic(ctx, scope, errors);
        return ok;
    }
}
