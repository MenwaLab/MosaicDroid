public class LabelDeclarationExpression : IExpression
{
    public string Label { get; }
    public LabelDeclarationExpression(string label)
    {
        Label = label;
    }

    public int Interpret(Context context)
    {
        // Let the context record the current instruction pointer (or node index)
        // so that when a GoTo occurs, the jump can be made.
        //context.DefineLabel(Label, context.CurrentInstructionIndex);
        return 0;
    }
    
    public override string ToString() => $"Label: {Label}";
}