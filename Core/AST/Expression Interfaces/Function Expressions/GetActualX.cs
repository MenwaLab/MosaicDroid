public class GetActualXExpression : Expression
    {
        public GetActualXExpression(CodeLocation loc) : base(loc) { }

        public override ExpressionType Type { get; set; }
        public override object? Value { get; set; }

        public override void Evaluate() => Value = 1;
        public override bool CheckSemantic(Context ctx, Scope sc, List<CompilingError> errs)
        {
            Type = ExpressionType.Number;
            return true;
        }
        public override string ToString() => "GetActualX()";
    }