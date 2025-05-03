public class DrawCircleCommand : ASTNode
    {
        public Expression DirX { get; }
        public Expression DirY { get; }
        public Expression Radius { get; }

        public DrawCircleCommand(Expression dirX, Expression dirY, Expression radius, CodeLocation loc) : base(loc)
        {
            DirX = dirX; DirY = dirY; Radius = radius;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            bool okX = DirX.CheckSemantic(context, scope, errors);
            bool okY = DirY.CheckSemantic(context, scope, errors);
            bool okR = Radius.CheckSemantic(context, scope, errors);
            if (DirX.Type != ExpressionType.Number || DirY.Type != ExpressionType.Number || Radius.Type != ExpressionType.Number)
            {
                errors.Add(new CompilingError(Location, ErrorCode.Invalid, "DrawCircle requires numeric dirX, dirY, radius."));
                return false;
            }
            return okX && okY && okR;
        }

        public override string ToString() => $"DrawCircle({DirX}, {DirY}, {Radius}) at {Location.Line}:{Location.Column}";
    }