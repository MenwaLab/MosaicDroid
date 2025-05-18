public static class FunctionFactory
{
    public static Expression Create(string name, List<Expression> args, CodeLocation loc, List<CompilingError> errors)
    {
        switch (name)
        {
            case TokenValues.GetActualX:
                return new GetActualXExpression(args,loc);
            case TokenValues.GetActualY:
                return new GetActualYExpression(args,loc);
            case TokenValues.GetCanvasSize:
                return new GetCanvasSize(args,loc);
            case TokenValues.GetColorCount:
                return new GetColorCountExpression(args, loc);
            case TokenValues.IsBrushColor:
                return new IsBrushColorExpression(args, loc);
            case TokenValues.IsBrushSize:
                return new IsBrushSizeExpression(args, loc);
            case TokenValues.IsCanvasColor:
                return new IsCanvasColorExpression(args, loc);
            default:
                errors.Add(new CompilingError(loc, ErrorCode.Invalid, $"Unknown function: {name}"));
                return new NoOpExpression(loc);
        }
    }
}