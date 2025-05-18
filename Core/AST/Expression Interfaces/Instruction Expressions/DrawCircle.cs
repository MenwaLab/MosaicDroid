public class DrawCircleCommand : CallNode
    {
        public DrawCircleCommand(IReadOnlyList<Expression> args, CodeLocation loc) : base(TokenValues.DrawCircle, args, loc)
        {
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            bool ok = base.CheckSemantic(context, scope, errors);
            
            if(!ok) return false;

            if (!ArgumentSpec.EnsureAllIntegerLiterals(Args, 3, "DrawCircle", errors))
            return false;

            var values = Args.Cast<Number>().Select(n => (int)(double)n.Value).ToArray();

        ok &= ArgumentSpec.EnsureDirectionInRange(values[0], Args[0].Location, "DrawCircle: dirX", errors);
        ok &= ArgumentSpec.EnsureDirectionInRange(values[1], Args[1].Location, "DrawCircle: dirY", errors);
        ok &= ArgumentSpec.EnsurePositive(values[2], Args[2].Location, "DrawCircle: radius", errors);

        return ok;
        }

        public override string ToString() =>
        $"DrawCircle({Args[0]}, {Args[1]}, {Args[2]}) at {Location.Line}:{Location.Column}";
    }