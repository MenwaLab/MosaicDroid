namespace MosaicDroid.Core
{
    public class ArgumentSpec
    {
        public int ArgsCount { get; }
        public ExpressionType[] ExpectedTypes { get; }

        public ArgumentSpec(int argsCount, params ExpressionType[] types)
        {
            ArgsCount = argsCount;
            ExpectedTypes = types;
        }

        public static bool EnsureAllIntegerLiterals(IReadOnlyList<Expression> args, int count, string commandName, List<CompilingError> errors)
        {
            bool ok = true;
            for (int i = 0; i < count; i++)
            {
                if (args[i] is not Number num || !num.IsInt)
                {
                    ErrorHelpers.ArgMismatch(errors, args[i].Location, commandName, i + 1, args[i].Type, ExpressionType.Number);
                    ok = false;
                }
            }
            return ok;
        }

        public static bool EnsureDirectionInRange(int value, CodeLocation loc, string label, List<CompilingError> errors)
        {
            if (value < -1 || value > 1)
            {
                ErrorHelpers.InvalidDirection(errors, loc, value, 0);
                return false;
            }
            return true;
        }

        public static bool EnsurePositive(int value, CodeLocation loc, string label, List<CompilingError> errors)
        {
            if (value <= 0)
            {
                ErrorHelpers.InvalidValue(errors, loc, $"{label} must be > 0; got {value}");
                return false;
            }
            return true;
        }
    }
}

