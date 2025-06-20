namespace MosaicDroid.Core
{
    public class ColorLiteralExpression : AtomExpression
    {

        private ExpressionType _type;
        public override ExpressionType Type
        {
            get => _type;
            set => _type = value;
        }
        public override object? Value { get; set; }
        public ColorLiteralExpression(string color, CodeLocation location)
                : base(location)
        {
            Value = color;
            _type = ExpressionType.Text;
        }
        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            return true;
        }
        public override void Evaluate() { /* no-op */ }
        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitColorLiteral(this);
        }
        public override string ToString() => Value?.ToString() ?? string.Empty;
    }
}