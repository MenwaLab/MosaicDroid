public class IsBrushSizeExpression : Expression
    {
        public Expression SizeExpr { get; }
        public IsBrushSizeExpression(Expression sizeExpr, CodeLocation loc) : base(loc)
        {
            SizeExpr = sizeExpr;
        }

        public override ExpressionType Type { get; set; }
        public override object? Value { get; set; }

        public override void Evaluate() => Value = 1;

        public override bool CheckSemantic(Context ctx, Scope sc, List<CompilingError> errs)
        {
            var ok = SizeExpr.CheckSemantic(ctx, sc, errs);
            if (SizeExpr.Type != ExpressionType.Number)
            {
                errs.Add(new CompilingError(Location, ErrorCode.Invalid, "IsBrushSize requires numeric argument."));
                Type = ExpressionType.ErrorType;
                return false;
            }
            Type = ExpressionType.Boolean;
            return ok;
        }

        public override string ToString() => $"IsBrushSize({SizeExpr})";
    }