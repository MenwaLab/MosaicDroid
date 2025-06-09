namespace MosaicDroid.Core
{
    public class Compiling
    {
        private static LexicalAnalyzer? _lexicalAnalyzer;
        public static LexicalAnalyzer Lexical => _lexicalAnalyzer ??= InitializeLexicalAnalyzer();

        private static LexicalAnalyzer InitializeLexicalAnalyzer()
        {
            var analyzer = new LexicalAnalyzer();

            // Operadores Arithméticos
            analyzer.RegisterOperator("+", TokenValues.Add);
            analyzer.RegisterOperator("-", TokenValues.Sub);
            analyzer.RegisterOperator("*", TokenValues.Mul);
            analyzer.RegisterOperator("/", TokenValues.Div);
            analyzer.RegisterOperator("%", TokenValues.Mod);
            analyzer.RegisterOperator("**", TokenValues.Pow);

            // Operadores Booleanos (Registran como operadores pero se  mapean como Bool_OP después)
            analyzer.RegisterOperator("&&", TokenValues.And);
            analyzer.RegisterOperator("||", TokenValues.Or);
            analyzer.RegisterOperator("==", TokenValues.Equal);
            analyzer.RegisterOperator(">=", TokenValues.GreaterEqual);
            analyzer.RegisterOperator("<=", TokenValues.LessEqual);
            analyzer.RegisterOperator(">", TokenValues.Greater);
            analyzer.RegisterOperator("<", TokenValues.Less);

            // Assignación y Delimiters
            analyzer.RegisterOperator("<-", TokenValues.Assign); // Assignment
            analyzer.RegisterOperator("(", TokenValues.OpenParenthesis);
            analyzer.RegisterOperator(")", TokenValues.ClosedParenthesis);
            analyzer.RegisterOperator(",", TokenValues.Comma);
            analyzer.RegisterOperator("[", TokenValues.OpenBrackets);
            analyzer.RegisterOperator("]", TokenValues.ClosedBrackets);

            // Instruciones (Spawn, Color)
            analyzer.RegisterKeyword("Spawn", TokenValues.Spawn);
            analyzer.RegisterKeyword("Color", TokenValues.Color);
            analyzer.RegisterKeyword("DrawLine", TokenValues.DrawLine);
            analyzer.RegisterKeyword("DrawCircle", TokenValues.DrawCircle);
            analyzer.RegisterKeyword("DrawRectangle", TokenValues.DrawRectangle);
            analyzer.RegisterKeyword("Fill", TokenValues.Fill);
            analyzer.RegisterKeyword("GoTo", TokenValues.GoTo);
            analyzer.RegisterKeyword("Size", TokenValues.Size);
            analyzer.RegisterKeyword("Move", TokenValues.Move);

            // Funciones (GetActualX, IsBrushColor)
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
}