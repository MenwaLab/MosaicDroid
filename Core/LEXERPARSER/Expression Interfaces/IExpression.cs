public interface IExpression
{
    int Interpret(Context context);
}public abstract class Expression
{
    public abstract int Interpret(Context context);
}