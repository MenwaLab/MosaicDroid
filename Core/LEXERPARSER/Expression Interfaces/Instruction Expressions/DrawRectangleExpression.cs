public class DrawRectangleExpression : IExpression
{
    private readonly IExpression dirX;
    private readonly IExpression dirY;
    private readonly IExpression distance;
    private readonly IExpression width;
    private readonly IExpression height;
    
    public DrawRectangleExpression(IExpression dirX, IExpression dirY, IExpression distance, IExpression width, IExpression height)
    {
        this.dirX = dirX;
        this.dirY = dirY;
        this.distance = distance;
        this.width = width;
        this.height = height;
    }
    
    public int Interpret(Context context)
    {
        int dX = dirX.Interpret(context);
        int dY = dirY.Interpret(context);
        int dist = distance.Interpret(context);
        int w = width.Interpret(context);
        int h = height.Interpret(context);
        //context.DrawRectangle(dX, dY, dist, w, h);
        Console.WriteLine($"Drawing rectangle in direction ({dX}, {dY}) with distance {dist}, width {w} and height {h}");
        return 0;
    }
}
