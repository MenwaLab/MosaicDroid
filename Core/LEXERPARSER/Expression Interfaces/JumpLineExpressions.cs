public class JumpLineExpression : IExpression
{
    public int Interpret(Context context)
    {
        // You might, for example, update a program counter in the context
        // or simply treat it as a no-op. For now, return 0.
        return 0;
    }
    public override string ToString() => "<JumpLine>";
}
