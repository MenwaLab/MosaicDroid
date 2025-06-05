public class RuntimeError : CompilingError
{
    public RuntimeErrorCode Code { get; }

    public RuntimeError(CodeLocation loc, RuntimeErrorCode code, string message)
      : base(loc, message)
    {
        Code = code;
    }
}

public enum RuntimeErrorCode
{
    OutOfBounds,
    DivisionByZero,
    ModulusByZero,
    ZeroToZeroPower,
    InfiniteLoop,
    InvalidDirection,
    InvalidDistance
}
public class PixelArtRuntimeException : Exception
{
    public PixelArtRuntimeException(string message) : base(message) { }
}
