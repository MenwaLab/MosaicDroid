public class FillCommand : CallNode
    {
        public FillCommand(IReadOnlyList<Expression> args, CodeLocation loc): base(TokenValues.Fill,args,loc)
        {
        }
        


        public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
        {
            if(Args.Count!=0)
            {
                errors.Add(new CompilingError(
                Location,
                ErrorCode.InvalidArgCount,
                $"Fill() expects exactly 0 arguments, but got {Args.Count}."
            ));
            return false;
            }
            return true;
        } 
        public override string ToString() => $"Fill() at {Location.Line}:{Location.Column}";
    }