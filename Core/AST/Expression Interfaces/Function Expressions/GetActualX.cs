public class GetActualXExpression : FunctionCallExpression
{
    public GetActualXExpression(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.GetActualX, args, loc)
    {
    }

    public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
    {
        // base does arity==0 & type==Number for you
        return base.CheckSemantic(ctx, scope, errors);
    }

    // stub out Evaluate so it compiles; we’ll come back later
    public override void Evaluate()
    {
        // TODO: once you have a robot/canvas, set Value = ctx.WallE.X
        Value = 0;         
        Type  = ExpressionType.Number;
    }

    public override ExpressionType Type { get; set; }
    public override object? Value   { get; set; }

    public override string ToString() => $"{Name}()";
}
