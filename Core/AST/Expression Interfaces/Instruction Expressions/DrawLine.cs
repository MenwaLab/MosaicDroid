// DrawLineCommand.cs

public class DrawLineCommand : CallNode
{
    public DrawLineCommand(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.DrawLine, args, loc)
    {
    }

/*     public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
    {
        // 1) Let the base CallNode/ArgumentRegistry enforce:
        //    - correct count (3)
        //    - ExpressionType.Number for each arg
        bool ok = base.CheckSemantic(ctx, scope, errors);
        if (!ok) return false;

        // 2) Now ensure *literal* integers (not e.g. variables or doubles).
        //    We check Number.IsInt, which your Number nodes carry.
        //if (!ArgumentSpec.EnsureAllIntegerLiterals(Args, 3, "DrawLine", errors))
          //  return false;

            var values = Args.Cast<Number>().Select(n => (int)(double)n.Value).ToArray();

        ok &= ArgumentSpec.EnsureDirectionInRange(values[0], Args[0].Location, "DrawLine: dirX", errors);
        ok &= ArgumentSpec.EnsureDirectionInRange(values[1], Args[1].Location, "DrawLine: dirY", errors);
        ok &= ArgumentSpec.EnsurePositive(values[2], Args[2].Location, "DrawLine: distance", errors);

        return ok;
        } */
        protected override bool ExtraArgumentChecks(Context ctx, Scope scope, List<CompilingError> errors)
        {
          bool ok = true;

          int count = Math.Min(Args.Count, 3);
          // Args[0] = dirX, Args[1] = dirY, Args[2] = distance
        
          for (int i = 0; i < count; i++)
          {
            // If it is literally a Number node with an integer value, check range:
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
            else if (Args[i] is Number numNode && !numNode.IsInt)
            {
                // A double‐literal is not allowed (must be an integer literal if literal):
                ErrorHelpers.ArgMismatch(
                    errors,
                    numNode.Location,
                    "DrawLine",
                    i + 1,
                    ExpressionType.Number,  // expected an integer literal
                    ExpressionType.Number   // actual is “Number but not integer literal”
                );
                ok = false;
            }
            else
            {
                // If it’s not a literal Number, it must be a VariableExpression or compound expression
                // whose type already looked up as Number.  In that case, we skip literal‐only checks.
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
