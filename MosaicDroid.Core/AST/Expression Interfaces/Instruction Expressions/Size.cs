namespace MosaicDroid.Core
{
    public class SizeCommand : CallNode
    {
        public SizeCommand(IReadOnlyList<Expression> args, CodeLocation loc)
          : base(TokenValues.Size, args, loc)
        {
        }

        public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
        {
            bool ok = base.CheckSemantic(ctx, scope, errors);


            return ok;

        }

        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitSize(this);
        }

        public override string ToString() =>
            $"Size({Args[0]}) at {Location.Line}:{Location.Column}";
    }
}