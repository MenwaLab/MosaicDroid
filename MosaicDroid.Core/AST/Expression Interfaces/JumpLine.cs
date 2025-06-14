namespace MosaicDroid.Core
{
    public class JumpLineExpression : ASTNode
    {
        public JumpLineExpression(CodeLocation loc) : base(loc) { }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            // siempre es valido semanticamente
            return true;
        }

        public override string ToString() => "<JumpLine>";
    }
}
