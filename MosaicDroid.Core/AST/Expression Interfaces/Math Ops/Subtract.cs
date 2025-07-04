namespace MosaicDroid.Core
{
    public class Sub : BinaryExpression
    {
        public override ExpressionType Type { get; set; }
        public override object? Value { get; set; }

        public Sub(CodeLocation location) : base(location) { }

        public override void Evaluate()
        {
            Right.Evaluate();
            Left.Evaluate();

            Value = (double)Right.Value - (double)Left.Value;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            bool right = Right.CheckSemantic(context, scope, errors);
            bool left = Left.CheckSemantic(context, scope, errors);
            if (Right.Type != ExpressionType.Number || Left.Type != ExpressionType.Number)
            {
                ErrorHelpers.InvalidOperands(errors, Location, "substraction");
                Type = ExpressionType.ErrorType;
                return false;
            }

            Type = ExpressionType.Number;
            return right && left;
        }

        public override string ToString()
        {
            if (Value == null)
            {
                return String.Format("({0} - {1})", Left, Right);
            }
            return Value.ToString();
        }
        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitSub(this);
        }
    }
}