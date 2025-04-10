public class SubtractExpression : BinaryExpression
{
    public SubtractExpression(IExpression left, IExpression right) : base(left, right) {}

    public override int Interpret(Context context)
    {
        return Left.Interpret(context) - Right.Interpret(context);
    }
}
