public class GetColorCountExpression : Expression
    {
        public ColorOptions Color { get; }
        public Expression X1 { get; }
        public Expression Y1 { get; }
        public Expression X2 { get; }
        public Expression Y2 { get; }

        public GetColorCountExpression(
            ColorOptions color,
            Expression x1, Expression y1,
            Expression x2, Expression y2,
            CodeLocation loc) : base(loc)
        {
            Color = color;
            X1 = x1; Y1 = y1; X2 = x2; Y2 = y2;
        }

        public override ExpressionType Type { get; set; }
        public override object? Value { get; set; }

        public override void Evaluate() => Value = 1;

        public override bool CheckSemantic(Context ctx, Scope sc, List<CompilingError> errs)
        {
            bool ok = X1.CheckSemantic(ctx, sc, errs)
                   & Y1.CheckSemantic(ctx, sc, errs)
                   & X2.CheckSemantic(ctx, sc, errs)
                   & Y2.CheckSemantic(ctx, sc, errs);

            if (new[]{X1,Y1,X2,Y2}.Any(e => e.Type != ExpressionType.Number))
            {
                errs.Add(new CompilingError(Location, ErrorCode.Invalid,
                    "GetColorCount requires four numeric coordinates."));
                Type = ExpressionType.ErrorType;
                return false;
            }

            Type = ExpressionType.Number;
            return ok;
        }

        public override string ToString() =>
            $"GetColorCount({Color}, {X1}, {Y1}, {X2}, {Y2})";
    }