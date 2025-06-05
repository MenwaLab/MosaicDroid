public abstract class CompilingError
    {
        //public ErrorCode Code { get; private set; }

       // public string Argument { get; private set; }
        public string Message    { get; }

        public CodeLocation Location {get; } //private set

        protected CompilingError(CodeLocation location, string message)//ErrorCode code, string argument)
        {
            //this.Code = code;
            //this.Argument = argument;
            Location = location;
            Message  = message;
        }
    }