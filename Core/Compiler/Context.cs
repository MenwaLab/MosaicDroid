public class Context
{
    public Dictionary<string, ExpressionType> Variables { get; } = new Dictionary<string, ExpressionType>();
    public HashSet<string> Labels { get; } = new HashSet<string>(); //pros?
    public bool SpawnSeen { get; set; } = false; //y zheli

    /* public void DeclareVariable(string name, ExpressionType type, CodeLocation loc, List<CompilingError> errors)
    {
        if (Variables.ContainsKey(name))
            errors.Add(new CompilingError(loc, ErrorCode.Invalid, $"Variable '{name}' is already declared."));
        else
            Variables[name] = type;
    } *///no references

    public ExpressionType GetVariableType(string name)
    {
        return Variables.TryGetValue(name, out var t) ? t : ExpressionType.ErrorType;
    }
    public void SetVariableType(string name, ExpressionType type)
    {
        Variables[name] = type;
    }

    public void DeclareLabel(string label, CodeLocation loc, List<CompilingError> errors)
    {
        if (IsLabelDeclared(label))
            //errors.Add(new CompilingError(loc, ErrorCode.Invalid, $"Label '{label}' is already defined."));
            ErrorHelpers.DuplicateLabel(errors,loc,label);
        else
            Labels.Add(label);
    }

    public bool IsLabelDeclared(string label)
    {
        return Labels.Contains(label);
    }
}