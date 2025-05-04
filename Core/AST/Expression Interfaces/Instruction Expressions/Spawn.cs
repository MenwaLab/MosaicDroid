public class SpawnCommand : CallNode
    {
        public SpawnCommand(Expression x, Expression y, CodeLocation loc)
        : base(TokenValues.Spawn, new[] { x, y }, loc)
    {
    }


public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        // 1) Base does argument‐count and type checking (must be two integers)
        bool ok = base.CheckSemantic(context, scope, errors);

        // 2) Enforce exactly one Spawn in the whole program
        if (context.SpawnSeen)
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid,
                "Only one Spawn(x,y) allowed."));
            ok = false;
        }
        context.SpawnSeen = true;

        return ok;
    }



       public override string ToString() => $"Spawn({Args[0]}, {Args[1]}) at {Location.Line}:{Location.Column}";
}
