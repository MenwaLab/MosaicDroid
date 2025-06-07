namespace MosaicDroid.Core
{
    public class IsBrushSizeExpression : FunctionCallExpression
    {
        public IsBrushSizeExpression(IReadOnlyList<Expression> args, CodeLocation loc)
            : base(TokenValues.IsBrushSize, args, loc)
        {
            Type = ExpressionType.Number;
        }

        public override ExpressionType Type { get; set; }
        public override object? Value { get; set; }

        public override void Evaluate() => Value = 0; // Stub

        public override string ToString() => $"{TokenValues.IsBrushSize}({Args[0]})";

        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitBrushSize(this);
        }
    }
}