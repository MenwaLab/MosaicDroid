namespace MosaicDroid.Core
{
    public class FillCommand : CallNode
    {
        public FillCommand(IReadOnlyList<Expression> args, CodeLocation loc) : base(TokenValues.Fill, args, loc)
        {
        }



        public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
        {
            return base.CheckSemantic(ctx, scope, errors);
        }

        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitFill(this);
        }
        public override string ToString() => $"Fill() at {Location.Line}:{Location.Column}";
    }
}