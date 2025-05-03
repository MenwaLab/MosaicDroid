public class IsBrushColorExpression : Expression
    {
        public ColorOptions Color { get; }
        public IsBrushColorExpression(ColorOptions color, CodeLocation loc) : base(loc)
        {
            Color = color;
        }

        public override ExpressionType Type { get; set; }
        public override object? Value { get; set; }

        public override void Evaluate()
        {
            // (runtime engine will set Value to 1/0)
            Value = 1; 
        }

        public override bool CheckSemantic(Context ctx, Scope sc, List<CompilingError> errs)
        {
            Type = ExpressionType.Boolean;
            return true;
        }

        public override string ToString() => $"IsBrushColor({Color})";
    }