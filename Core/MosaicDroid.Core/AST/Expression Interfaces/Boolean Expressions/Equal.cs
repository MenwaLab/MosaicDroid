namespace MosaicDroid.Core
{
    public class LogicalEqualExpression : BinaryExpression
    {
        public LogicalEqualExpression(Expression left, Expression right, CodeLocation loc)
            : base(loc)
        {
            Left = left;
            Right = right;
        }

        public override ExpressionType Type { get; set; }
        public override object? Value { get; set; }

        public override void Evaluate()
        {
            Left.Evaluate();
            Right.Evaluate();
            Value = (Left.Value?.Equals(Right.Value) ?? false) ? 1 : 0;
        }

        public override bool CheckSemantic(Context ctx, Scope sc, List<CompilingError> errs)
        {
            bool okL = Left.CheckSemantic(ctx, sc, errs);
            bool okR = Right.CheckSemantic(ctx, sc, errs);

            bool isNumberComparison = Left.Type == ExpressionType.Number && Right.Type == ExpressionType.Number;
            bool isTextComparison = Left.Type == ExpressionType.Text && Right.Type == ExpressionType.Text;

            bool isBoolNumComparison =
                (Left.Type == ExpressionType.Boolean && Right.Type == ExpressionType.Number) ||
                (Left.Type == ExpressionType.Number && Right.Type == ExpressionType.Boolean);


            if (!isNumberComparison && !isTextComparison && !isBoolNumComparison)
            {
                ErrorHelpers.InvalidOperands(errs, Location, "equality");
                Type = ExpressionType.ErrorType;
                return false;
            }

            Type = ExpressionType.Boolean;
            return okL && okR;
        }

        public override string ToString() =>
            Value == null ? $"({Left} == {Right})" : Value.ToString()!;

        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitEqual(this);
        }
    }
}