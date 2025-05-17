// ColorCommand.cs

public class ColorCommand : CallNode
{
    public ColorCommand(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.Color, args, loc)
    {
    }

public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
{
    // 1) Arity
    if (Args.Count != 1)
    {
        errors.Add(new CompilingError(
            Location,
            ErrorCode.InvalidArgCount,
            $"Color() expects exactly 1 argument, but got {Args.Count}."
        ));
        return false;
    }

    // 2) Must be a literal (we wrapped everything in ColorLiteralExpression)
    if (Args[0] is not ColorLiteralExpression atom)
    {
        errors.Add(new CompilingError(
            Location,
            ErrorCode.Invalid,
            "Color() expects a string literal."
        ));
        return false;
    }

    // 3) Pull out the raw text and do an *exact* enum‐name lookup
    var text = atom.Value as string ?? "";
    var enumNames = Enum.GetNames(typeof(ColorOptions));

    bool isExact = enumNames
        .Any(n => string.Equals(n, text, StringComparison.OrdinalIgnoreCase));

    if (!isExact)
    {
        errors.Add(new CompilingError(
            Location,
            ErrorCode.Invalid,
            $"Unknown color: '{text}'"
        ));
        return false;
    }

    return true;
}


    public override string ToString() =>
        $"Color({Args[0]}) at {Location.Line}:{Location.Column}";
}

