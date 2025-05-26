class Program
{
    static void Main()
    {
        string code = 
@" Spawn0,0)
Color(""Black"")
DrawLine(-1,0,9)
DrawCircle(1,0,10)
Size(1)
DrawRectangle(1,0, 2,3,4)
Fill()
actual_x <- GetActualX()
actual_y <- GetActualY()
n <- 5 % 2
k <- 3 - 3 / 10
n <- k * 2 ** 3
canvas_size <- GetCanvasSize()
color <- GetColorCount(""Blue"",3,7,6,5)
actual_z <- IsBrushSize(1)
actual_m <- IsCanvasColor(""Transparent"",1,1)
i <- 0

loop1
i <- i + 1
is_brush_color_blue <- IsBrushColor(""Blue"")
GoTo [loop_ends_here] (is_brush_color_blue == 1)
GoTo [loop1] (i < 10)
GoTo [loop1] (1 == 1)
loop_ends_here
";
        var errors = new List<CompilingError>();
        var lexer = Compiling.Lexical;
        IEnumerable<Token> tokens = lexer.GetTokens(code, errors);

        Console.WriteLine("Tokens:");
foreach (var token in tokens)
{
    Console.WriteLine($" {token.Type}: [{token.Value}] at {token.Location.Line}:{token.Location.Column}");
}

        // 2) Build token stream and parse
        var stream = new TokenStream(tokens);
        var ctx = new Context();

        var parserErrors = new List<CompilingError>();
        var parser = new Parser(stream, parserErrors);
        var program = parser.ParseProgram();
        Scope globalScope = new Scope();
     
        // 3) Report semantic/parse errors
        if (parserErrors.Count > 0)
        {
            Console.WriteLine("\nParser errors:");
            foreach (var err in parserErrors)
                Console.WriteLine(err.Argument + " at " + err.Location.Line + ":" + err.Location.Column);
        }
        var semanticErrors = new List<CompilingError>();
        program.CheckSemantic(ctx, globalScope, semanticErrors);


// you’ll need to fill in or stub a global Scope if your CheckSemantic needs it

/* foreach (var stmt in program.Statements)
    stmt.CheckSemantic(ctx, globalScope, semanticErrors);
     */

// 4) report semantic errors
if (semanticErrors.Count > 0)
{
    Console.WriteLine("\nSemantic errors:");
    foreach (var e in semanticErrors)
        Console.WriteLine($"{e.Argument} at {e.Location.Line}:{e.Location.Column}");
}
else
{
            Console.WriteLine("\nParsed & semantically valid AST:");
            Console.WriteLine(program);
        }
    }
}
