namespace MosaicDroid.Core
{
    public class StringExpression : AtomExpression
    {

        public override ExpressionType Type
        {
            get => ExpressionType.Text;
            set { /* no-op */ }
        }

        public override object? Value { get; set; }

        public StringExpression(string rawValue, CodeLocation loc)
            : base(loc)
        {
            if (rawValue.StartsWith("\"") && rawValue.EndsWith("\""))
                Value = rawValue.Substring(1, rawValue.Length - 2);
            else
                Value = rawValue;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            return true;
        }

        public override void Evaluate()
        {
        }
        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitString(this);
        }

        public override string ToString() => $"\"{Value}\"";
    }
}
