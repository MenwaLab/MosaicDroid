namespace MosaicDroid.Core
{
    public class GetColorCountExpression : FunctionCallExpression
    {
        public GetColorCountExpression(IReadOnlyList<Expression> args, CodeLocation loc)
            : base(TokenValues.GetColorCount, args, loc)
        {
            Type = ExpressionType.Number;
        }

        public override ExpressionType Type { get; set; }
        public override object? Value { get; set; }

        public override void Evaluate() => Value = 0; 

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            bool ok = base.CheckSemantic(context, scope, errors);
            if (!ok) return false;

            return ColorValidationHelper.ValidateColorArgument(Args, 0, Args[0].Location, errors);
        }

        public string Color => ((ColorLiteralExpression)Args[0]).Value!.ToString()!;
        public Expression X1 => Args[1];
        public Expression Y1 => Args[2];
        public Expression X2 => Args[3];
        public Expression Y2 => Args[4];
        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitColorCount(this);
        }

        public override string ToString() =>
            $"{TokenValues.GetColorCount}({string.Join(", ", Args)})";
    }
}