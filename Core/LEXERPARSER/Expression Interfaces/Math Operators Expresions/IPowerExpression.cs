public class PowerExpression : IExpression
{
    private readonly IExpression left;
    private readonly IExpression right;
    
    public PowerExpression(IExpression left, IExpression right)
    {
        this.left = left;
        this.right = right;
    }
    
    public int Interpret(Context context)
    {
        return (int)Math.Pow(left.Interpret(context), right.Interpret(context));
    }
}
