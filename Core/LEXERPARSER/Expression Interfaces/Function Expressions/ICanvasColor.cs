public class IsCanvasColorExpression : IExpression
{
    private readonly Lexer.AllColors color;
    private readonly IExpression vertical;
    private readonly IExpression horizontal;
    
    public IsCanvasColorExpression(Lexer.AllColors color, IExpression vertical, IExpression horizontal)
    {
        this.color = color;
        this.vertical = vertical;
        this.horizontal = horizontal;
    }
    
    public int Interpret(Context context)
    {
        int v = vertical.Interpret(context);
        int h = horizontal.Interpret(context);
        //return context.IsCanvasColor(color, v, h) ? 1 : 0;
        return 1;
    }
}
