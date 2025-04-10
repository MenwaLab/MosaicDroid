public abstract class BinaryExpression : IExpression
{
    protected readonly IExpression Left;
    protected readonly IExpression Right;

    protected BinaryExpression(IExpression left, IExpression right)
    {
        Left = left;
        Right = right;
    }

    public abstract int Interpret(Context context);
}
