public class MatrixInterpreterVisitor : IStmtVisitor
{
    private readonly string[,] _canvas;
    private readonly Dictionary<string,double> _variables
        = new Dictionary<string,double>();
    private readonly ExpressionEvaluatorVisitor _exprEval;
    private readonly List<CompilingError>      _runtimeErrors;

    public int Size   { get; }
    public int CurrentX { get; private set; }
    public int CurrentY { get; private set; }
    public string BrushCode { get; private set; } = "  ";
    public int  BrushSize { get; private set; } = 1;

    public MatrixInterpreterVisitor(int size, List<CompilingError> runtimeErrors)
    {
        Size = size;
        _runtimeErrors = runtimeErrors;

        _canvas = new string[size,size];
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                _canvas[x,y] = "w ";

        _exprEval = new ExpressionEvaluatorVisitor(_variables, this,_runtimeErrors);
    }

    public void PrintCanvas()
    {
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
                Console.Write(_canvas[x,y]);
            Console.WriteLine();
        }
    }

    public void VisitSpawn(SpawnCommand cmd)
    {
        int x = (int)cmd.Args[0].Accept(_exprEval);
        int y = (int)cmd.Args[1].Accept(_exprEval);

        if (x < 0 || x >= Size || y < 0 || y >= Size)
        {
            ErrorHelpers.OutOfBounds(_runtimeErrors, cmd.Location, x, y);
            throw new PixelArtRuntimeException(
            $"Runtime error: Spawn at ({x},{y}) is outside canvas 0..{Size-1}");
        }
        

    CurrentX = x;
    CurrentY = y;
    }

    public void VisitColor(ColorCommand cmd)
{
  
  var lit = (ColorLiteralExpression)cmd.Args[0];
    BrushCode = GetBrushCode((string)lit.Value!);
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
        int dx   = (int)cmd.Args[0].Accept(_exprEval),
            dy   = (int)cmd.Args[1].Accept(_exprEval),
            dist = (int)cmd.Args[2].Accept(_exprEval);

        for(int i = 0; i < dist; i++)
            Stamp(CurrentX + dx*i, CurrentY + dy*i);

        CurrentX += dx * dist;
        CurrentY += dy * dist;
    }

    public void VisitDrawCircle(DrawCircleCommand cmd)
    {
        int dx     = (int)cmd.Args[0].Accept(_exprEval),
            dy     = (int)cmd.Args[1].Accept(_exprEval),
            radius = (int)cmd.Args[2].Accept(_exprEval);

        // center:
        int cx = CurrentX + dx * radius,
            cy = CurrentY + dy * radius;

        int x=0, y=radius, d=3-2*radius;
        while(x<=y)
        {
            Stamp(cx + x, cy + y);
            Stamp(cx - x, cy + y);
            Stamp(cx + x, cy - y);
            Stamp(cx - x, cy - y);
            Stamp(cx + y, cy + x);
            Stamp(cx - y, cy + x);
            Stamp(cx + y, cy - x);
            Stamp(cx - y, cy - x);

            if (d < 0) d += 4*x + 6;
            else       { d += 4*(x-y) + 10; y--; }
            x++;
        }
        CurrentX = cx;
        CurrentY = cy;
    }

    public void VisitDrawRectangle(DrawRectangleCommand cmd)
    {
        int dx     = (int)cmd.Args[0].Accept(_exprEval),
            dy     = (int)cmd.Args[1].Accept(_exprEval),
            dist   = (int)cmd.Args[2].Accept(_exprEval),
            width  = (int)cmd.Args[3].Accept(_exprEval),
            height = (int)cmd.Args[4].Accept(_exprEval);

        int cx = CurrentX + dx * dist,
            cy = CurrentY + dy * dist;

        int hw = width/2,
            hh = height/2;

        for(int x = cx-hw; x <= cx+hw; x++)
        {
            Stamp(x, cy - hh);
            Stamp(x, cy + hh);
        }
        // left/right edges
        for(int y = cy-hh; y <= cy+hh; y++)
        {
            Stamp(cx - hw, y);
            Stamp(cx + hw, y);
        }
        CurrentX = cx;
        CurrentY = cy;
    }

    public void VisitFill(FillCommand cmd)
    {
         if (CurrentX < 0 || CurrentX >= Size || CurrentY < 0 || CurrentY >= Size)
            return;
            
        string target = _canvas[CurrentX,CurrentY];
        if (target == BrushCode) return;
        var q = new Queue<(int x,int y)>();
        q.Enqueue((CurrentX,CurrentY));
        while(q.Count>0)
        {
            var (x,y) = q.Dequeue();
            if (x<0||y<0||x>=Size||y>=Size) continue;
            if (_canvas[x,y]!=target) continue;
            _canvas[x,y] = BrushCode;
            q.Enqueue((x+1,y));
            q.Enqueue((x-1,y));
            q.Enqueue((x,y+1));
            q.Enqueue((x,y-1));
        }
    }

    public void VisitLabel(LabelExpression lbl) { /* no-op */ }

    public void VisitGoto(GotoCommand gt)
    {
        // You’ll need to implement a simple “instruction pointer” loop
        // over your ProgramExpression.  Many students find it easier
        // to *transform* their AST into a flat list, then run an index
        // with explicit “ip = labelIndex” jumps.  But that’s a small
        // state machine on top of what we have here.
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
        string want = GetBrushCode(colorName);
        int cnt = 0;
            int xmin = Math.Min(x1, x2), xmax = Math.Max(x1, x2);
    int ymin = Math.Min(y1, y2), ymax = Math.Max(y1, y2);

    for (int y = ymin; y <= ymax; y++)
    for (int x = xmin; x <= xmax; x++)
        if (_canvas[x,y] == want)
            cnt++;

        return cnt;
    }
    private string GetBrushCode(string colorName)
{
    if (colorName == null) return "  ";
    
    switch (colorName.ToLower())
    {
        case "black": return "bk";
        case "blue": return "bl";
        case "red": return "r ";
        case "green": return "g ";
        case "yellow": return "y ";
        case "orange": return "o ";
        case "purple": return "p ";
        case "white": return "w ";
        case "transparent": return "  ";
        default: return "??";
    }
}
public string GetBrushCodeForColor(string colorName)
{
    return GetBrushCode(colorName);
}
    public bool CanvasColor(string colorName, int dx, int dy)
    {
        string want = GetBrushCode(colorName); 
        int x = CurrentX + dx, y = CurrentY + dy;
        if (x < 0 || y < 0 || x >= Size || y >= Size) return false;
        return _canvas[x, y] == want;
    }
    private void Stamp(int x, int y)
    {
        if (BrushCode ==  "  ") return;
        int r = BrushSize/2;
        for(int dy=-r; dy<=r; dy++)
         for(int dx=-r; dx<=r; dx++)
         {
           int px = x+dx, py = y+dy;
           if (px>=0 && py>=0 && px<Size && py<Size)
             _canvas[px,py] = BrushCode;
         }
    }
    public void VisitProgram(ProgramExpression prog)
{
    // build label→statement index map: jump to the first stmt *after* the label line
    var labelToIndex = prog.LabelIndices.ToDictionary(
      kv => kv.Key,
      kv => {
        var loc = kv.Value;
        // find first statement whose location is strictly after the label token:
        int idx = prog.Statements.FindIndex(stmt =>
            stmt.Location.Line  > loc.Line ||
            (stmt.Location.Line == loc.Line && stmt.Location.Column > loc.Column)
        );
        // if label is the last thing in the file, jump past end → will terminate
        return idx < 0 ? prog.Statements.Count : idx;
      }
    );

    var stmts = prog.Statements;
    int ip = 0;

    while (ip < stmts.Count)
    {
        // 1) conditional jump:
        if (stmts[ip] is GotoCommand gt
         && gt.Condition.Accept(_exprEval) != 0)
        {
            if (!labelToIndex.TryGetValue(gt.Label, out ip))
                {
                    // Record “undefined label” as a runtime error
                    ErrorHelpers.UndefinedLabel(_runtimeErrors, gt.Location, gt.Label);
                    throw new PixelArtRuntimeException(
                        $"Runtime error: label '{gt.Label}' not declared"
                    );
                }
            continue;
        }

        // 2) normal statement:
        if (stmts[ip] is StatementNode stmt)
            stmt.Accept(this);

        ip++;
    }
}

    private void EnsureInBounds(int x, int y)
{
    if (x < 0 || x >= Size || y < 0 || y >= Size)
        throw new InvalidOperationException($"Runtime error: moved outside canvas at {x},{y}");
}


}