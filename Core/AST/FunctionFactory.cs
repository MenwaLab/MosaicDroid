public static class FunctionFactory
{
    public static Expression Create(string name, List<Expression> args, CodeLocation loc, List<CompilingError> errors)
    {
        switch (name)
        {
            case TokenValues.GetActualX:
                return new GetActualXExpression(loc);
            case TokenValues.GetActualY:
                return new GetActualYExpression(loc);
            case TokenValues.GetCanvasSize:
                return new GetCanvasSizeExpression(loc);
            case TokenValues.GetColorCount:
                if (args.Count == 5 && args[0] is ColorLiteralExpression colorLit)
                {
                    // Extract color name from literal
                    var colorName = Enum.Parse<ColorOptions>((string)colorLit.Value!);
                    return new GetColorCountExpression(colorName, args[1], args[2], args[3], args[4], loc);
                }
                errors.Add(new CompilingError(loc, ErrorCode.Invalid, "GetColorCount expects (color, x1, y1, x2, y2)"));
                return new NoOpExpression(loc);
            case TokenValues.IsBrushColor:
                if (args.Count == 1 && args[0] is ColorLiteralExpression cLit)
                {
                    var cn = Enum.Parse<ColorOptions>((string)cLit.Value!);
                    return new IsBrushColorExpression(cn, loc);
                }
                errors.Add(new CompilingError(loc, ErrorCode.Invalid, "IsBrushColor expects (color)"));
                return new NoOpExpression(loc);
            case TokenValues.IsBrushSize:
                if (args.Count == 1)
                    return new IsBrushSizeExpression(args[0], loc);
                errors.Add(new CompilingError(loc, ErrorCode.Invalid, "IsBrushSize expects (size)"));
                return new NoOpExpression(loc);
            case TokenValues.IsCanvasColor:
                if (args.Count == 3 && args[0] is ColorLiteralExpression clit)
                {
                    var cn2 = Enum.Parse<ColorOptions>((string)clit.Value!);
                    return new IsCanvasColorExpression(cn2, args[1], args[2], loc);
                }
                errors.Add(new CompilingError(loc, ErrorCode.Invalid, "IsCanvasColor expects (color, vertical, horizontal)"));
                return new NoOpExpression(loc);
            default:
                errors.Add(new CompilingError(loc, ErrorCode.Invalid, $"Unknown function: {name}"));
                return new NoOpExpression(loc);
        }
    }
}