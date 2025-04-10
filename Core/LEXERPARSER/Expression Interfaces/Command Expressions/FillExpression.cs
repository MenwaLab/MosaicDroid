public class FillExpression : IExpression
{
    public int Interpret(Context context)
    {
        //context.Fill();
        Console.WriteLine("Filling contiguous area");
        return 0;
    }
}
