public class LogicalOrExpression : IExpression
{
    private readonly IExpression left;
    private readonly IExpression right;
    
    public LogicalOrExpression(IExpression left, IExpression right)
    {
        this.left = left;
        this.right = right;
    }
    
    public int Interpret(Context context)
    {
        return (left.Interpret(context) != 0 || right.Interpret(context) != 0) ? 1 : 0;
    }
}
