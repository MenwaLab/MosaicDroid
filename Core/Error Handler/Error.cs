/* public abstract class CompilingError
{
    public CodeLocation Location { get; }
    public string Message    { get; }

    protected CompilingError(CodeLocation loc, string message)
    {
        Location = loc;
        Message  = message;
    }
}
 */

 // File: Errors/ErrorHelpers.cs
public static class ErrorHelpers
{
    // ===== Lexical =====
    public static void UnrecognizedChar(List<CompilingError> errs, CodeLocation loc, char c)
      => errs.Add(new LexicalError(loc, LexicalErrorCode.UnrecognizedCharacter,
           $"Unrecognized character '{c}'"));

    public static void UnterminatedString(List<CompilingError> errs, CodeLocation loc)
      => errs.Add(new LexicalError(loc, LexicalErrorCode.UnterminatedString,
           "Unterminated string literal"));
     public static void UnterminatedText(List<CompilingError> errs, CodeLocation loc, string closingDelimiter)
         => errs.Add(new LexicalError(loc, LexicalErrorCode.UnterminatedText,
                    $"Unterminated text literal; expected closing '{closingDelimiter}'"));


           
    public static void InvalidIdentifier(List<CompilingError> errs, CodeLocation loc, string id)
      => errs.Add(new LexicalError(loc, LexicalErrorCode.InvalidIdentifier,
           $"Sorry, please fix the invalid identifier '{id}'"));

    public static void InvalidInteger(List<CompilingError> errs, CodeLocation loc,string number)
      => errs.Add(new LexicalError(loc, LexicalErrorCode.InvalidInteger,
           $"Sorry, please fix the invalid integer {number}"));

    // ===== Parse =====


  public static void ExpectedSpawn(List<CompilingError> errs, CodeLocation loc)
      => errs.Add(new ParseError(loc, ParseErrorCode.ExpectedSpawn,
           "Please make sure your code starts with a Spawn(x,y)"));
  public static void MissingNewLine(
        List<CompilingError> errs,
        CodeLocation loc,
        string suffix = null
    )
    {
        string baseMsg = "Missing newline";
        if (!string.IsNullOrEmpty(suffix))
            baseMsg += $" after {suffix}";
        errs.Add(new ParseError(loc, ParseErrorCode.MissingNewLine, baseMsg));
    }


    public static void MissingCloseParen(
    List<CompilingError> errs,
    CodeLocation loc,
    string d
) => errs.Add(new ParseError(loc, ParseErrorCode.MissingParenthesis,
           $"Expected '{d}'"));


  

    public static void MissingOpenParen(
        List<CompilingError> errs,
        CodeLocation loc,
        string instructionName = null
    )
    {
        string baseMsg = "Sorry, theres a missing open parentesis";
        if (!string.IsNullOrEmpty(instructionName))
            baseMsg += $" after {instructionName}";
        errs.Add(new ParseError(loc, ParseErrorCode.MissingOpenParen, baseMsg));
    }
    public static void InvalidVariableName(List<CompilingError> errs, CodeLocation loc, string variable)
      => errs.Add(new ParseError(loc, ParseErrorCode.InvalidVariableName,
           $"Sorry, please make sure the variable name {variable} is valid according to the grammar rules"));
    
    public static void InvalidLabelName(List<CompilingError> errs, CodeLocation loc, string label)
      => errs.Add(new ParseError(loc, ParseErrorCode.InvalidLabelName,
           $"Sorry, please make sure the label name {label} is valid according to the grammar rules"));

    public static void UnexpectedToken(List<CompilingError> errs, CodeLocation loc, string tok)
      => errs.Add(new ParseError(loc, ParseErrorCode.UnexpectedToken,
           $"Unexpected token '{tok}'"));

    public static void MissingQuotation(List<CompilingError> errs, CodeLocation loc)
      => errs.Add(new ParseError(loc, ParseErrorCode.MissingQuotation,
           "Please make sure your color is inside quotations"));

    public static void UnknownInstrFunc(List<CompilingError> errs, CodeLocation loc, string instrFunc)
      => errs.Add(new ParseError(loc, ParseErrorCode.UnknownInstrFunc,
           $"Unknown Instruction or Function'{instrFunc}'"));

           

