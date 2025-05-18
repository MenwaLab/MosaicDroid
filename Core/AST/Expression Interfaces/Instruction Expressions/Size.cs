// SizeCommand.cs

public class SizeCommand : CallNode
{
    public SizeCommand(IReadOnlyList<Expression> args, CodeLocation loc)
      : base(TokenValues.Size, args, loc)
    {
    }

    public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
    {
        bool ok = base.CheckSemantic(ctx, scope, errors);
        if (!ok) return false;

        // 3) If it's a literal Number, adjust even values
        if (Args[0] is Number literal && literal.IsInt)
        {
            int iv = (int)(double)literal.Value;
                if (iv % 2 == 0)
                {
                    literal.Value = iv > 1 ? iv - 1 : 1;
                }
            }
            return true;
            
        }

        
    

    public override string ToString() =>
        $"Size({Args[0]}) at {Location.Line}:{Location.Column}";
}