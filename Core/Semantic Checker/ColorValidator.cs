public enum ColorOptions
{
Red, Blue, Green, Yellow, Orange, Purple, Black, White, Transparent
}
public static class ColorValidationHelper
{
    public static bool ValidateColorArgument(IReadOnlyList<Expression> args, int argIndex, CodeLocation location,
        List<CompilingError> errors)
    {
        if (argIndex >= args.Count || args[argIndex] is not ColorLiteralExpression colorLit)
        {
            errors.Add(new CompilingError(location, ErrorCode.ArgMismatch,
                $"Argument {argIndex + 1} must be a valid color literal."));
            return false;
        }

        string colorName = (string)colorLit.Value!;
        if (!Enum.GetNames(typeof(ColorOptions))
                 .Any(n => n.Equals(colorName, StringComparison.Ordinal))) // Ordinal: case sensitive
        {
            errors.Add(new CompilingError(location, ErrorCode.Invalid,
                $"Unknown color: '{colorName}'"));
            return false;
        }

        return true;
    }
}