// DrawLineCommand.cs

public class DrawLineCommand : CallNode
{
    public DrawLineCommand(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.DrawLine, args, loc)
    {
    }

    public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
    {
        // 1) Let the base CallNode/ArgumentRegistry enforce:
        //    - correct count (3)
        //    - ExpressionType.Number for each arg
        bool ok = base.CheckSemantic(ctx, scope, errors);
        if (!ok) return false;

        // 2) Now ensure *literal* integers (not e.g. variables or doubles).
        //    We check Number.IsInt, which your Number nodes carry.
        if (!ArgumentSpec.EnsureAllIntegerLiterals(Args, 3, "DrawLine", errors))
            return false;

            var values = Args.Cast<Number>().Select(n => (int)(double)n.Value).ToArray();

        ok &= ArgumentSpec.EnsureDirectionInRange(values[0], Args[0].Location, "DrawLine: dirX", errors);
        ok &= ArgumentSpec.EnsureDirectionInRange(values[1], Args[1].Location, "DrawLine: dirY", errors);
        ok &= ArgumentSpec.EnsurePositive(values[2], Args[2].Location, "DrawLine: distance", errors);

        return ok;
        }

    public override string ToString() =>
        $"DrawLine({Args[0]}, {Args[1]}, {Args[2]}) at {Location.Line}:{Location.Column}";
}
