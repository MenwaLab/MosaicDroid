public class DrawLineExpression : IExpression
{
    private readonly IExpression dirX;
    private readonly IExpression dirY;
    private readonly IExpression distance;
    
    public DrawLineExpression(IExpression dirX, IExpression dirY, IExpression distance)
    {
        this.dirX = dirX;
        this.dirY = dirY;
        this.distance = distance;
    }
    
    public int Interpret(Context context)
    {
        int xDir = dirX.Interpret(context);
        int yDir = dirY.Interpret(context);
        int dist = distance.Interpret(context);
        
        // Let the context handle drawing – it must update the canvas and Wall-E’s position.
        //context.DrawLine(xDir, yDir, dist);
        Console.WriteLine($"Drawing line in direction ({xDir}, {yDir}) for {dist} pixels");
        return 0;
    }
}
