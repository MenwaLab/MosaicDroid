public class IsCanvasColorExpression : Expression
    {
        public ColorOptions Color { get; }
        public Expression Vertical { get; }
        public Expression Horizontal { get; }

        public IsCanvasColorExpression(ColorOptions color, Expression vert, Expression horz, CodeLocation loc) 
            : base(loc)
        {
            Color = color;
            Vertical = vert;
            Horizontal = horz;
        }

        public override ExpressionType Type { get; set; }
        public override object? Value { get; set; }

        public override void Evaluate() => Value = 1;

        public override bool CheckSemantic(Context ctx, Scope sc, List<CompilingError> errs)
        {
            bool vOk = Vertical.CheckSemantic(ctx, sc, errs);
            bool hOk = Horizontal.CheckSemantic(ctx, sc, errs);
            if (Vertical.Type != ExpressionType.Number || Horizontal.Type != ExpressionType.Number)
            {
                errs.Add(new CompilingError(Location, ErrorCode.Invalid,
                    "IsCanvasColor requires two numeric offsets."));
                Type = ExpressionType.ErrorType;
                return false;
            }
            Type = ExpressionType.Boolean;
            return vOk && hOk;
        }

        public override string ToString() => $"IsCanvasColor({Color}, {Vertical}, {Horizontal})";
    }