// ColorCommand.cs

public class ColorCommand : CallNode
{
    public ColorCommand(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.Color, args, loc)
    {
    }

    public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
    {
        // 1) arity + basic type‐check (one TEXT comes from CallNode)
        bool ok = base.CheckSemantic(ctx, scope, errors);

        // 2) if it parsed as a literal, pull out its Value and test the enum
        if (ok && Args.Count == 1 && Args[0] is ColorLiteralExpression atom)
        {
            // atom.Value is object but always set from our parser to a string
            var text = atom.Value as string ?? "";
            if (!Enum.TryParse<ColorOptions>(text, ignoreCase: true, out _))
            {
                errors.Add(new CompilingError(
                    Location,
                    ErrorCode.Invalid,
                    $"Unknown color: '{text}'"
                ));
                ok = false;
            }
        }

        return ok;
    }

    public override string ToString() =>
        $"Color({Args[0]}) at {Location.Line}:{Location.Column}";
}
