public class SpawnCommand : ASTNode
    {
        public Expression X { get; }
        public Expression Y { get; }

        public SpawnCommand(Expression x, Expression y, CodeLocation loc) : base(loc)
        {
            X = x;
            Y = y;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            bool okX = X.CheckSemantic(context, scope, errors);
            bool okY = Y.CheckSemantic(context, scope, errors);
            if (X.Type != ExpressionType.Number || Y.Type != ExpressionType.Number)
            {
                errors.Add(new CompilingError(Location, ErrorCode.Invalid, "Spawn requires numeric x and y."));
                return false;
            }
            return okX && okY;
        }

        public override string ToString() => $"Spawn({X}, {Y}) at {Location.Line}:{Location.Column}";
    }