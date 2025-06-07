namespace MosaicDroid.Core
{
    public interface IExprVisitor<TResult>
    {
        TResult VisitNumber(Number num);
        TResult VisitVariable(VariableExpression v);
        TResult VisitString(StringExpression s);
        TResult VisitColorLiteral(ColorLiteralExpression c);
        TResult VisitAdd(Add a);
        TResult VisitSub(Sub s);
        TResult VisitMult(Mul m);
        TResult VisitDiv(Div d);
        TResult VisitMod(ModulusExpression mod);
        TResult VisitPow(PowerExpression p);

        // Comparisons
        TResult VisitLess(LogicalLessExpression l);
        TResult VisitLessEqual(LogicalLessEqualExpression l);
        TResult VisitGreater(LogicalGreaterExpression g);
        TResult VisitGreaterEqual(LogicalGreaterEqualExpression g);
        TResult VisitEqual(LogicalEqualExpression e);

        // Boolean ops
        TResult VisitAnd(LogicalAndExpression a);
        TResult VisitOr(LogicalOrExpression o);

        // Function calls
        TResult VisitActualX(GetActualXExpression fn);
        TResult VisitActualY(GetActualYExpression fn);
        TResult VisitCanvasSize(GetCanvasSize fn);
        TResult VisitColorCount(GetColorCountExpression fn);
        TResult VisitBrushColor(IsBrushColorExpression fn);
        TResult VisitBrushSize(IsBrushSizeExpression fn);
        TResult VisitCanvasColor(IsCanvasColorExpression fn);

        TResult VisitNoOp(NoOpExpression fn);
    }
}