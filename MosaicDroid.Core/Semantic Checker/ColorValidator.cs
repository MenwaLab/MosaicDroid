// file: ColorValidationHelper.cs in MosaicDroid.Core
/*sing System;
using System.Collections.Generic;
using System.Drawing;  // from System.Drawing.Common
using System.Linq;

namespace MosaicDroid.Core
{
    public static class ColorValidationHelper
    {
        public static bool ValidateColorArgument(
            IReadOnlyList<Expression> args,
            int argIndex,
            CodeLocation location,
            List<CompilingError> errors)
        {
            if (argIndex >= args.Count
             || args[argIndex] is not ColorLiteralExpression colorLit)
            {
                ErrorHelpers.ArgMismatch(
                    errors, location, "Color", argIndex + 1,
                    ExpressionType.Text, args[argIndex].Type);
                return false;
            }

            string raw = ((string)colorLit.Value!).Trim();
            if (string.IsNullOrEmpty(raw))
            {
                ErrorHelpers.InvalidColor(errors, location, raw);
                return false;
            }

            try
            {
                // FromHtml accepts "#RRGGBB", "#AARRGGBB" (any hex length),
                // and *all* known HTML/CSS color names (e.g. "DarkTurquoise", "Fuchsia", "Tomato", etc.)
                ColorTranslator.FromHtml(raw);
                return true;
            }
            catch
            {
                ErrorHelpers.InvalidColor(errors, location, raw);
                return false;
            }
        }
    }
}


*/
// file: ColorValidationHelper.cs in MosaicDroid.Core
// file: ColorValidationHelper.cs
// ColorValidationHelper.cs (in MosaicDroid.Core)
using System;
using System.Collections.Generic;
using System.ComponentModel;         // for TypeConverter
using System.Windows.Media;        // PresentationCore.dll → you must add <UseWPF>true</UseWPF> in your Core .csproj

namespace MosaicDroid.Core
{

    public static class ColorValidationHelper
    {
        // You *can* share a single ColorConverter instance, but its ConvertFromString is a static call.
        private static readonly ColorConverter WpfConverter = new ColorConverter();

        public static bool IsValidWpfColor(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return false;

            try
            {
                // throws if not a WPF color name or valid hex
                ColorConverter.ConvertFromString(raw.Trim());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidateColorArgument(
            IReadOnlyList<Expression> args,
            int argIndex,
            CodeLocation location,
            List<CompilingError> errors)
        {
            if (argIndex >= args.Count
             || args[argIndex] is not ColorLiteralExpression colorLit)
            {
                ErrorHelpers.ArgMismatch(
                    errors, location, "Color", argIndex + 1,
                    ExpressionType.Text, args[argIndex].Type);
                return false;
            }

            string raw = ((string)colorLit.Value!).Trim();
            if (!IsValidWpfColor(raw))
            {
                ErrorHelpers.InvalidColor(errors, location, raw);
                return false;
            }

            // First check if WPF knows this name or hex:
            return true;
        }
    }
}

/*if (!Enum.GetNames(typeof(ColorOptions))
                     .Any(n => n.Equals(colorName, StringComparison.Ordinal))) // Ordinal: case sensitive
            {
                ErrorHelpers.InvalidColor(errors, location, colorName);
                return false;
            }

            return true; }}} */
