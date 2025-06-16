namespace MosaicDroid.Core
{
    public abstract class CompilingError
    {

        public string Message { get; }

        public CodeLocation Location { get; } 

        protected CompilingError(CodeLocation location, string message)
        {

            Location = location;
            Message = message;
        }
    }
}