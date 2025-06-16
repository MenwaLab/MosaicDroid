namespace MosaicDroid.Core
{
    public class SpawnCommand : CallNode
    {
        public SpawnCommand(IReadOnlyList<Expression> args, CodeLocation loc)
          : base(TokenValues.Spawn, args, loc)
        {
        }

        public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
        {
            // haceel chequeo de la cantidad de argumentos de spawn(2) y que ambos sean  Number
            bool ok = base.CheckSemantic(ctx, scope, errors);

            // solo un spawn
            if (ctx.SpawnSeen)
            {
                ErrorHelpers.DuplicateSpawn(errors, Location);
                ok = false;
            }
            ctx.SpawnSeen = true;
            return ok;
        }


        public override string ToString() =>
            $"Spawn({Args[0]}, {Args[1]}) at {Location.Line}:{Location.Column}";

        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitSpawn(this);
        }

    }
}