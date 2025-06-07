namespace MosaicDroid.Core
{
    public class ParseError : CompilingError
    {
        public ParseErrorCode Code { get; }

        public ParseError(CodeLocation loc, ParseErrorCode code, string message)
          : base(loc, message)
        {
            Code = code;
        }
    }

    public enum ParseErrorCode
    {
        ExpectedSpawn,
        MissingNewLine,
        MissingComma,
        MissingParenthesis,
        MissingOpenParen,
        MissingCloseParen,
        UnexpectedToken,
        InvalidVariableName,
        InvalidLabelName,
        MissingQuotation,
        UnknownInstrFunc,


    }
}