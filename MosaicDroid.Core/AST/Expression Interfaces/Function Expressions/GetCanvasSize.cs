namespace MosaicDroid.Core
{
    public class GetCanvasSize : FunctionCallExpression
    {

        public GetCanvasSize(IReadOnlyList<Expression> args, CodeLocation loc)
          : base(TokenValues.GetCanvasSize, args, loc)
        {
            Type = ExpressionType.Number;
        }

        public override ExpressionType Type { get; set; }
        public override object? Value { get; set; }

        public override void Evaluate()
        {
            Value = 0;  
        }

        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitCanvasSize(this);
        }
        public override string ToString() => $"{Name}()";
    }
}