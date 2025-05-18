public static class ColorValidationHelper
{
    public static bool ValidateColorArgument(
        IReadOnlyList<Expression> args,
        int argIndex,
        CodeLocation location,
        List<CompilingError> errors)
    {
        // Check if the argument is a ColorLiteralExpression
        if (argIndex >= args.Count || args[argIndex] is not ColorLiteralExpression colorLit)
        {
            errors.Add(new CompilingError(location, ErrorCode.Invalid,
                $"Argument {argIndex + 1} must be a valid color literal."));
            return false;
        }

        // Validate the color name against the ColorOptions enum (case-sensitive)
        string colorName = (string)colorLit.Value!;
        if (!Enum.GetNames(typeof(ColorOptions))
                 .Any(n => n.Equals(colorName, StringComparison.Ordinal))) // <<-- Ordinal (case-sensitive)
        {
            errors.Add(new CompilingError(location, ErrorCode.Invalid,
                $"Unknown color: '{colorName}'"));
            return false;
        }

        return true;
    }
}