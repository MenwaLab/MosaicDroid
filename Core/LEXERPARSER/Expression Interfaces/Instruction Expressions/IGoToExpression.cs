public class GotoExpression : IExpression
{
    private readonly string label;
    private readonly IExpression condition; // Could be a boolean expression.
 
    public GotoExpression(string label, IExpression condition)
    {
        this.label = label;
        this.condition = condition;
    }

    public int Interpret(Context context)
    {
        // Evaluate the condition; if true, update the context’s instruction pointer.
        if (condition.Interpret(context) != 0)
        {
            //int targetIndex = context.GetLabelIndex(label);
            //context.JumpTo(targetIndex); // Your interpreter loop must honor this.
            //Console.WriteLine($"[Goto] Jumping to label {label} at instruction {targetIndex}");
        }
        return 0;
    }
}
