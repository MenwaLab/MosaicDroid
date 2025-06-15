using System.Reflection.Metadata;
using System.Resources;
using System.Threading;
using System.Windows.Interop;
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
               $"{_resmgr.GetString("UnrecognizedChar")} '{c}' "));

        public static void UnterminatedString(List<CompilingError> errs, CodeLocation loc)
          => errs.Add(new LexicalError(loc, LexicalErrorCode.UnterminatedString, _resmgr.GetString("UnterminatedString")));

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
        public static void MissingNewLine(List<CompilingError> errs,CodeLocation loc,string suffix = null)
        {
            if (!string.IsNullOrEmpty(suffix))
            errs.Add(new ParseError(loc, ParseErrorCode.MissingNewLine, $"{_resmgr.GetString("MissingNewLine")} {suffix}"));
        }


        public static void MissingCloseParen( List<CompilingError> errs,CodeLocation loc, string d
    ) => errs.Add(new ParseError(loc, ParseErrorCode.MissingParenthesis,$"Expected/Esperaba '{d}'"));

        public static void MissingOpenParen(List<CompilingError> errs,CodeLocation loc,string instructionName = null)
        {
            string baseMsg = _resmgr.GetString("MissingOpenParen");
            if (!string.IsNullOrEmpty(instructionName))
                baseMsg += $" after/después de {instructionName}";
            errs.Add(new ParseError(loc, ParseErrorCode.MissingOpenParen, baseMsg));
        }

        public static void InvalidVariableName(List<CompilingError> errs, CodeLocation loc, string variable)
        {
            var tpl = _resmgr.GetString("InvalidVariableName");
            var msg = string.Format(tpl, variable);
            errs.Add(new ParseError(loc, ParseErrorCode.InvalidVariableName, msg));
        }

        public static void InvalidLabelName(List<CompilingError> errs, CodeLocation loc, string label)
        {
            var tpl = _resmgr.GetString("InvalidLabelName");
            var msg = string.Format(tpl, label);
            errs.Add(new ParseError(loc, ParseErrorCode.InvalidLabelName, msg));
        }

        public static void UnexpectedToken(List<CompilingError> errs, CodeLocation loc, string tok)
          => errs.Add(new ParseError(loc, ParseErrorCode.UnexpectedToken, $"{_resmgr.GetString("UnexpectedToken")} {tok}"));

        public static void MissingQuotation(List<CompilingError> errs, CodeLocation loc)
          => errs.Add(new ParseError(loc, ParseErrorCode.MissingQuotation, _resmgr.GetString("MissingQuotation")));

        public static void UnknownInstrFunc(List<CompilingError> errs, CodeLocation loc, string instrFunc)
          => errs.Add(new ParseError(loc, ParseErrorCode.UnknownInstrFunc, $"{_resmgr.GetString("UnknownInstrFunc")} {instrFunc}"));


        // ===== Semantic =====
        public static void DuplicateSpawn(List<CompilingError> errs, CodeLocation loc)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.DuplicateSpawn, _resmgr.GetString("DuplicateSpawn")));

        public static void DuplicateLabel(List<CompilingError> errs, CodeLocation loc, string label)
        {
            var tpl = _resmgr.GetString("DuplicateLabel");
            var msg = string.Format(tpl, label);
            errs.Add(new SemanticError(loc, SemanticErrorCode.DuplicateLabel, msg));
        }
         

        public static void UndefinedVariable(List<CompilingError> errs, CodeLocation loc, string name)
        {
            var undef = _resmgr.GetString("InvalidOperands");
            var message = string.Format(undef, name);
            errs.Add(new SemanticError(loc, SemanticErrorCode.UndefinedVariable, message));

        }


        public static void InvalidColor(List<CompilingError> errs, CodeLocation loc, string color)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidColor, $"{_resmgr.GetString("InvalidColor")} {color}"));
        public static void InvalidGoTo(List<CompilingError> errs, CodeLocation loc)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidGoTo, _resmgr.GetString("InvalidGoTo")));

        public static void InvalidOperands(List<CompilingError> errs, CodeLocation loc, string op)
        {
            var inv = _resmgr.GetString("InvalidOperands");
            var message = string.Format(inv, op);
            errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidOperands, message));
        }

        public static void ArgMismatch(List<CompilingError> errs, CodeLocation loc, string name, int argIndex, ExpressionType type, ExpressionType actualType)
        {
            var tpl = _resmgr.GetString("ArgMismatch");
            var msg = string.Format(tpl, name, argIndex, type, actualType);
            errs.Add(new SemanticError(loc, SemanticErrorCode.ArgMismatch, msg));
        }

        public static void WrongArity(List<CompilingError> errs, CodeLocation loc, string name, int expecCount, int actCount)
        {
            var tpl = _resmgr.GetString("WrongArity");
            var msg = string.Format(tpl, name, expecCount, actCount);
            errs.Add(new SemanticError(loc, SemanticErrorCode.WrongArity, msg));
        }

        public static void InvalidValue(
            List<CompilingError> errs,
            CodeLocation loc,
            string label, int value
        ) => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidValue,
                   $"{label} < 0: {value}"));

        public static void UndefinedLabel(List<CompilingError> errs,CodeLocation loc,string label)
        {
            var tpl = _resmgr.GetString("UndefinedLabel");
            var msg = string.Format(tpl, label);
            errs.Add(new SemanticError(loc, SemanticErrorCode.UndefinedLabel, msg));
        }
        

        public static void InvalidAssign(List<CompilingError> errs, CodeLocation loc, string name)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidAssign,
               $"{_resmgr.GetString("InvalidAssign")} '{name}"));
        public static void InvalidFunctionCall(List<CompilingError> errs, CodeLocation loc)
          => errs.Add(new SemanticError(loc, SemanticErrorCode.InvalidFunctionCall, _resmgr.GetString("InvalidFunctionCall")));
       
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

        public static void InvalidDistance(List<CompilingError> errs, CodeLocation loc, int distance)
            => errs.Add(new RuntimeError(loc,
                                        RuntimeErrorCode.InvalidDistance,
                                        $"Runtime error: distance = {distance} must be > 0"));
    }
}
