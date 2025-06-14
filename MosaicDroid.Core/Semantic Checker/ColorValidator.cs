using System;
using System.Collections.Generic;
using System.ComponentModel;         // para TypeConverter y convertir el nombre del color/RGB a un color WPF
using System.Windows.Media;        
namespace MosaicDroid.Core
{

    public static class ColorValidationHelper
    {
        private static readonly ColorConverter WpfConverter = new ColorConverter();

        public static bool IsValidWpfColor(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return false;

            try
            {
                ColorConverter.ConvertFromString(raw.Trim());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidateColorArgument(IReadOnlyList<Expression> args,int argIndex,CodeLocation location,List<CompilingError> errors)
        {
            if (argIndex >= args.Count || args[argIndex] is not ColorLiteralExpression colorLit)
            {
                ErrorHelpers.ArgMismatch(errors, location, "Color", argIndex + 1, ExpressionType.Text, args[argIndex].Type);
                return false;
            }

            string raw = ((string)colorLit.Value!).Trim();
            if (!IsValidWpfColor(raw))
            {
                ErrorHelpers.InvalidColor(errors, location, raw);
                return false;
            }
            return true;
        }
    }
}