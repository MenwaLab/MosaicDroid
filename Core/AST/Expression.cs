
public abstract class Expression : ASTNode
{
    public abstract void Evaluate();

    public abstract ExpressionType Type { get; set; }

    public abstract object? Value { get; set; }
    //public abstract void Evaluate(Context ctx);

    public Expression(CodeLocation location) : base (location) { }

    public abstract string DebugPrint();

     public static implicit operator Expression(string v)
    {
        throw new NotImplementedException();
    } 
}