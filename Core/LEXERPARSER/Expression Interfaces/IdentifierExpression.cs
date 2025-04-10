public class IdentifierExpression : IExpression
{
    private readonly string name;

    public IdentifierExpression(string name)
    {
        this.name = name;
    }

    public int Interpret(Context context)
    {
        return context.GetVariable(name); // You’ll need a Context class with variable storage
    }
}
