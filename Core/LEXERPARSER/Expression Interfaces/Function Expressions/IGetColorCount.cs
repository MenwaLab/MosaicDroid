public class GetColorCountExpression : IExpression
{
    private readonly Lexer.AllColors color;
    private readonly IExpression exprX1, exprY1, exprX2, exprY2;

    public GetColorCountExpression(Lexer.AllColors color,
        IExpression exprX1, IExpression exprY1, IExpression exprX2, IExpression exprY2)
    {
        this.color = color;
        this.exprX1 = exprX1;
        this.exprY1 = exprY1;
        this.exprX2 = exprX2;
        this.exprY2 = exprY2;
    }

    public int Interpret(Context context)
    {
        int x1 = exprX1.Interpret(context);
        int y1 = exprY1.Interpret(context);
        int x2 = exprX2.Interpret(context);
        int y2 = exprY2.Interpret(context);
        // return context.GetColorCount(color, x1, y1, x2, y2);
        return 1;
    }
}
