public class ArgumentSpec
{
    public int ArgsCount { get; }
    public ExpressionType[] ExpectedTypes { get; }

    public ArgumentSpec(int argsCount, params ExpressionType[] types)
    {
        ArgsCount=argsCount;
        ExpectedTypes = types;
    }

    public bool Validate(List<Expression> args, CodeLocation loc, List<CompilingError> errors)
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
                errors.Add(new CompilingError(args[i].Location, ErrorCode.TypeArgMismatch,
                    $"Arg {i+1}: Expected {ExpectedTypes[i]}, got {args[i].Type}"));
                return false;
            }
        }
        return true;
    }
}