public class FillCommand : ASTNode
    {
        public FillCommand(CodeLocation loc) : base(loc) {}

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            // No args
            return true;
        }

        public override string ToString() => $"Fill() at {Location.Line}:{Location.Column}";
    }