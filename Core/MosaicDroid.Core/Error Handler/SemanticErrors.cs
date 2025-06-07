namespace MosaicDroid.Core
{
    public class SemanticError : CompilingError
    {
        public SemanticErrorCode Code { get; }

        public SemanticError(CodeLocation loc, SemanticErrorCode code, string message)
          : base(loc, message)
        {
            Code = code;
        }
    }

    public enum SemanticErrorCode
    {
        DuplicateSpawn,
        DuplicateLabel,
        UndefinedVariable,
        UndefinedLabel,
        ArgMismatch,
        WrongArity,
        InvalidColor,
        InvalidDirection,
        InvalidBrushSize,
        InvalidLabelSyntax,
        InvalidValue,
        InvalidGoTo,
        InvalidOperands,
        InvalidAssign,
        InvalidFunctionCall
    }
}