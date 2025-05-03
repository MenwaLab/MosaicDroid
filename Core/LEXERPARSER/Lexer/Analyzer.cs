public class Compiling
{
    private static LexicalAnalyzer? _lexicalAnalyzer;
    public static LexicalAnalyzer Lexical => _lexicalAnalyzer ??= InitializeLexicalAnalyzer();

    private static LexicalAnalyzer InitializeLexicalAnalyzer()
    {
        var analyzer = new LexicalAnalyzer();

        // Arithmetic Operators
        analyzer.RegisterOperator("+", TokenValues.Add);
        analyzer.RegisterOperator("-", TokenValues.Sub);
        analyzer.RegisterOperator("*", TokenValues.Mul);
        analyzer.RegisterOperator("/", TokenValues.Div);
        analyzer.RegisterOperator("%", TokenValues.Mod);
        analyzer.RegisterOperator("**", TokenValues.Pow);

        // Boolean Operators (Register as operators but map to Bool_OP later)
        analyzer.RegisterOperator("&&", TokenValues.And);
        analyzer.RegisterOperator("||", TokenValues.Or);
        analyzer.RegisterOperator("==", TokenValues.Equal);
        analyzer.RegisterOperator(">=", TokenValues.GreaterEqual);
        analyzer.RegisterOperator("<=", TokenValues.LessEqual);
        analyzer.RegisterOperator(">", TokenValues.Greater);
        analyzer.RegisterOperator("<", TokenValues.Less);

        // Assignment and Delimiters
        analyzer.RegisterOperator("<-", TokenValues.Assign); // Assignment
        analyzer.RegisterOperator("(", TokenValues.OpenParenthesis);
        analyzer.RegisterOperator(")", TokenValues.ClosedParenthesis);
        analyzer.RegisterOperator(",", TokenValues.Comma);
        //analyzer.RegisterOperator(",",",");
        analyzer.RegisterOperator("[", TokenValues.OpenBrackets); // Label delimiters
        analyzer.RegisterOperator("]",  TokenValues.ClosedBrackets);
        //analyzer.RegisterText(" ", TokenValues.Jumpline);

        // Instructions (e.g., Spawn, Color)
        analyzer.RegisterKeyword("Spawn", TokenValues.Spawn);
        analyzer.RegisterKeyword("Color", TokenValues.Color);
        analyzer.RegisterKeyword("DrawLine", TokenValues.DrawLine);
        analyzer.RegisterKeyword("DrawCircle", TokenValues.DrawCircle);
        analyzer.RegisterKeyword("DrawRectangle", TokenValues.DrawRectangle);
        analyzer.RegisterKeyword("Fill", TokenValues.Fill);
        analyzer.RegisterKeyword("GoTo", TokenValues.GoTo);
        analyzer.RegisterKeyword("Size", TokenValues.Size);

        // Functions (e.g., GetActualX, IsBrushColor)
        analyzer.RegisterKeyword("GetActualX", TokenValues.GetActualX);
        analyzer.RegisterKeyword("GetActualY", TokenValues.GetActualY);
        analyzer.RegisterKeyword("GetCanvasSize", TokenValues.GetCanvasSize);
        analyzer.RegisterKeyword("GetColorCount", TokenValues.GetColorCount);
        analyzer.RegisterKeyword("IsBrushColor", TokenValues.IsBrushColor);
        analyzer.RegisterKeyword("IsBrushSize", TokenValues.IsBrushSize);
        analyzer.RegisterKeyword("IsCanvasColor", TokenValues.IsCanvasColor);

        analyzer.RegisterText("\"", "\""); // Strings/Colors

        return analyzer;
    }
}