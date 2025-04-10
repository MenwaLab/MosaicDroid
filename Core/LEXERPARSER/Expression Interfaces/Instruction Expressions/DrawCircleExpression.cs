public class DrawCircleExpression : IExpression
{
    private readonly IExpression dirX;
    private readonly IExpression dirY;
    private readonly IExpression radius;
    
    public DrawCircleExpression(IExpression dirX, IExpression dirY, IExpression radius)
    {
        this.dirX = dirX;
        this.dirY = dirY;
        this.radius = radius;
    }
    
    public int Interpret(Context context)
    {
        int dX = dirX.Interpret(context);
        int dY = dirY.Interpret(context);
        int rad = radius.Interpret(context);
        //context.DrawCircle(dX, dY, rad);
        Console.WriteLine($"Drawing circle at direction ({dX}, {dY}) with radius {rad}");
        return 0;
    }
}
