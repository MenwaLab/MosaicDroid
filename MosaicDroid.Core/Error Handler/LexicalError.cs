namespace MosaicDroid.Core
{
    public class LexicalError : CompilingError
    {
        public LexicalErrorCode Code { get; }

        public LexicalError(CodeLocation loc, LexicalErrorCode code, string message)
          : base(loc, message)
        {
            Code = code;
        }
    }

    public enum LexicalErrorCode
    {
        UnrecognizedCharacter,
        UnterminatedString,
        InvalidIdentifier,
        InvalidInteger,
        UnterminatedText,
        UnexpectedEOI
    }
}