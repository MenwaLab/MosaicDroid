public class ProgramExpression : ASTNode
{
    public List<ASTNode> Statements { get; } = new();
    public List<CompilingError> Errors { get; } = new();

    public Dictionary<string,CodeLocation> LabelIndices { get; } = new Dictionary<string,CodeLocation>();

    public ProgramExpression(CodeLocation loc) : base(loc) {}

    public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
    {       
        foreach (var kv in LabelIndices)
            ctx.DeclareLabel(kv.Key, kv.Value, errors);

        bool ok = true;
        foreach(var stmt in Statements)
            ok &= stmt.CheckSemantic(ctx, scope, errors);
        return ok;
    }
}
