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

        /*  public bool Validate(List<Expression> args, CodeLocation loc, List<CompilingError> errors)
         {
             // Argument count check
             if (args.Count !=ArgsCount)
             {
                 errors.Add(new CompilingError(loc, ErrorCode.InvalidArgCount, 
                     $"Expected ArgsCount args, got {args.Count}"));
                 return false;
             }


             // Type checking
             for (int i = 0; i < args.Count; i++)
             {
                 if (i < ExpectedTypes.Length && args[i].Type != ExpectedTypes[i])
                 {
                     errors.Add(new CompilingError(args[i].Location, ErrorCode.ArgMismatch,
                         $"Arg {i+1}: Expected {ExpectedTypes[i]}, got {args[i].Type}"));
                     return false;
                 }
             }
             return true;
         } */

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

