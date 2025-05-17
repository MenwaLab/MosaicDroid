public class ColorLiteralExpression : AtomExpression
{
    
    
    private ExpressionType _type;
    public override ExpressionType Type
        {
            get => _type;
            set => _type = value;
        }
    public override object? Value { get; set; }
    public ColorLiteralExpression(string color, CodeLocation location)
            : base(location)
        {
            Value = color;
            // You could introduce a dedicated ExpressionType.Color if desired;
            _type = ExpressionType.Text;
        }
    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            // Color literals are always semantically valid
            return true;
        }
    public override void Evaluate() { /* no-op */ }
    public override string ToString() => Value?.ToString() ?? string.Empty;
}
