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
    bool ok = base.CheckSemantic(ctx, scope, errors);
        if (!ok) return false;

        return ColorValidationHelper.ValidateColorArgument(Args, 0, Location, errors);

    // 2) Must be a literal (we wrapped everything in ColorLiteralExpression)
    /* if (Args[0] is not ColorLiteralExpression atom)
    {
        errors.Add(new CompilingError(
            Location,
            ErrorCode.TypeArgMismatch,
            "Color() expects a string literal."
        ));
        return false;
    } */

    // 3) Pull out the raw text and do an *exact* enum‐name lookup
    //var text = atom.Value as string ?? "";
    //var enumNames = Enum.GetNames(typeof(ColorOptions));

    //var atom = (ColorLiteralExpression)Args[0];
    //var text = atom.Value as string ?? "";

    /* bool isExact = enumNames
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

    return true; */
    /* if (!Enum.GetNames(typeof(ColorOptions))
                .Any(n => string.Equals(n, text, StringComparison.OrdinalIgnoreCase)))
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, $"Unknown color: '{text}'"));
            return false;
        }
        return true; */
    }



    public override string ToString() =>
        $"Color({Args[0]}) at {Location.Line}:{Location.Column}";
}

