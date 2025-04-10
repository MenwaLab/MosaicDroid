public class AddExpression : BinaryExpression
{
    public AddExpression(IExpression left, IExpression right) : base(left, right) {}

    public override int Interpret(Context context)
    {
        return Left.Interpret(context) + Right.Interpret(context);
    }
}


    
    
