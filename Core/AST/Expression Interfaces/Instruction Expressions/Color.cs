public class ColorCommand : ASTNode
    {
        public ColorLiteralExpression ColorLiteral { get; }

        public ColorCommand(ColorLiteralExpression colorLiteral, CodeLocation loc) : base(loc)
        {
            ColorLiteral = colorLiteral;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            // Already validated as color by lexer; always valid here
            return true;
        }

        public override string ToString() => $"Color({ColorLiteral.Value}) at {Location.Line}:{Location.Column}";
    }