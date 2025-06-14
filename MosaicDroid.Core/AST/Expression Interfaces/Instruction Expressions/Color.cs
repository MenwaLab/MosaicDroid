namespace MosaicDroid.Core
{
    public class ColorCommand : CallNode
    {
        public ColorCommand(IReadOnlyList<Expression> args, CodeLocation loc)
          : base(TokenValues.Color, args, loc)
        {
        }

        public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
        {
            // 1) Arity
            bool ok = base.CheckSemantic(ctx, scope, errors);
            if (!ok) return false;

            return ColorValidationHelper.ValidateColorArgument(Args, 0, Location, errors);
        }

        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitColor(this);
        }

        public override string ToString() =>
            $"Color({Args[0]}) at {Location.Line}:{Location.Column}";
    }
}
