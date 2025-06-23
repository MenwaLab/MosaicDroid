using System.Resources;

namespace MosaicDroid.Core
{
    public class MatrixInterpreterVisitor : IStmtVisitor
    {
        private readonly string[,] _canvas;
        private readonly Dictionary<string, double> _variables = new Dictionary<string, double>();
        private readonly ExpressionEvaluatorVisitor _exprEval;

        private readonly List<CompilingError> _runtimeErrors;

        private static readonly ResourceManager _resmgr = 
            new ResourceManager("MosaicDroid.Core.Resources.Strings", typeof(MatrixInterpreterVisitor).Assembly);

        public int Size { get; }
        public int CurrentX { get; private set; }
        public int CurrentY { get; private set; }
        public string BrushCode { get; private set; } = "  ";
        public int BrushSize { get; private set; } = 1;

        private int _executionSteps;
        private const int MaxExecutionSteps = 100000;

        public MatrixInterpreterVisitor(int size, List<CompilingError> runtimeErrors)
        {
            Size = size;
            _runtimeErrors = runtimeErrors;

            _canvas = new string[size, size];

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    _canvas[x, y] = "w ";

            _runtimeErrors = runtimeErrors ?? throw new PixelArtRuntimeException(nameof(runtimeErrors));
            _exprEval = new ExpressionEvaluatorVisitor(_variables, this, _runtimeErrors);
        }
        
        public void VisitSpawn(SpawnCommand cmd)
        {
            int x = (int)cmd.Args[0].Accept(_exprEval);
            int y = (int)cmd.Args[1].Accept(_exprEval);

            EnsureInBounds(x, y);


            CurrentX = x;
            CurrentY = y;
        }

        public void VisitColor(ColorCommand cmd)
        {
            var lit = (ColorLiteralExpression)cmd.Args[0];
            BrushCode = (string)lit.Value!;
        }


        public void VisitSize(SizeCommand cmd)
        {
            int k = (int)cmd.Args[0].Accept(_exprEval);
            if (k < 1) k = 1;
            if (k % 2 == 0) k--;
            BrushSize = k;
        }

        public void VisitDrawLine(DrawLineCommand cmd)
        {
            int dx = (int)cmd.Args[0].Accept(_exprEval),
                dy = (int)cmd.Args[1].Accept(_exprEval),
                dist = (int)cmd.Args[2].Accept(_exprEval);

             bool ok = true;
             ok &= ArgumentSpec.EnsureDirectionInRange(dx, cmd.Location, "DrawLine: dirX", _runtimeErrors);
             ok &= ArgumentSpec.EnsureDirectionInRange(dy, cmd.Location, "DrawLine: dirY", _runtimeErrors);
             ok &= ArgumentSpec.EnsurePositive(dist, cmd.Location, "DrawLine: dist", _runtimeErrors);

             if (!ok)
             {
                 // Si alguna falla, detenemos:
                 throw new PixelArtRuntimeException($"{_resmgr.GetString("Inv_DrwLine")} ¨{cmd.Location.Line}:{cmd.Location.Column}");
             }
            

            for (int i = 0; i < dist; i++)
                Stamp(CurrentX + dx * i, CurrentY + dy * i);

            CurrentX += dx * dist;
            CurrentY += dy * dist;
        }

