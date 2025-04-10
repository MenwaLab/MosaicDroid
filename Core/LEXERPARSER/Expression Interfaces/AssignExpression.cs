public class AssignExpression : IExpression
{
    private readonly string variable;
    private readonly IExpression expression;

    public AssignExpression(string variable, IExpression expression)
    {
        this.variable = variable;
        this.expression = expression;
    }

    public int Interpret(Context context)
    {
        int value = expression.Interpret(context);
        context.SetVariable(variable, value);
        return value;
    }
}
