public class MatrixInterpreterVisitor : IStmtVisitor
{
    private readonly char[,] _canvas;
    private readonly Dictionary<string,double> _variables
        = new Dictionary<string,double>();
    private readonly ExpressionEvaluatorVisitor _exprEval;

    public int Size   { get; }
    public int CurrentX { get; private set; }
    public int CurrentY { get; private set; }
    public char BrushChar { get; private set; } = 'w';
    public int  BrushSize { get; private set; } = 1;

    public MatrixInterpreterVisitor(int size)
    {
        Size = size;
        _canvas = new char[size,size];
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                _canvas[x,y] = 'w';

        _exprEval = new ExpressionEvaluatorVisitor(_variables, this);
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
        CurrentX = (int)cmd.Args[0].Accept(_exprEval);
        CurrentY = (int)cmd.Args[1].Accept(_exprEval);
    }

    public void VisitColor(ColorCommand cmd)
    {
        var c = cmd.ColorLiteral.Value;
        BrushChar = char.ToLower(c[0]);
    }

    public void VisitSize(SizeCommand cmd)
    {
        BrushSize = (int)cmd.Args[0].Accept(_exprEval);
    }

    public void VisitDrawLine(DrawLineCommand cmd)
    {
        int dx   = (int)cmd.Args[0].Accept(_exprEval),
            dy   = (int)cmd.Args[1].Accept(_exprEval),
            dist = (int)cmd.Args[2].Accept(_exprEval);

        // *Implement Bresenham-style line drawing here*, stamping BrushChar
        // into _canvas at each stepped pixel (and its BrushSize neighbors).
        // Then:
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

        // *Implement midpoint-circle algorithm stamping BrushChar* …
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

        // *Draw rectangle outline of size width×height at center (cx,cy)* …
        CurrentX = cx;
        CurrentY = cy;
    }

    public void VisitFill(FillCommand cmd)
    {
        // *Recursive flood-fill from (CurrentX,CurrentY), stamping BrushChar…*
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

    public void VisitAssign(AssignStatement assign)
    {
        var val = assign.Expression.Accept(_exprEval);
        _variables[assign.VariableName] = val;
    }
}