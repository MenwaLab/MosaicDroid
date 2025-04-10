public class IsBrushColorExpression : IExpression
{
    private readonly Lexer.AllColors color;
    
    public IsBrushColorExpression(Lexer.AllColors color)
    {
        this.color = color;
    }
    
    public int Interpret(Context context)
    {
        //return context.IsBrushColor(color) ? 1 : 0;
        return 1;
    }
}
