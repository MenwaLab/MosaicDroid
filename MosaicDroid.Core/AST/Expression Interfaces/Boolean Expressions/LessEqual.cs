namespace MosaicDroid.Core
{
    public class LogicalLessEqualExpression : BinaryExpression
    {
        public LogicalLessEqualExpression(Expression left, Expression right, CodeLocation loc)
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

            if (Left.Type == ExpressionType.Number && Right.Type == ExpressionType.Number)
            {
                double l = (double)Left.Value!;
                double r = (double)Right.Value!;
                Value = ((double)Left.Value! <= (double)Right.Value!) ? 1 : 0;
            }
            else // both must be text
            {
                string l = (string)Left.Value!;
                string r = (string)Right.Value!;
                Value = (string.Compare(l, r, StringComparison.Ordinal) <= 0) ? 1 : 0;
            }
        }



        public override bool CheckSemantic(Context ctx, Scope sc, List<CompilingError> errs)
        {
            bool okL = Left.CheckSemantic(ctx, sc, errs);
            bool okR = Right.CheckSemantic(ctx, sc, errs);

            bool isNum = Left.Type == ExpressionType.Number && Right.Type == ExpressionType.Number;
            bool isText = Left.Type == ExpressionType.Text && Right.Type == ExpressionType.Text;

            if (!isNum && !isText)
            {
                ErrorHelpers.InvalidOperands(errs, Location, "less than or equal to");
                Type = ExpressionType.ErrorType;
                return false;
            }

            Type = ExpressionType.Boolean;
            return okL && okR;
        }

        public override string ToString() =>
            Value == null
            ? $"({Left} <= {Right})"
            : Value.ToString()!;
        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitLessEqual(this);
        }
    }
}
