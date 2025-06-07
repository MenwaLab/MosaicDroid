namespace MosaicDroid.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "Code.pw";

            Console.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
            Console.WriteLine($"Looking for file: {Path.Combine(Directory.GetCurrentDirectory(), filePath)}");

            if (args.Length > 0) filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File not found - {filePath}");
                return;
            }
            string code = File.ReadAllText(filePath);

            var lexerErrors = new List<CompilingError>();

            var lexer = Compiling.Lexical;

            IEnumerable<Token> tokens = lexer.GetTokens(code, lexerErrors);

            Console.WriteLine("Tokens:");
            foreach (var token in tokens)
            {
                Console.WriteLine($" {token.Type}: [{token.Value}] at {token.Location.Line}:{token.Location.Column}");
            }

            // PARSER

            var stream = new TokenStream(tokens);


            var parserErrors = new List<CompilingError>();
            var parser = new Parser(stream, parserErrors);
            var program = parser.ParseProgram();

            Console.WriteLine("Declared labels:");
            foreach (var kv in program.LabelIndices)
                Console.WriteLine($"  {kv.Key}  at  {kv.Value.Line}:{kv.Value.Column}");

            // SEMANTIC ANALYSIS
            var ctx = new Context();
            Scope globalScope = new Scope();

            var semanticErrors = new List<CompilingError>();
            program.CheckSemantic(ctx, globalScope, semanticErrors);

            // ERROR REPORTING
            if (lexerErrors.Count > 0)
            {
                Console.WriteLine("\nLexer errors:");
                foreach (var err in lexerErrors)
                    Console.WriteLine($"  {err.Message} at {err.Location.Line}:{err.Location.Column}");
            }
            if (parserErrors.Count > 0)
            {
                Console.WriteLine("\nParser errors:");
                foreach (var err in parserErrors)
                    Console.WriteLine($"  {err.Message} at {err.Location.Line}:{err.Location.Column}");
            }

            if (semanticErrors.Count > 0)
            {
                Console.WriteLine("\nSemantic errors:");
                foreach (var e in semanticErrors)
                    Console.WriteLine($"  {e.Message} at {e.Location.Line}:{e.Location.Column}");
            }
            if (lexerErrors.Count > 0 || parserErrors.Count > 0 || semanticErrors.Count > 0)
                return;//??

            //if(lexerErrors.Count == 0 && parserErrors.Count == 0 && semanticErrors.Count == 0){
            Console.WriteLine("\nParsed & semantically valid AST:");
            Console.WriteLine(program);

            var runtimeErrors = new List<CompilingError>();
            var canvSize = 40;
            var interpreter = new MatrixInterpreterVisitor(canvSize, runtimeErrors);

            bool runTimeError = false;
            try
            {
                interpreter.VisitProgram(program);
            }
            catch (PixelArtRuntimeException ex)
            {
                Console.WriteLine(ex.Message);
                runTimeError = true;
            }
            interpreter.PrintCanvas();

            if (runTimeError && runtimeErrors.Count > 0)
            {
                Console.WriteLine("\nRuntime errors:");
                foreach (var e in runtimeErrors)
                    Console.WriteLine($"  {e.Message} at {e.Location.Line}:{e.Location.Column}");
                Environment.Exit(1);
            }
        }
    }
}