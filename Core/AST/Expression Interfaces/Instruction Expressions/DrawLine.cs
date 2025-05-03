public class DrawLineCommand : ASTNode
    {
        public Expression DirX { get; }
        public Expression DirY { get; }
        public Expression Distance { get; }

        public DrawLineCommand(Expression dirX, Expression dirY, Expression distance, CodeLocation loc) : base(loc)
        {
            DirX = dirX; DirY = dirY; Distance = distance;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            bool okX = DirX.CheckSemantic(context, scope, errors);
            bool okY = DirY.CheckSemantic(context, scope, errors);
            bool okD = Distance.CheckSemantic(context, scope, errors);
            if (DirX.Type != ExpressionType.Number || DirY.Type != ExpressionType.Number || Distance.Type != ExpressionType.Number)
            {
                errors.Add(new CompilingError(Location, ErrorCode.Invalid, "DrawLine requires numeric dirX, dirY, distance."));
                return false;
            }
            return okX && okY && okD;
        }

        public override string ToString() => $"DrawLine({DirX}, {DirY}, {Distance}) at {Location.Line}:{Location.Column}";
    }