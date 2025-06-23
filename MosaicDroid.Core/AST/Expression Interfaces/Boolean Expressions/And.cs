namespace MosaicDroid.Core
{
    public class LogicalAndExpression : BinaryExpression
    {
        public LogicalAndExpression(Expression left, Expression right, CodeLocation loc) : base(loc)
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
            Value = ((int)Left.Value! != 0 && (int)Right.Value! != 0) ? 1 : 0;
        }

        public override bool CheckSemantic(Context ctx, Scope sc, List<CompilingError> errs)
        {
            bool okL = Left.CheckSemantic(ctx, sc, errs);
            bool okR = Right.CheckSemantic(ctx, sc, errs);

            /*
            bool isNumberComparison = Left.Type == ExpressionType.Number && Right.Type == ExpressionType.Number;
            bool isTextComparison = Left.Type == ExpressionType.Text && Right.Type == ExpressionType.Text;

            if (!isNumberComparison && !isTextComparison)
            {
                ErrorHelpers.InvalidOperands(errs, Location, "boolean and");
                Type = ExpressionType.ErrorType;
                return false;
            }
            */
            if (Left.Type != ExpressionType.Boolean || Right.Type != ExpressionType.Boolean)
            {
                ErrorHelpers.InvalidOperands(errs, Location, "boolean and");
                Type = ExpressionType.ErrorType;
                return false;
            }

            Type = ExpressionType.Boolean;
            return okL && okR;
        }

        public override string ToString() =>
            Value == null ? $"({Left} && {Right})" : Value.ToString()!;

        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitAnd(this);
        }


    }
}