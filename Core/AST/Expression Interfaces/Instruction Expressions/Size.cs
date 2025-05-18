// SizeCommand.cs

public class SizeCommand : CallNode
{
    public SizeCommand(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.Size, args, loc)
    {
    }

    public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
    {
        // 1) Arity check
        if (Args.Count != 1)
        {
            errors.Add(new CompilingError(
                Location,
                ErrorCode.InvalidArgCount,
                $"Size() expects exactly 1 argument, but got {Args.Count}."
            ));
            return false;
        }

        // 2) Type check: must be numeric
        var expr = Args[0];
        if (expr.Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(
                Location,
                ErrorCode.TypeArgMismatch,
                "Size() expects a numeric argument."
            ));
            return false;
        }

        // 3) If it's a literal Number, adjust even values
        if (expr is Number literal)
        {
            // Determine integer part without direct cast
            double raw = (double)literal.Value;
            int iv = (int)Math.Floor(raw);

            // If it's not already an integer literal, skip rounding here
            if (literal.IsInt)
            {
                if (iv % 2 == 0)
                {
                    // Round down to nearest odd
                    int rounded = iv - 1;
                    if (rounded < 1) rounded = 1;  // enforce minimum brush size = 1
                    literal.Value = rounded;
                    // still an integer
                    //literal.IsInt = true;
                }
            }
            else
            {
                // Non‐literal number: we can't know integerness at compile‐time.
                // Optionally warn or defer to runtime.
            }
        }

        return true;
    }

    public override string ToString() =>
        $"Size({Args[0]}) at {Location.Line}:{Location.Column}";
}