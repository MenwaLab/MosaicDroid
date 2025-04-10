public class IsBrushColorExpression : IExpression
{
    private readonly string color;
    
    public IsBrushColorExpression(string color)
    {
        this.color = color;
    }
    
    public int Interpret(Context context)
    {
        //return context.IsBrushColor(color) ? 1 : 0;
        return 1;
    }
}
