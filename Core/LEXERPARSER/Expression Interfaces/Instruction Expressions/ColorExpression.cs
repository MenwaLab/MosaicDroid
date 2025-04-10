public class ColorExpression : IExpression
{
    private readonly string color;
    
    public ColorExpression(string color)
    {
        this.color = color;
    }
    
    public int Interpret(Context context)
    {
        //context.SetBrushColor(color);
        Console.WriteLine($"Brush color set to: {color}");
        return 0;
    }
}

