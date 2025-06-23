namespace MosaicDroid.Core
{
    public class SizeCommand : CallNode
    {
        public SizeCommand(IReadOnlyList<Expression> args, CodeLocation loc)
          : base(TokenValues.Size, args, loc)
        {
        }

        public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
        {
            bool ok = base.CheckSemantic(ctx, scope, errors);

            /*
            if (!ok) return false;

            // si es un numero, ajusta el valor si es un par
            if (Args[0] is Number literal && literal.IsInt)
            {
                int iv = (int)(double)literal.Value;
                if (iv % 2 == 0)
                {
                    literal.Value = iv > 1 ? iv - 1 : 1;
                }
            }

            return true;
            */
            return ok;

        }

        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitSize(this);
        }

        public override string ToString() =>
            $"Size({Args[0]}) at {Location.Line}:{Location.Column}";
    }
}