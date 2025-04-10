class Program
{
    static void Main()
    {
        string code = @"
        Spawn(0, 0) 
        n <- 5 
        k <- 3 + 3 * 10 
        n <- k * 2 
        actual-x <- GetActualX() 
        i <- 0 
        
        loop-1 
        DrawLine(1, 0, 1) 
        i <- i + 1 
        GoTo [loop1] (i < 10)
        
        GoTo [loop1] (1 == 1) 
        loop-ends-here
        ";

        List<Token> tokens = Lexer.Tokenizer(code);
        foreach (var token in tokens)
            Console.WriteLine(token);

        Console.WriteLine("\nParsing:");
        Parser parser = new(tokens);
        var program = parser.ParseProgram();  // ✅ Use correct method

        Console.WriteLine(program);
    }
}
