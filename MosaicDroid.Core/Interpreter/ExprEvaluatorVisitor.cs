using System.Resources;

namespace MosaicDroid.Core
{
    public class ExpressionEvaluatorVisitor : IExprVisitor<double>
    {
        private readonly Dictionary<string, double> _variables;
        private readonly MatrixInterpreterVisitor _canvasContext;
        private readonly List<CompilingError> _runtimeErrors;

        private static readonly ResourceManager _resmgr =
    new ResourceManager("MosaicDroid.Core.Resources.Strings", typeof(MatrixInterpreterVisitor).Assembly);

        public ExpressionEvaluatorVisitor(Dictionary<string, double> variables,
                                          MatrixInterpreterVisitor canvasContext, List<CompilingError> runtimeErrors)
        {
            _variables = variables;
            _canvasContext = canvasContext;
            _runtimeErrors = runtimeErrors;
        }

        public double VisitNumber(Number num)
        {
            if (num.Value is int i)
                return (double)i;
            else
                return (double)num.Value!;
        }

        public double VisitVariable(VariableExpression v)
        {
            if (!_variables.TryGetValue(v.Name, out var val))
                throw new InvalidOperationException($"{_resmgr.GetString("UndefinedVariable")} {v.Name}"); 
            return val;
        }

        public double VisitString(StringExpression s)
            => 0.0;
        public double VisitColorLiteral(ColorLiteralExpression c)
            => 0.0;
        // operadores aritmeticos
        public double VisitAdd(Add a)
            => a.Left.Accept(this) + a.Right.Accept(this);
        public double VisitSub(Sub s)
            => s.Left.Accept(this) - s.Right.Accept(this);
        public double VisitMult(Mul m)
            => m.Left.Accept(this) * m.Right.Accept(this);
        public double VisitDiv(Div d)
        {
            double left = d.Left.Accept(this);
            double right = d.Right.Accept(this);
            CheckDivisionByZero(right, d.Location);
            return left / right;
        }
        public double VisitMod(ModulusExpression m)
        {
            double left = m.Left.Accept(this);
            double right = m.Right.Accept(this);
            CheckModulusByZero(right, m.Location);
            return left % right;

        }

        public double VisitPow(PowerExpression p)
        {
            double left = p.Left.Accept(this);
            double right = p.Right.Accept(this);
            CheckZeroPowerZero(left, right, p.Location);
            return Math.Pow(left, right);
        }

        // operadores booleanos
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

        // Funciones
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
            string colorArg = fn.Color.Trim();
            var current = _canvasContext.BrushCode.Trim();
            return string.Equals(current, colorArg, StringComparison.OrdinalIgnoreCase)
         ? 1.0
         : 0.0;
        }
        public double VisitBrushSize(IsBrushSizeExpression fn)
        {
            double size = fn.Args[0].Accept(this); 
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
            // cualquier expresion no reconocida evalua a 0.0
            return 0.0;
        }
        private void CheckDivisionByZero(double divisor, CodeLocation loc)
        {
            if (Math.Abs(divisor) < 1e-9)
            {
                ErrorHelpers.DivisionByZero(_runtimeErrors, loc);
                throw new PixelArtRuntimeException($"{_resmgr.GetString("DivByZero")} {loc.Line}:{loc.Column}");
            }

        }

        private void CheckModulusByZero(double divisor, CodeLocation loc)
        {
            if (Math.Abs(divisor) < 1e-9)
            {
                ErrorHelpers.ModulusByZero(_runtimeErrors, loc);
                throw new PixelArtRuntimeException($"{_resmgr.GetString("ModulusByZero")} {loc.Line}:{loc.Column}");
            }
        }

        private void CheckZeroPowerZero(double baseVal, double exponent, CodeLocation loc)
        {
            if (Math.Abs(baseVal) < 1e-9 && Math.Abs(exponent) < 1e-9)
            {
                ErrorHelpers.ZeroToZeroPower(_runtimeErrors, loc);
                throw new PixelArtRuntimeException($"{_resmgr.GetString("ZeroToZeroPower")} {loc.Line}:{loc.Column}");
            }
        }

    }
}
