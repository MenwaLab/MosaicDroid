class Program
{
    static void Main()
    {
        string code = @"
        Spawn(0, 0) 
        Color(""Black"")
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
        GoTo [loop1] (i < 10)
        
        Color(""Blue"")
        GoTo [loop1] (1 == 1) 
        loop-ends-here
        ";

        List<Token> tokens = Lexer.Tokenizer(code);
        foreach (var token in tokens)
            Console.WriteLine(token);

        Console.WriteLine("\nParsing:");
        Parser parser = new(tokens);
        var program = parser.ParseProgram(); 

        Console.WriteLine(program);
    }
}
