public abstract class CallNode : ASTNode
{
    public string Name { get; }
    public IReadOnlyList<Expression> Args { get; }

    public CallNode(string name, IReadOnlyList<Expression> args, CodeLocation loc)
      : base(loc)
    {
        Name = name;
        Args = args;
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool ok = true;
        var spec = ArgumentRegistry.Get(Name);
        if (spec == null)
        {
            
            errors.Add(new CompilingError(Location, ErrorCode.Invalid,
                $"Unknown command or function '{Name}'."));
            return false;
        }

        if (Args.Count != spec.ArgsCount)
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid,
                $"'{Name}' expects {spec.ArgsCount} arguments, got {Args.Count}."));
            ok = false;
        }

        for (int i = 0; i < Args.Count && i < spec.ExpectedTypes.Length; i++)
        {
            var expr = Args[i];
            ExpressionType actualType;

            if (expr is Number)            
                actualType = ExpressionType.Number;
            else if (expr is StringExpression ||expr is ColorLiteralExpression)
                actualType = ExpressionType.Text;
            else if (expr is VariableExpression v)
                actualType = context.GetVariableType(v.Name);
            // function call o expresión compuesta
            else
            {
                expr.CheckSemantic(context, scope, errors);
                actualType = expr.Type;
            }

            if (actualType != spec.ExpectedTypes[i])
            {
                errors.Add(new CompilingError(expr.Location, ErrorCode.Invalid,
                    $"Argument {i+1} of '{Name}' must be {spec.ExpectedTypes[i]}, but got {actualType}."));
                ok = false;
            }
            else
            {
                // solo si el tipo de arg es correcto entonces sigue la recursión
                if (!(expr is Number || expr is StringExpression || expr is ColorLiteralExpression))
                    ok &= expr.CheckSemantic(context, scope, errors);
            }
        }

        return ok;
    }
    public abstract void Accept(IStmtVisitor visitor);
}
public class CommandNode : CallNode
{
    public CommandNode(string name, IReadOnlyList<Expression> args, CodeLocation loc)
      : base(name, args, loc) { }

    public override void Accept(IStmtVisitor visitor)
    {
        // no-op
    }
}
