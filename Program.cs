class Program
{
    static void Main()
    {
        string code = @" 
Spawn(0,0)
n <- 5 % 2
k <- 3 - 3 / 10
n <- k * 2 ** 3

actual-x <- GetActualX()
i <- 0


loop-1
DrawLine(1, 0, 1)
i <- i + 1
is-brush-color-blue <- IsBrushColor(""Blue"")
GoTo [loop-ends-here] (is-brush-color-blue == 1)
GoTo [loop-1] (i < 10)

Color(""Red,Blue"")
GoTo [loop-1] (1 == 1)
loop-ends-here
";
        var errors = new List<CompilingError>();
        var lexer = Compiling.Lexical;
        IEnumerable<Token> tokens = lexer.GetTokens(code, errors);

        Console.WriteLine("Tokens:");
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }

        // 2) Build token stream and parse
        var stream = new TokenStream(tokens);

        var parserErrors = new List<CompilingError>();
        var parser = new Parser(stream, parserErrors);
        var program = parser.ParseProgram();

        // 3) Report semantic/parse errors
        if (parserErrors.Count > 0)
        {
            Console.WriteLine("\nParser errors:");
            foreach (var err in parserErrors)
                Console.WriteLine(err.Argument + " at " + err.Location.Line + ":" + err.Location.Column);
        }
        var semanticErrors = new List<CompilingError>();
var ctx = new Context();
// you’ll need to fill in or stub a global Scope if your CheckSemantic needs it
Scope globalScope = new Scope();
foreach (var stmt in program.Statements)
    stmt.CheckSemantic(ctx, globalScope, semanticErrors);

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
