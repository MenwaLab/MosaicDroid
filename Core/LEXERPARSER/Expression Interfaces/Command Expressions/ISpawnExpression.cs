public class SpawnExpression : IExpression
{
    private readonly IExpression x;
    private readonly IExpression y;

    public SpawnExpression(IExpression x, IExpression y)
    {
        this.x = x;
        this.y = y;
    }

    public int Interpret(Context context)
    {
        // Initialize Wall-E’s position in the context.
        int posX = x.Interpret(context);
        int posY = y.Interpret(context);
        // context.SetPosition(posX, posY); // Example call
        Console.WriteLine($"Spawning at ({posX}, {posY})");
        return 0;
    }
}