        public void VisitDrawCircle(DrawCircleCommand cmd)
        {
            int dx = (int)cmd.Args[0].Accept(_exprEval),
                dy = (int)cmd.Args[1].Accept(_exprEval),
                radius = (int)cmd.Args[2].Accept(_exprEval);

            bool ok = true;
            ok &= ArgumentSpec.EnsureDirectionInRange(dx, cmd.Location, "DrawCircle: dirX", _runtimeErrors);
            ok &= ArgumentSpec.EnsureDirectionInRange(dy, cmd.Location, "DrawCircle: dirY", _runtimeErrors);
            ok &= ArgumentSpec.EnsurePositive(radius, cmd.Location, "DrawCircle: radius", _runtimeErrors);

            if (!ok)
            {
                throw new PixelArtRuntimeException($"{_resmgr.GetString("Inv_DrwCircle")} {cmd.Location.Line}:{cmd.Location.Column}"
);

            }

            // centro:
            int cx = CurrentX + dx * radius - dx, // mueve un pixel
                cy = CurrentY + dy * radius - dy;

            int x = 0, y = radius, d = 3 - 2 * radius;
            while (x <= y)
            {
                Stamp(cx + x, cy + y);
                Stamp(cx - x, cy + y);
                Stamp(cx + x, cy - y);
                Stamp(cx - x, cy - y);
                Stamp(cx + y, cy + x);
                Stamp(cx - y, cy + x);
                Stamp(cx + y, cy - x);
                Stamp(cx - y, cy - x);

                if (d < 0) d += 4 * x + 6;
                else { d += 4 * (x - y) + 10; y--; }
                x++;
            }
            CurrentX += dx * (radius + 1);
            CurrentY += dy * (radius + 1);

        }

        public void VisitDrawRectangle(DrawRectangleCommand cmd)
        {
            int dx = (int)cmd.Args[0].Accept(_exprEval),
                dy = (int)cmd.Args[1].Accept(_exprEval),
                dist = (int)cmd.Args[2].Accept(_exprEval),
                width = (int)cmd.Args[3].Accept(_exprEval),
                height = (int)cmd.Args[4].Accept(_exprEval);
                
            bool ok = true;
            ok &= ArgumentSpec.EnsureDirectionInRange(dx, cmd.Location, "DrawRectangle: dirX", _runtimeErrors);
            ok &= ArgumentSpec.EnsureDirectionInRange(dy, cmd.Location, "DrawRectangle: dirY", _runtimeErrors);
            ok &= ArgumentSpec.EnsurePositive(dist, cmd.Location, "DrawRectangle: distance", _runtimeErrors);
            ok &= ArgumentSpec.EnsurePositive(width, cmd.Location, "DrawRectangle: width", _runtimeErrors);
            ok &= ArgumentSpec.EnsurePositive(height, cmd.Location, "DrawRectangle: height", _runtimeErrors);

            if (!ok)
            {
                throw new PixelArtRuntimeException($"{_resmgr.GetString("Inv_DrwRectangle")} {cmd.Location.Line}:{cmd.Location.Column}");
            }

             int cx = CurrentX + dx * dist,
             cy = CurrentY + dy * dist;

            // calcula los limites
            int topLeftX = cx - width / 2;
            int topLeftY = cy - height / 2;
            int bottomRightX = topLeftX + width;
            int bottomRightY = topLeftY + height;

            // pinta el borde
            for (int x = topLeftX; x < bottomRightX; x++)
            {
                for (int y = topLeftY; y < bottomRightY; y++)
                {
                    // chequea si el pixel actyal esta en el borde
                    bool isBorder = (x == topLeftX) || (x == bottomRightX - 1) ||
                                   (y == topLeftY) || (y == bottomRightY - 1);

                    if (isBorder)
                    {
                        Stamp(x, y);
                    }
                }
            }
                CurrentX = cx;
            CurrentY = cy;
        }

        public void VisitFill(FillCommand cmd)
        {
            if (CurrentX < 0 || CurrentX >= Size || CurrentY < 0 || CurrentY >= Size)
                return;

            string target = _canvas[CurrentX, CurrentY];
            if (target == BrushCode) return;
            var q = new Queue<(int x, int y)>();
            q.Enqueue((CurrentX, CurrentY));
            while (q.Count > 0)
            {
                var (x, y) = q.Dequeue();
                if (x < 0 || y < 0 || x >= Size || y >= Size) continue;
                if (_canvas[x, y] != target) continue;
                _canvas[x, y] = BrushCode;
                q.Enqueue((x + 1, y));
                q.Enqueue((x - 1, y));
                q.Enqueue((x, y + 1));
                q.Enqueue((x, y - 1));
            }
        }
        public void VisitMove(MoveCommand cmd)
        {
            int x = (int)cmd.Args[0].Accept(_exprEval);
            int y = (int)cmd.Args[1].Accept(_exprEval);

            EnsureInBounds(x, y); 
            CurrentX = x;
            CurrentY = y;
        }

