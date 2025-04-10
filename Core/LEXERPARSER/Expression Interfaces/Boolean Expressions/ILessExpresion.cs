public class LessExpression : IExpression
{
    private readonly IExpression left;
    private readonly IExpression right;
    
    public LessExpression(IExpression left, IExpression right)
    {
        this.left = left;
        this.right = right;
    }
    
    public int Interpret(Context context)
    {
        return left.Interpret(context) < right.Interpret(context) ? 1 : 0;
    }
}
