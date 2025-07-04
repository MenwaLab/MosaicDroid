namespace MosaicDroid.Core
{
    public class LogicalOrExpression : BinaryExpression
    {
        public LogicalOrExpression(Expression left, Expression right, CodeLocation loc) : base(loc)
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
            Value = ((int)Left.Value! != 0 || (int)Right.Value! != 0) ? 1 : 0;
        }

        public override bool CheckSemantic(Context ctx, Scope sc, List<CompilingError> errs)
        {
            bool okL = Left.CheckSemantic(ctx, sc, errs);
            bool okR = Right.CheckSemantic(ctx, sc, errs);

            if (Left.Type != ExpressionType.Boolean || Right.Type != ExpressionType.Boolean)
            {
                ErrorHelpers.InvalidOperands(errs, Location, "boolean or");
                Type = ExpressionType.ErrorType;
                return false;
            }
            Type = ExpressionType.Boolean;
            return okL || okR;
        }

        public override string ToString() =>
            Value == null ? $"({Left} || {Right})" : Value.ToString()!;

        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitOr(this);
        }
    }
}