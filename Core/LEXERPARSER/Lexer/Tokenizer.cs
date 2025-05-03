public class Token{
    public TokenType Type { get; }
    public string Value { get; }
    public CodeLocation Location { get; }

    public Token(TokenType type, string value, CodeLocation location)
    {
        Type = type;
        Value = value;
        Location = location;
    }

    public override string ToString() => $"{Type} [{Value}] at line {Location.Line}, column {Location.Column}";
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
    Operator, // Arithmetic operators: +, -, *, /, **, %
    Bool_OP, // Boolean operators: ==, >=, <=, >, <, &&, ||
    Assign, // Assignment: <-
    Integer,
    Variable,
    Function, // Functions: GetActualX, GetColorCount, etc
    String,
    Color,
    Delimeter,
    Label, // Delimiters: (, ), ,
    Jumpline
}
public class TokenValues
{
    protected TokenValues() { }

    // Instructions
    public const string Spawn = "Spawn";
    public const string Color = "Color";
    public const string DrawLine = "DrawLine";

    public const string DrawCircle = "DrawCircle";

    public const string DrawRectangle = "DrawRectangle";

    public const string Fill = "Fill";

    public const string GoTo = "GoTo";
    public const string Size = "Size";


    // Functions

    public const string GetActualX = "GetActualX";
    public const string GetActualY = "GetActualY";
    public const string GetCanvasSize = "GetCanvasSize";
    public const string GetColorCount = "GetColorCount";

    public const string IsBrushColor = "IsBrushColor";
    public const string IsBrushSize = "IsBrushSize";
    public const string IsCanvasColor = "IsCanvasColor";

    // Operators
    public const string Add = "Addition";
    public const string Sub = "Subtraction";
    public const string Mul = "Multiplication";
    public const string Div = "Division";
    public const string Mod = "Modulus";
    public const string Pow = "Power";
    public const string Assign = "Assign";

    // Boolean Operators
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
