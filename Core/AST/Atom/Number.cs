public class Number : AtomExpression
{
    public Number(double value, CodeLocation location) : base(location)
    {
        Value = value;
    }
    public bool IsInt
    {
        get
        {
            int a;
            return int.TryParse(Value.ToString(), out a);
        }
    }

    public override ExpressionType Type { get; set; } = ExpressionType.Number;

    public override object? Value { get; set; }
    
    
    
    public override bool CheckSemantic(Context context, Scope table, List<CompilingError> errors)
    {
        return true;
    }

    public override void Evaluate()
    {
        
    }

    public override string ToString()
    {
        return String.Format("{0}",Value);
    }
    public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
    {
        return visitor.VisitNumber(this);
    }
    
}