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
        for (int i = 0; i < 3; i++)
        {
            if (Args[i] is not Number num || !num.IsInt)
            {
                errors.Add(new CompilingError(
                    Args[i].Location,
                    ErrorCode.Invalid,
                    $"DrawLine argument #{i+1} must be an integer literal."
                ));
                ok = false;
            }
        }
        if (!ok) return false;

        // 3) Safely unbox the double → int
        Number dxNum   = (Number)Args[0];
        Number dyNum   = (Number)Args[1];
        Number distNum = (Number)Args[2];

        double dxD   = (double)dxNum.Value;
        double dyD   = (double)dyNum.Value;
        double distD = (double)distNum.Value;

        int dx   = (int)dxD;
        int dy   = (int)dyD;
        int dist = (int)distD;

        // 4) dirX/dirY must be –1, 0, or 1
        if (dx < -1 || dx > 1)
        {
            errors.Add(new CompilingError(
                dxNum.Location,
                ErrorCode.Invalid,
                $"DrawLine: dirX must be –1, 0, or 1; got {dx}."
            ));
            ok = false;
        }
        if (dy < -1 || dy > 1)
        {
            errors.Add(new CompilingError(
                dyNum.Location,
                ErrorCode.Invalid,
                $"DrawLine: dirY must be –1, 0, or 1; got {dy}."
            ));
            ok = false;
        }

        // 5) distance must be non‑negative
        if (dist < 0)
        {
            errors.Add(new CompilingError(
                distNum.Location,
                ErrorCode.Invalid,
                $"DrawLine: distance must be > 0; got {dist}."
            ));
            ok = false;
        }

        return ok;
    }

    public override string ToString() =>
        $"DrawLine({Args[0]}, {Args[1]}, {Args[2]}) at {Location.Line}:{Location.Column}";
}
