public class IsBrushSizeExpression : IExpression
{
    private readonly IExpression sizeExpr;
    
    public IsBrushSizeExpression(IExpression sizeExpr)
    {
        this.sizeExpr = sizeExpr;
    }
    
    public int Interpret(Context context)
    {
        int size = sizeExpr.Interpret(context);
        //return context.IsBrushSize(size) ? 1 : 0;
        return size;
    }
}
