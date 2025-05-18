public class DrawRectangleCommand : CallNode
{
    public DrawRectangleCommand(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.DrawRectangle, args, loc)
    {
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool ok = base.CheckSemantic(context, scope, errors);
        if (!ok) return false;

        if (!ArgumentSpec.EnsureAllIntegerLiterals(Args, 5, "DrawRectangle", errors))
            return false;

        // Safe cast
        var values = Args.Cast<Number>().Select(n => (int)(double)n.Value).ToArray();

        ok &= ArgumentSpec.EnsureDirectionInRange(values[0], Args[0].Location, "DrawRectangle: dirX", errors);
        ok &= ArgumentSpec.EnsureDirectionInRange(values[1], Args[1].Location, "DrawRectangle: dirY", errors);
        ok &= ArgumentSpec.EnsurePositive(values[2], Args[2].Location, "DrawRectangle: distance", errors);
        ok &= ArgumentSpec.EnsurePositive(values[3], Args[3].Location, "DrawRectangle: width", errors);
        ok &= ArgumentSpec.EnsurePositive(values[4], Args[4].Location, "DrawRectangle: height", errors);

        return ok;
    }

    public override string ToString() =>
        $"DrawRectangle({Args[0]}, {Args[1]}, {Args[2]}, {Args[3]}, {Args[4]}) at {Location.Line}:{Location.Column}";
}
