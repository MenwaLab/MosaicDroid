namespace MosaicDroid.Core
{
    public class DrawCircleCommand : CallNode
    {
        public DrawCircleCommand(IReadOnlyList<Expression> args, CodeLocation loc) : base(TokenValues.DrawCircle, args, loc)
        {
        }
        protected override bool ExtraArgumentChecks(Context ctx, Scope scope, List<CompilingError> errors)
        {
            bool ok = true;

            int count = Math.Min(Args.Count, 3);

            for (int i = 0; i < count; i++)
            {
                if (Args[i] is Number literalNum && literalNum.IsInt)
                {
                    int value = (int)(double)literalNum.Value!;

                    if (i < 2)
                    {
                        string label = (i == 0 ? "DrawCircle: dirX" : "DrawCircle: dirY");
                        ok &= ArgumentSpec.EnsureDirectionInRange(value, literalNum.Location, label, errors);
                    }
                    else
                    {
                        ok &= ArgumentSpec.EnsurePositive(value, literalNum.Location, "DrawCircle: distance", errors);
                    }
                }
                else if (Args[i] is Number numNode && !numNode.IsInt)
                {
                    
                    ErrorHelpers.ArgMismatch(
                        errors,
                        numNode.Location,
                        "DrawCircle",
                        i + 1,
                        ExpressionType.Number,  
                        ExpressionType.Number   
                    );
                    ok = false;
                }
                else
                {
                    
                }
            }

            return ok;
        }

        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitDrawCircle(this);
        }

        public override string ToString() =>
        $"DrawCircle({Args[0]}, {Args[1]}, {Args[2]}) at {Location.Line}:{Location.Column}";
    }
}