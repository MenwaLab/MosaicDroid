namespace MosaicDroid.Core
{
    public abstract class Expression : ASTNode
    {
        public abstract void Evaluate();

        public abstract ExpressionType Type { get; set; }

        public abstract object? Value { get; set; }

        protected Expression(CodeLocation location) : base(location) { }

        public abstract TResult Accept<TResult>(IExprVisitor<TResult> visitor);

        public static explicit operator char(Expression v)
        {
            throw new NotImplementedException();
        }
    }
}