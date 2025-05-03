public class DrawRectangleCommand : ASTNode
    {
        public Expression DirX { get; }
        public Expression DirY { get; }
        public Expression Distance { get; }
        public Expression Width { get; }
        public Expression Height { get; }

        public DrawRectangleCommand(
            Expression dirX,
            Expression dirY,
            Expression distance,
            Expression width,
            Expression height,
            CodeLocation loc) : base(loc)
        {
            DirX = dirX; DirY = dirY;
            Distance = distance; Width = width; Height = height;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            bool okX = DirX.CheckSemantic(context, scope, errors);
            bool okY = DirY.CheckSemantic(context, scope, errors);
            bool okD = Distance.CheckSemantic(context, scope, errors);
            bool okW = Width.CheckSemantic(context, scope, errors);
            bool okH = Height.CheckSemantic(context, scope, errors);
            if (new[]{DirX,DirY,Distance,Width,Height} .Any(e => e.Type != ExpressionType.Number))
            {
                errors.Add(new CompilingError(Location, ErrorCode.Invalid,
                    "DrawRectangle requires numeric dirX, dirY, distance, width, height."));
                return false;
            }
            return okX && okY && okD && okW && okH;
        }

        public override string ToString() =>
            $"DrawRectangle({DirX}, {DirY}, {Distance}, {Width}, {Height}) at {Location.Line}:{Location.Column}";
    }