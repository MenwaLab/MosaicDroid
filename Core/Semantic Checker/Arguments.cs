public class ArgumentSpec
{
    public int ArgsCount { get; }
    public ExpressionType[] ExpectedTypes { get; }

    public ArgumentSpec(int argsCount, params ExpressionType[] types)
    {
        ArgsCount=argsCount;
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
                errors.Add(new CompilingError(args[i].Location, ErrorCode.ArgMismatch,
                    $"{commandName} argument #{i + 1} must be an integer literal."));
                ok = false;
            }
        }
        return ok;
    }

    public static bool EnsureDirectionInRange(int value, CodeLocation loc, string label, List<CompilingError> errors)
    {
        if (value < -1 || value > 1)
        {
            errors.Add(new CompilingError(loc, ErrorCode.Invalid,
                $"{label} must be –1, 0, or 1; got {value}."));
            return false;
        }
        return true;
    }

    public static bool EnsurePositive(int value, CodeLocation loc, string label, List<CompilingError> errors)
    {
        if (value <= 0)
        {
            errors.Add(new CompilingError(loc, ErrorCode.Invalid,
                $"{label} must be > 0; got {value}."));
            return false;
        }
        return true;
    }
}

