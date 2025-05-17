public class SpawnCommand : CallNode
{
    // now takes *all* args
    public SpawnCommand(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.Spawn, args, loc)
    {
    }
        
    public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
    {
        // 1) base does arity (2?) and type‐check (both Number)
        bool ok = base.CheckSemantic(ctx, scope, errors);

        // 2) exactly one Spawn
        if (ctx.SpawnSeen)
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid,
                "Only one Spawn(x,y) allowed."));
            ok = false;
        }
        ctx.SpawnSeen = true;
        return ok;
    }

    public override string ToString() =>
        $"Spawn({Args[0]}, {Args[1]}) at {Location.Line}:{Location.Column}";
}
