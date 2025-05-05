public abstract class CallNode : ASTNode
{
    public string Name { get; }
    public IReadOnlyList<Expression> Args { get; }

    protected CallNode(string name, IReadOnlyList<Expression> args, CodeLocation loc)
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

        // 1) Arity‐check
        if (Args.Count != spec.ArgsCount)
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid,
                $"'{Name}' expects {spec.ArgsCount} arguments, got {Args.Count}."));
            ok = false;
        }

        // 2) Per‐argument type‐check
        for (int i = 0; i < Args.Count && i < spec.ExpectedTypes.Length; i++)
        {
            var expr = Args[i];
            ExpressionType actualType;

            // a) Literal numbers/strings/colors:
            if (expr is Number)            actualType = ExpressionType.Number;
            else if (expr is StringExpression ||
                     expr is ColorLiteralExpression)
                                           actualType = ExpressionType.Text;
            // b) A variable:
            else if (expr is VariableExpression v)
                actualType = context.GetVariableType(v.Name);
            // c) A function call or compound expr:
            else
            {
                // we *haven’t* recursed yet, so probe:
                expr.CheckSemantic(context, scope, errors);
                actualType = expr.Type;
            }

            // d) If it doesn’t match, report exactly one “type mismatch”:
            if (actualType != spec.ExpectedTypes[i])
            {
                errors.Add(new CompilingError(expr.Location, ErrorCode.Invalid,
                    $"Argument {i+1} of '{Name}' must be {spec.ExpectedTypes[i]}, but got {actualType}."));
                ok = false;
            }
            else
            {
                // only if the type was correct do we recurse fully to pick up
                // e.g. nested errors inside a sub‐expression
                if (!(expr is Number || expr is StringExpression || expr is ColorLiteralExpression))
                    ok &= expr.CheckSemantic(context, scope, errors);
            }
        }

        return ok;
    }
}
