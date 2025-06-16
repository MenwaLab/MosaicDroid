namespace MosaicDroid.Core
{
    public class GetActualYExpression : FunctionCallExpression
    {
        public GetActualYExpression(IReadOnlyList<Expression> args, CodeLocation loc)
          : base(TokenValues.GetActualY, args, loc)
        {
            Type = ExpressionType.Number;
        }

        public override ExpressionType Type { get; set; }
        public override object? Value { get; set; }

        public override void Evaluate()
        {
            Value = 0;  
        }

        public override string ToString() => $"{Name}()";
        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitActualY(this);
        }
    }
}