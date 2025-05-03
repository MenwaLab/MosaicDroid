public class SizeCommand : ASTNode
    {
        public Expression SizeExpr { get; }

        public SizeCommand(Expression sizeExpr, CodeLocation loc) : base(loc)
        {
            SizeExpr = sizeExpr;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            bool ok = SizeExpr.CheckSemantic(context, scope, errors);
            if (SizeExpr.Type != ExpressionType.Number)
            {
                errors.Add(new CompilingError(Location, ErrorCode.Invalid, "Size requires numeric argument."));
                return false;
            }
            return ok;
        }

        public override string ToString() => $"Size({SizeExpr}) at {Location.Line}:{Location.Column}";
    }