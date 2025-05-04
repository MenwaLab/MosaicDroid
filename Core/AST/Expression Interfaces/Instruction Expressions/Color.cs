public class ColorCommand : ASTNode
{
    public ColorLiteralExpression ColorLiteral { get; }

    public ColorCommand(ColorLiteralExpression colorLiteral, CodeLocation loc)
        : base(loc)
    {
        ColorLiteral = colorLiteral;
    }

public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
{
    if (ColorLiteral.Value is not string text)
    {
        errors.Add(new CompilingError(Location, ErrorCode.Invalid, "Color() expects a string literal"));
        return false;
    }

    // Directly check against enum names (case-insensitive)
    bool isValidColor = Enum.GetNames(typeof(ColorOptions))
        .Any(name => name.Equals(text, StringComparison.OrdinalIgnoreCase));

    if (!isValidColor)
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
        $"Color({ColorLiteral.Value}) at {Location.Line}:{Location.Column}";
}
