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

            for (int i = 0; i < 5; i++)
        {
            if (Args[i] is not Number num || !num.IsInt)
            {
                errors.Add(new CompilingError(
                    Args[i].Location,
                    ErrorCode.Invalid,
                    $"DrawRectangle argument #{i+1} must be an integer literal."
                ));
                ok = false;
            }
        }
        if (!ok) return false;
        
        Number dxNum   = (Number)Args[0];
        Number dyNum   = (Number)Args[1];
        Number distNum = (Number)Args[2];
        Number widthNum   = (Number)Args[3];
        Number heightNum   = (Number)Args[4];

        double dxD   = (double)dxNum.Value;
        double dyD   = (double)dyNum.Value;
        double distD = (double)distNum.Value;
        double widthD   = (double)widthNum.Value;
        double heightD   = (double)heightNum.Value;

        int dx   = (int)dxD;
        int dy   = (int)dyD;
        int dist = (int)distD;
        int width   = (int)widthD;
        int height   = (int)heightD;

        if (dx < -1 || dx > 1)
        {
            errors.Add(new CompilingError(
                dxNum.Location,
                ErrorCode.Invalid,
                $"DrawRectangle: dirX must be –1, 0, or 1; got {dx}."
            ));
            ok = false;
        }
        if (dy < -1 || dy > 1)
        {
            errors.Add(new CompilingError(
                dyNum.Location,
                ErrorCode.Invalid,
                $"DrawRectangle: dirY must be –1, 0, or 1; got {dy}."
            ));
            ok = false;
        }

        // 5) distance must be non‑negative
        if (dist < 0)
        {
            errors.Add(new CompilingError(
                distNum.Location,
                ErrorCode.Invalid,
                $"DrawRectangle: distance must be > 0; got {dist}."
            ));
            ok = false;
        }

        if (height < 0)
        {
            errors.Add(new CompilingError(
                distNum.Location,
                ErrorCode.Invalid,
                $"DrawRectangle: height must be > 0; got {height}."
            ));
            ok = false;
        }

        if (width < 0)
        {
            errors.Add(new CompilingError(
                distNum.Location,
                ErrorCode.Invalid,
                $"DrawRectangle: width must be > 0; got {width}."
            ));
            ok = false;
        }
        return ok;


        
        
        }

        public override string ToString() =>
        $"DrawRectangle({Args[0]}, {Args[1]}, {Args[2]}, {Args[3]}, {Args[4]}) at {Location.Line}:{Location.Column}";
    }