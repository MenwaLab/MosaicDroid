namespace MosaicDroid.Core
{
    public class DrawRectangleCommand : CallNode
    {
        public DrawRectangleCommand(IReadOnlyList<Expression> args, CodeLocation loc)
          : base(TokenValues.DrawRectangle, args, loc)
        {
        }

        protected override bool ExtraArgumentChecks(Context ctx, Scope scope, List<CompilingError> errors)
        {
            bool ok = true;
            int count = Math.Min(Args.Count, 5);

            for (int i = 0; i < count; i++)
            {
                if (Args[i] is Number literalNum && literalNum.IsInt)
                {
                    int value = (int)(double)literalNum.Value!;

                    if (i < 2)
                    {
                        // primer y segundo argumento: dirección en {-1,0,1}
                        string label = (i == 0 ? "DrawRectangle: dirX" : "DrawRectangle: dirY");
                        ok &= ArgumentSpec.EnsureDirectionInRange(value, literalNum.Location, label, errors);
                    }
                    else
                    {
                        // argumentos 2,3,4 (distance, width, height): deben ser > 0
                        string label = i switch
                        {
                            2 => "DrawRectangle: distance",
                            3 => "DrawRectangle: width",
                            4 => "DrawRectangle: height",
                            _ => "DrawRectangle: value"
                        };
                        ok &= ArgumentSpec.EnsurePositive(value, literalNum.Location, label, errors);
                    }
                }
                else if (Args[i] is Number numNode && !numNode.IsInt)
                {
                    // Si es literal Number pero no entero
                    ErrorHelpers.ArgMismatch(
                        errors,
                        numNode.Location,
                        "DrawRectangle",
                        i + 1,
                        ExpressionType.Number,  
                        ExpressionType.Number   
                    );
                    ok = false;
                }
                else
                {
                    // Si NO es literal Number, sume que es VariableExpression o expresión compuesta
                    // cuyo tipo ya se validó como Number en CheckSemantic. No chequear rangos ahora, se hara despues en el Visitor
                }
            }

            return ok;
        }

        public override void Accept(IStmtVisitor visitor)
        {
            visitor.VisitDrawRectangle(this);
        }
        public override string ToString() =>
            $"DrawRectangle({Args[0]}, {Args[1]}, {Args[2]}, {Args[3]}, {Args[4]}) at {Location.Line}:{Location.Column}";
    }
}