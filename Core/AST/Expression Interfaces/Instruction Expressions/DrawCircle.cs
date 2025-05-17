public class DrawCircleCommand : CallNode
    {
        public DrawCircleCommand(IReadOnlyList<Expression> args, CodeLocation loc) : base(TokenValues.DrawCircle, args, loc)
        {
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            bool ok = base.CheckSemantic(context, scope, errors);
            
            if(!ok) return false;

            for(int i=0;i<3;i++)
            {
                if(Args[i] is not Number num || !num.IsInt)
                {
                    errors.Add(new CompilingError(Args[i].Location, ErrorCode.Invalid, $"DrawCircle argument #{i+1} must be an integer"));

                    ok=false;
                }
            }

            if(!ok) return false;

            Number dxNum=(Number)Args[0];
            Number dyNum=(Number)Args[1];
            Number radNum=(Number)Args[2];

            double dxD   = (double)dxNum.Value;
        double dyD   = (double)dyNum.Value;
        double radD = (double)radNum.Value;

        int dx   = (int)dxD;
        int dy   = (int)dyD;
        int radius = (int)radD;

        if(dx<-1 || dx>1)
        {
            errors.Add(new CompilingError(dxNum.Location, ErrorCode.Invalid, $"DrawCircle: dx must be -1,0 or 1; got {dx}"));

            ok=false;
        }
        if (dy < -1 || dy > 1)
        {
            errors.Add(new CompilingError(
                dyNum.Location,
                ErrorCode.Invalid,
                $"DrawCircle: dirY must be –1, 0, or 1; got {dy}."
            ));
            ok = false;
        }
         if (radius < 0)
        {
            errors.Add(new CompilingError(
                radNum.Location,
                ErrorCode.Invalid,
                $"DrawCircle: radius must be > 0; got {radius}."
            ));
            ok = false;
        }

        return ok;
        }

        public override string ToString() =>
        $"DrawLine({Args[0]}, {Args[1]}, {Args[2]}) at {Location.Line}:{Location.Column}";
    }