// Non-abstract base for commands


// Function expressions remain abstract
public abstract class FunctionCallExpression : Expression
{
    public string Name { get; }
    public IReadOnlyList<Expression> Args { get; }

    protected FunctionCallExpression(string name, 
                                   IReadOnlyList<Expression> args, 
                                   CodeLocation loc)
      : base(loc)
    {
        Name = name;
        Args = args;
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        // Use concrete CommandNode for validation
        var tempNode = new CommandNode(Name, Args, Location);
        return tempNode.CheckSemantic(context, scope, errors);
    }
}