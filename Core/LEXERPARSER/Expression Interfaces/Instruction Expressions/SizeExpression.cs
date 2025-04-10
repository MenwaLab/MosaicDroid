public class SizeExpression : IExpression
{
    private readonly IExpression sizeExpr;
    
    public SizeExpression(IExpression sizeExpr)
    {
        this.sizeExpr = sizeExpr;
    }
    
    public int Interpret(Context context)
    {
        int size = sizeExpr.Interpret(context);
        if (size % 2 == 0)
            size -= 1;
        //context.SetBrushSize(size);
        Console.WriteLine($"Brush size set to: {size}");
        return 0;
    }
}
