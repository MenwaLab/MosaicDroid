namespace MosaicDroid.Core
{
    public abstract class StatementNode : ASTNode
    {
        protected StatementNode(CodeLocation loc) : base(loc) { }

        public abstract void Accept(IStmtVisitor visitor);
    }
}