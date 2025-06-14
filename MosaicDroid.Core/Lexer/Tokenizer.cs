namespace MosaicDroid.Core
{
    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public CodeLocation Location { get; }

        public Token(TokenType type, string value, CodeLocation location)
        {
            Type = type;
            Value = value;
            Location = location;
        }
    }


    public struct CodeLocation
    {
        public int Line;
        public int Column;
    }

    public enum TokenType
    {
        Instruction, // Commandos: Spawn, Color, etc
        GoTo, 
        Operator, // Operadores aritméticos: +, -, *, /, **, %
        Bool_OP, // Operadores booleanos: ==, >=, <=, >, <, &&, ||
        Assign, // Assignación <-
        Integer,
        Variable,
        Function, // Funciones: GetActualX, GetColorCount, etc
        String,
        Color,
        Delimeter, // (,)
        Label,
        Jumpline
    }
    public class TokenValues
    {
        protected TokenValues() { }

        // Instruciones
        public const string Spawn = "Spawn";
        public const string Color = "Color";
        public const string DrawLine = "DrawLine";

        public const string DrawCircle = "DrawCircle";

        public const string DrawRectangle = "DrawRectangle";


        public const string Fill = "Fill";

        public const string GoTo = "GoTo";
        public const string Size = "Size";
        public const string Move = "Move";


        // Funciones

        public const string GetActualX = "GetActualX";
        public const string GetActualY = "GetActualY";
        public const string GetCanvasSize = "GetCanvasSize";
        public const string GetColorCount = "GetColorCount";

        public const string IsBrushColor = "IsBrushColor";
        public const string IsBrushSize = "IsBrushSize";
        public const string IsCanvasColor = "IsCanvasColor";

        // Operadores aritméticos
        public const string Add = "+";
        public const string Sub = "-";
        public const string Mul = "*";
        public const string Div = "/";
        public const string Mod = "%";
        public const string Pow = "**";
        public const string Assign = "<-";

        // Operadores booleanos
        public const string And = "And";
        public const string Or = "Or";
        public const string Equal = "Equal";
        public const string GreaterEqual = "GreaterEqual";
        public const string LessEqual = "LessEqual";
        public const string Greater = "Greater";
        public const string Less = "Less";

        // Delimiters
        public const string OpenParenthesis = "OpenParenthesis";
        public const string ClosedParenthesis = "ClosedParenthesis";
        public const string OpenBrackets = "OpenBrackets";
        public const string ClosedBrackets = "ClosedBrackets";
        public const string Comma = "Comma";
        public const string Jumpline = "Jumpline";
    }
}