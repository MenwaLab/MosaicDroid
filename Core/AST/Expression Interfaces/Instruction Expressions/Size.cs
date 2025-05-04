// SizeCommand.cs

public class SizeCommand : CallNode
{
    public SizeCommand(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.Size, args, loc)
    {
    }

    public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
    {
        // 1) base does arity‐check (1) + type‐check (Number)
        bool ok = base.CheckSemantic(ctx, scope, errors);

        if (ok && Args[0] is Number n)
        {
            // only literal Number nodes carry an IsInt flag
            if (n.IsInt)
            {
                var iv = (int)n.Value;
                if (iv % 2 == 0)
                {
                    // round down to nearest odd
                    n.Value = iv - 1;
                    // leave n.IsInt = true, since it's still an integer
                }
            }
            else
            {
                // non‐literal: nothing to do here, runtime may truncate/round
            }
        }

        return ok;
    }

    public override string ToString() =>
        $"Size({Args[0]}) at {Location.Line}:{Location.Column}";
}
