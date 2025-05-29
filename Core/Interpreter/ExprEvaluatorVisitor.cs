public class ExpressionEvaluatorVisitor : IExprVisitor<double>
{
    private readonly Dictionary<string,double> _variables;
    private readonly MatrixInterpreterVisitor _canvasContext;

    public ExpressionEvaluatorVisitor(Dictionary<string,double> variables,
                                      MatrixInterpreterVisitor canvasContext)
    {
        _variables = variables;
        _canvasContext = canvasContext;
    }

    public double VisitNumber(Number num) => (double) num.Value!;

    public double VisitVariable(VariableExpression v)
    {
        if (!_variables.TryGetValue(v.Name, out var val))
            throw new InvalidOperationException($"Undefined variable {v.Name}");
        return val;
    }

    public double VisitString(StringExpression s)
        => 0.0;
    public double VisitColorLiteral(ColorLiteralExpression c)
        => 0.0;
    public double VisitAdd(Add a)
        => a.Left.Accept(this) + a.Right.Accept(this);
    public double VisitSub(Sub s)
        => s.Left.Accept(this) - s.Right.Accept(this);
    public double VisitMult(Mul m)
        => m.Left.Accept(this) * m.Right.Accept(this);
    public double VisitDiv(Div d)
        => d.Left.Accept(this) / d.Right.Accept(this);
    public double VisitMod(ModulusExpression m)
        => m.Left.Accept(this) % m.Right.Accept(this);

    public double VisitPow(PowerExpression p)
        => Math.Pow(p.Left.Accept(this), p.Right.Accept(this));

    // Comparisons: return 1.0 for true, 0.0 for false
    public double VisitLess(LogicalLessExpression l)
        => l.Left.Accept(this) < l.Right.Accept(this) ? 1 : 0;
    public double VisitLessEqual(LogicalLessEqualExpression l)
        => l.Left.Accept(this) <= l.Right.Accept(this) ? 1 : 0;
    public double VisitGreater(LogicalGreaterExpression g)
        => g.Left.Accept(this) > g.Right.Accept(this) ? 1 : 0;
    public double VisitGreaterEqual(LogicalGreaterEqualExpression g)
        => g.Left.Accept(this) >= g.Right.Accept(this) ? 1 : 0;
    public double VisitEqual(LogicalEqualExpression e)
        => e.Left.Accept(this) == e.Right.Accept(this) ? 1 : 0;

    public double VisitAnd(LogicalAndExpression a)
        => (a.Left.Accept(this) != 0 && a.Right.Accept(this) != 0) ? 1 : 0;
    public double VisitOr(LogicalOrExpression o)
        => (o.Left.Accept(this) != 0 || o.Right.Accept(this) != 0) ? 1 : 0;

    // Functions—may need to delegate back to the canvas context:
    public double VisitActualX(GetActualXExpression fn)
        => _canvasContext.CurrentX;
    public double VisitActualY(GetActualYExpression fn)
        => _canvasContext.CurrentY;
    public double VisitCanvasSize(GetCanvasSize fn)
        => _canvasContext.Size;
    public double VisitColorCount(GetColorCountExpression fn)
        => _canvasContext.GetColorCount(
               fn.Color, 
               (int)fn.X1.Accept(this),
               (int)fn.Y1.Accept(this),
               (int)fn.X2.Accept(this),
               (int)fn.Y2.Accept(this)
           );
    public double VisitBrushColor(IsBrushColorExpression fn)
{
    string colorArg = fn.Color;
    string wantedBrushCode = _canvasContext.GetBrushCodeForColor(colorArg);
    return _canvasContext.BrushCode == wantedBrushCode ? 1 : 0;
}
    public double VisitBrushSize(IsBrushSizeExpression fn)
{
    double size = fn.Args[0].Accept(this); // let evaluator handle ANY expression
    return _canvasContext.BrushSize == (int)size ? 1.0 : 0.0;
}

    public double VisitCanvasColor(IsCanvasColorExpression fn)
    => _canvasContext.CanvasColor(
           fn.Color,
           (int)fn.OffsetX.Accept(this),
           (int)fn.OffsetY.Accept(this)
       )
       ? 1 : 0;
    public double VisitNoOp(NoOpExpression noOp)
    {
        // any unrecognized expression just evaluates to 0.0
        return 0.0;
    }
    
}
