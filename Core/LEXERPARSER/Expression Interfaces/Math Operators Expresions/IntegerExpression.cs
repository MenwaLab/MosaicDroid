using System.Linq.Expressions;

public class IntegerExpression: IExpression
{
    private readonly int number;

    public IntegerExpression(int number)
    {
        this.number=number;
    }
 
    public int Interpret(Context context)
    {
        return number;
    }
}    