    // ===== Semantic =====
    public static void DuplicateSpawn(List<CompilingError> errs, CodeLocation loc)
      => errs.Add(new SemanticError(loc, SemanticErrorCode.DuplicateSpawn,
           "Only one Spawn(x,y) allowed"));

           
    public static void DuplicateLabel(List<CompilingError> errs, CodeLocation loc, string label)
      => errs.Add(new SemanticError(loc, SemanticErrorCode.DuplicateLabel,
           $"Sorry the label {label} can only be declared once."));

    public static void UndefinedVariable(List<CompilingError> errs, CodeLocation loc, string name)
      => errs.Add(new SemanticError(loc, SemanticErrorCode.UndefinedVariable,
           $"Variable '{name}' is not declared"));

    public static void InvalidColor(List<CompilingError> errs, CodeLocation loc, string color)
      => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidColor,
           $"Sorry, unknown color detected: '{color}'"));
    public static void InvalidGoTo(List<CompilingError> errs, CodeLocation loc)
      => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidGoTo,
           $"Sorry, please make sure the condition for the GoTo is boolean"));

    public static void InvalidOperands(List<CompilingError> errs, CodeLocation loc, string op)
      => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidOperands,
           $"Sorry, both operands for the operation {op} need to be both integers or both be text (for boolean operations)"));

    public static void ArgMismatch(List<CompilingError> errs, CodeLocation loc, string name, int argIndex, ExpressionType type, ExpressionType actualType)
      => errs.Add(new SemanticError(loc, SemanticErrorCode.ArgMismatch,
           $"Sorry, for {name} we expect argument {argIndex} to be of type {type} but got {actualType}"));

    public static void InvalidDirection(List<CompilingError> errs, CodeLocation loc, int dx, int dy)
      => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidDirection,
           $"Invalid direction ({dx},{dy})"));

           
    public static void WrongArity(List<CompilingError> errs, CodeLocation loc, string name, int expecCount, int actCount)
      => errs.Add(new SemanticError(loc, SemanticErrorCode.WrongArity,
           $"Sorry, for {name} we expect {expecCount} arguments but got {actCount}"));

    public static void InvalidValue(
        List<CompilingError> errs,
        CodeLocation loc,
        string message
    ) => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidValue,
               message));

    public static void UndefinedLabel(
        List<CompilingError> errs,
        CodeLocation loc,
        string label
    ) => errs.Add(new SemanticError(loc, SemanticErrorCode.UndefinedLabel,
               $"Sorry, label '{label}' not declared"));

    public static void InvalidAssign(List<CompilingError> errs, CodeLocation loc, string name)
      => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidAssign,
           $"Cannot assign invalid expression to '{name}"));
    public static void InvalidFunctionCall(List<CompilingError> errs, CodeLocation loc)
      => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidFunctionCall,
           "Function calls cannot be nested as arguments"));
    // ===== Runtime =====
    public static void DivisionByZero(List<CompilingError> errs, CodeLocation loc)
      => errs.Add(new RuntimeError(loc, RuntimeErrorCode.DivisionByZero,
           "Division by zero"));

    public static void ModulusByZero(
        List<CompilingError> errs,
        CodeLocation loc
    ) => errs.Add(new RuntimeError(loc, RuntimeErrorCode.ModulusByZero,
               "Modulus by zero"));

    public static void ZeroToZeroPower(
        List<CompilingError> errs,
        CodeLocation loc
    ) => errs.Add(new RuntimeError(loc, RuntimeErrorCode.ZeroToZeroPower,
               "0^0 is undefined"));
    
    public static void OutOfBounds(List<CompilingError> errs, CodeLocation loc, int x, int y)
      => errs.Add(new RuntimeError(loc, RuntimeErrorCode.OutOfBounds,
           $"Position ({x},{y}) is outside canvas"));

    public static void InfiniteLoopDetected(
        List<CompilingError> errs,
        CodeLocation loc
    ) => errs.Add(new RuntimeError(loc, RuntimeErrorCode.InfiniteLoop,
               "Potential infinite loop"));
}
