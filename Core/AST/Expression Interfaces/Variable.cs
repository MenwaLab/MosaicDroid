public class VariableExpression : AtomExpression
{
    public string Name { get; }

    public VariableExpression(string name, CodeLocation loc)
        : base(loc)
    {
        Name = name;
    }

    public override ExpressionType Type { get; set; }
    public override object? Value { get; set; }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        var varType = context.GetVariableType(Name);
        if (varType == ExpressionType.ErrorType)
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, $"Undeclared variable: {Name}"));
            Type = ExpressionType.ErrorType;
            return false;
        }

        Type = varType;
        return true;
    }

    public override void Evaluate()
    {
        Value = 0; // Later during execution you will get the real value.
    }

    public override string ToString() => Name;
    public override string DebugPrint()
    => null;
}

