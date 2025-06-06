public abstract class ASTNode
    {
        public CodeLocation Location {get; set;}
        protected ASTNode(CodeLocation location)
        {
            Location = location;
        }
        public abstract bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors);
    }