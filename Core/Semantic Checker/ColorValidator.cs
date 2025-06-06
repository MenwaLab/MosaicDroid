using System.Drawing;

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
            

            ErrorHelpers.ArgMismatch(errors, location, "Color", argIndex+1, ExpressionType.Text,  args[argIndex].Type);

            return false;
        }

        string colorName = (string)colorLit.Value!;
        if (!Enum.GetNames(typeof(ColorOptions))
                 .Any(n => n.Equals(colorName, StringComparison.Ordinal))) // Ordinal: case sensitive
        {
            ErrorHelpers.InvalidColor(errors,location,colorName);
            return false;
        }

        return true;
    }
}