        public void VisitLabel(LabelExpression lbl) { /* no-op */ }

        public void VisitGoto(GotoCommand gt)
        {
        }

        public void VisitAssign(AssignExpression assign)
        {
            var val = assign.ValueExpr.Accept(_exprEval);
            _variables[assign.VariableName] = val;
        }
        public int GetColorCount(string colorName, int x1, int y1, int x2, int y2)
        {
            if (x1 < 0 || x1 >= Size ||
           y1 < 0 || y1 >= Size ||
           x2 < 0 || x2 >= Size ||
           y2 < 0 || y2 >= Size)
            {
                return 0;
            }

            int cnt = 0;

            int rowMin = Math.Min(x1, x2), rowMax = Math.Max(x1, x2);
            int colMin = Math.Min(y1, y2), colMax = Math.Max(y1, y2);

            for (int row = rowMin; row <= rowMax; row++)
                for (int col = colMin; col <= colMax; col++)
                    if (string.Equals(_canvas[col, row], colorName, StringComparison.OrdinalIgnoreCase))
                        cnt++;

            return cnt;
        }

        public bool CanvasColor(string colorName, int dx, int dy)
        {
            int x = CurrentX + dx, y = CurrentY + dy;
            if (x < 0 || y < 0 || x >= Size || y >= Size) return false;
            return string.Equals(_canvas[x, y], colorName, StringComparison.OrdinalIgnoreCase); // case-insensitive
        }
        private void Stamp(int x, int y)
        {
            if (BrushCode == "  ") return;
            int r = BrushSize / 2;
            for (int dy = -r; dy <= r; dy++)
                for (int dx = -r; dx <= r; dx++)
                {
                    int px = x + dx, py = y + dy;
                    if (px >= 0 && py >= 0 && px < Size && py < Size)
                        _canvas[px, py] = BrushCode;
                }
        }
        public void VisitProgram(ProgramExpression prog)
        {
            var labelToIndex = prog.LabelIndices.ToDictionary(
              kv => kv.Key,
              kv =>
              {
                  var loc = kv.Value;

                  // encuentre el primer stm tq su location es despues de la etiqueta 
                  int idx = prog.Statements.FindIndex(stmt =>
              stmt.Location.Line > loc.Line ||
              (stmt.Location.Line == loc.Line && stmt.Location.Column > loc.Column) );

                  // si la etiqueta es lo ultimo del arhivo -> termina
                  return idx < 0 ? prog.Statements.Count : idx;
              }
            );

            var stmts = prog.Statements;
            int ip = 0;
            _executionSteps = 0;

            while (ip < stmts.Count)
            {
                _executionSteps++;

                if (_executionSteps > MaxExecutionSteps)
                {
                    // error en tiempo de ejecucion
                    var loc = stmts[ip].Location;
                    ErrorHelpers.InfiniteLoopDetected(_runtimeErrors, loc);
                    throw new PixelArtRuntimeException($"{_resmgr.GetString("PotentialInfiniteLoop")} {loc.Line}:{loc.Column}");
                }

                if (stmts[ip] is GotoCommand gt
                 && gt.Condition.Accept(_exprEval) != 0)
                {
                    if (!labelToIndex.TryGetValue(gt.Label, out ip))
                    {
                        ErrorHelpers.UndefinedLabel(_runtimeErrors, gt.Location, gt.Label);
                        throw new PixelArtRuntimeException(string.Format(_resmgr.GetString("Err_LabelUndefined"), gt.Label));

                    }
                    continue;
                }

                // statement usual
                if (stmts[ip] is StatementNode stmt)
                    stmt.Accept(this);

                ip++;
            }
        }

        private void EnsureInBounds(int x, int y)
        {
            if (x < 0 || x >= Size || y < 0 || y >= Size)
            {
                throw new PixelArtRuntimeException($"{_resmgr.GetString("MovedOutsideCanvas")} {x},{y}");

            }
        }
        public string GetBrushCodeForUI(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Size || y >= Size)
                return "  ";  // Retorna transparente para out-of-bounds
            return _canvas[x, y];
        }


    }
}