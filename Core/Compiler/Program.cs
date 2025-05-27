class Program
{
    static void Main()
    {
        var code1 = "1 + 2 * 3 ** 4 && 5 == 6 || 7 < 8";
       
        var errors1 = new List<CompilingError>();
        var lexer1 = Compiling.Lexical;
       IEnumerable<Token> tokens1 = lexer1.GetTokens(code1, errors1);
        //var tokens1 = lexer1.GetTokens(code1, errors1);
        var parser1 = new Parser(new TokenStream(tokens1), errors1);
        var expr    = new Parser(new TokenStream(tokens1), errors1).ParseExpression();
        Console.WriteLine(expr.DebugPrint());
    }
}
/* class Program
{
    static void Main()
    {
        string code = 
@" Spawn(0,0) Fill()
Color(""Black"")
DrawLine(1,0,9)
DrawCircle(1,0,9)
Size(1)
DrawRectangle(1,0, 2,3,8)
Fill()
actual_x <- GetActualX()
actual_y <- GetActualY()
n <- 5 % 2
k <- 3 - 3 / 10
n <- k * 2 ** 3
canvas_size <- GetCanvasSize()
color <- GetColorCount(""Blue"",3,7,6,8)
actual_z <- IsBrushSize(1)

actual_m <- IsCanvasColor(""Transparent"",1,9)
i <- 0

loop1
i <- i + 1
is_brush_color_blue <- IsBrushColor(""Blue"")
GoTo [loop_ends_here] (is_brush_color_blue == 1)
GoTo [loop1] (i < 10)
GoTo [loop1] (1 == 1)
loop_ends_here
";


--
        var errors = new List<CompilingError>();
      --  var lexer = Compiling.Lexical;
       -- IEnumerable<Token> tokens = lexer.GetTokens(code, errors);

        Console.WriteLine("Tokens:");
        foreach (var token in tokens)
        {
            Console.WriteLine($" {token.Type}: [{token.Value}] at {token.Location.Line}:{token.Location.Column}");
        }

        var stream = new TokenStream(tokens);
        var ctx = new Context();

        var parserErrors = new List<CompilingError>();
        var parser = new Parser(stream, parserErrors);
        var program = parser.ParseProgram();
        Scope globalScope = new Scope();

        var semanticErrors = new List<CompilingError>();
        program.CheckSemantic(ctx, globalScope, semanticErrors);
     
        // After printing tokens
        if (errors.Count > 0)
        {
            Console.WriteLine("\nLexer errors:");
            foreach (var err in errors)
                Console.WriteLine($"{err.Argument} at {err.Location.Line}:{err.Location.Column}");
        }
        if (parserErrors.Count > 0)
        {
            Console.WriteLine("\nParser errors:");
            foreach (var err in parserErrors)
                Console.WriteLine(err.Argument + " at " + err.Location.Line + ":" + err.Location.Column);
        }

        if (semanticErrors.Count > 0)
        {
            Console.WriteLine("\nSemantic errors:");
            foreach (var e in semanticErrors)
                Console.WriteLine($"{e.Argument} at {e.Location.Line}:{e.Location.Column}");
        }
        if(errors.Count == 0 && parserErrors.Count == 0 && semanticErrors.Count == 0)
        {
            Console.WriteLine("\nParsed & semantically valid AST:");
            Console.WriteLine(program);
        }
    }
}
 */