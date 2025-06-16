namespace MosaicDroid.Core
{
    public class DrawLineCommand : CallNode
    {
        public DrawLineCommand(IReadOnlyList<Expression> args, CodeLocation loc)
          : base(TokenValues.DrawLine, args, loc)
        {
        }

        protected override bool ExtraArgumentChecks(Context ctx, Scope scope, List<CompilingError> errors)
        {
            bool ok = true;


            for (int i = 0; i < 3; i++)
            {
                // si es un literal, chequea rango
                if (Args[i] is Number literalNum && literalNum.IsInt)
                {
                    int value = (int)(double)literalNum.Value!;

                    if (i < 2)
                    {
                        string label = (i == 0 ? "DrawLine: dirX" : "DrawLine: dirY");
                        ok &= ArgumentSpec.EnsureDirectionInRange(value, literalNum.Location, label, errors);
                    }
                    else
                    {
                        ok &= ArgumentSpec.EnsurePositive(value, literalNum.Location, "DrawLine: distance", errors);
                    }
                }
            }

            return ok;
        }

        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitDrawLine(this);
        }
        public override string ToString() =>
            $"DrawLine({Args[0]}, {Args[1]}, {Args[2]}) at {Location.Line}:{Location.Column}";
    }
}