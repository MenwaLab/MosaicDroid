using System.Resources;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MosaicDroid.Core
{
    public static class ErrorHelpers
    {
        private static readonly ResourceManager _resmgr =
    new ResourceManager("MosaicDroid.Core.Resources.Strings", typeof(MatrixInterpreterVisitor).Assembly);

        // ===== Lexical =====
        public static void UnrecognizedChar(List<CompilingError> errs, CodeLocation loc, char c)
          => errs.Add(new LexicalError(loc, LexicalErrorCode.UnrecognizedCharacter,
               $"{_resmgr.GetString("UnrecognizedChar")} '{c}'"));

        public static void UnterminatedString(List<CompilingError> errs, CodeLocation loc)
          => errs.Add(new LexicalError(loc, LexicalErrorCode.UnterminatedString, _resmgr.GetString("UnterminatedString")));
            //   ""));
        public static void UnterminatedText(List<CompilingError> errs, CodeLocation loc, string closingDelimiter)
            => errs.Add(new LexicalError(loc, LexicalErrorCode.UnterminatedText,
                       $" {_resmgr.GetString("UnterminatedText")} '{closingDelimiter}'"));



        public static void InvalidIdentifier(List<CompilingError> errs, CodeLocation loc, string id)
          => errs.Add(new LexicalError(loc, LexicalErrorCode.InvalidIdentifier,
               $"{_resmgr.GetString("InvalidId")} '{id}'"));

        public static void InvalidInteger(List<CompilingError> errs, CodeLocation loc, string number)
          => errs.Add(new LexicalError(loc, LexicalErrorCode.InvalidInteger,
               $"{_resmgr.GetString("InvalidInt")} {number}"));

        // ===== Parse =====


        public static void ExpectedSpawn(List<CompilingError> errs, CodeLocation loc)
            => errs.Add(new ParseError(loc, ParseErrorCode.ExpectedSpawn,
                 _resmgr.GetString("ExpectedSpawn")));
        public static void MissingNewLine(
              List<CompilingError> errs,
              CodeLocation loc,
              string suffix = null
          )
        {
            string baseMsg = _resmgr.GetString("MissingnewLine");
            if (!string.IsNullOrEmpty(suffix))
                baseMsg += $" after/después de {suffix}";
            errs.Add(new ParseError(loc, ParseErrorCode.MissingNewLine, baseMsg));
        }


        public static void MissingCloseParen(
        List<CompilingError> errs,
        CodeLocation loc,
        string d
    ) => errs.Add(new ParseError(loc, ParseErrorCode.MissingParenthesis,
               $"Expected/Esperaba '{d}'"));




        public static void MissingOpenParen(
            List<CompilingError> errs,
            CodeLocation loc,
            string instructionName = null
        )
        {
            string baseMsg = _resmgr.GetString("MissingOpenParen");
            if (!string.IsNullOrEmpty(instructionName))
                baseMsg += $" after/después de {instructionName}";
            errs.Add(new ParseError(loc, ParseErrorCode.MissingOpenParen, baseMsg));
        }

        public static void InvalidVariableName(List<CompilingError> errs, CodeLocation loc, string variable)
          => errs.Add(new ParseError(loc, ParseErrorCode.InvalidVariableName, $"{_resmgr.GetString("InvalidVariableName")} {variable}"));
              // $"Sorry, please make sure the variable name {variable} is valid according to the grammar rules"));


        public static void InvalidLabelName(List<CompilingError> errs, CodeLocation loc, string label)
          => errs.Add(new ParseError(loc, ParseErrorCode.InvalidLabelName, $"{_resmgr.GetString("InvalidLabelName")} {label}"));
              // $"Sorry, please make sure the label name {label} is valid according to the grammar rules"));

        public static void UnexpectedToken(List<CompilingError> errs, CodeLocation loc, string tok)
          => errs.Add(new ParseError(loc, ParseErrorCode.UnexpectedToken, $"{_resmgr.GetString("UnexpectedToken")} {tok}"));
        //$"Unexpected token '{tok}'"));

        public static void MissingQuotation(List<CompilingError> errs, CodeLocation loc)
          => errs.Add(new ParseError(loc, ParseErrorCode.MissingQuotation, _resmgr.GetString("MissingQuotation")));
             //  "Please make sure your color is inside quotations"));

        public static void UnknownInstrFunc(List<CompilingError> errs, CodeLocation loc, string instrFunc)
          => errs.Add(new ParseError(loc, ParseErrorCode.UnknownInstrFunc, $"{_resmgr.GetString("UnknownInstrFunc")} {instrFunc}"));
              // $"Unknown Instruction or Function'{instrFunc}'"));



        // ===== Semantic =====
        public static void DuplicateSpawn(List<CompilingError> errs, CodeLocation loc)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.DuplicateSpawn, _resmgr.GetString("DuplicateSpawn")));


        public static void DuplicateLabel(List<CompilingError> errs, CodeLocation loc, string label)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.DuplicateLabel, $"{_resmgr.GetString("DuplicateLabel")} {label}"));
               //$"Sorry the label {label} can only be declared once."));

        public static void UndefinedVariable(List<CompilingError> errs, CodeLocation loc, string name)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.UndefinedVariable, $"{_resmgr.GetString("UndefinedVariableSemantic")} {name}"));
               //$"Variable '{name}' is not declared"));

        public static void InvalidColor(List<CompilingError> errs, CodeLocation loc, string color)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidColor, $"{_resmgr.GetString("InvalidColor")} {color}"));
        // $"Sorry, unknown color detected: '{color}'"));
        public static void InvalidGoTo(List<CompilingError> errs, CodeLocation loc)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidGoTo, _resmgr.GetString("InvalidGoTo")));
              // $"Sorry, please make sure the condition for the GoTo is boolean"));

        public static void InvalidOperands(List<CompilingError> errs, CodeLocation loc, string op)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidOperands, $"{_resmgr.GetString("InvalidOperands")} {op}"));
              // $"Sorry, both operands for the operation {op} need to be both integers or both be text (for boolean operations)"));

        public static void ArgMismatch(List<CompilingError> errs, CodeLocation loc, string name, int argIndex, ExpressionType type, ExpressionType actualType)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.ArgMismatch, $"{_resmgr.GetString("ArgMismatch")} {name} {argIndex} {type} {actualType}"));
               //$"Sorry, for {name} we expect argument {argIndex} to be of type {type} but got {actualType}"));




        public static void WrongArity(List<CompilingError> errs, CodeLocation loc, string name, int expecCount, int actCount)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.WrongArity, $"{_resmgr.GetString("WrongArity")} {name} {expecCount} {actCount}"));
              // $"Sorry, for {name} we expect {expecCount} arguments but got {actCount}"));

        public static void InvalidValue(
            List<CompilingError> errs,
            CodeLocation loc,
            string label, int value
        ) => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidValue,
                   $"{label} < 0: {value}"));

        public static void UndefinedLabel(
            List<CompilingError> errs,
            CodeLocation loc,
            string label
        ) => errs.Add(new SemanticError(loc, SemanticErrorCode.UndefinedLabel, $"{_resmgr.GetString("UndefinedLabel")} {label}"));
                  // $"Sorry, label '{label}' not declared"));

        public static void InvalidAssign(List<CompilingError> errs, CodeLocation loc, string name)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidAssign,
               $"{_resmgr.GetString("InvalidAssign")} '{name}"));
        public static void InvalidFunctionCall(List<CompilingError> errs, CodeLocation loc)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidFunctionCall, _resmgr.GetString("InvalidFunctionCall")));
               //"Function calls cannot be nested as arguments"));
       
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

        public static void InvalidDirection(List<CompilingError> errs, CodeLocation loc, string which, int value)
            => errs.Add(new RuntimeError(loc,
                                        RuntimeErrorCode.InvalidDirection,
                                        $"Runtime error: {which} value = {value} is not in [-1..1]"));

        public static void InvalidDirection(List<CompilingError> errs, CodeLocation loc, int dx, int dy)
  => errs.Add(new RuntimeError(loc, RuntimeErrorCode.InvalidDirection,
       $"Invalid direction ({dx},{dy})"));

        // 2) “distance” debe ser > 0:
        public static void InvalidDistance(List<CompilingError> errs, CodeLocation loc, int distance)
            => errs.Add(new RuntimeError(loc,
                                        RuntimeErrorCode.InvalidDistance,
                                        $"Runtime error: distance = {distance} must be > 0"));
    }
}
