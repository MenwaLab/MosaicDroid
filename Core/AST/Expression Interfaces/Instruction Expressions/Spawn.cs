public class SpawnCommand : ASTNode
    {
        public Expression X { get; }
        public Expression Y { get; }

        public SpawnCommand(Expression x, Expression y, CodeLocation loc) : base(loc)
        {
            X = x;
            Y = y;
        }

public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
{
  bool ok = true;

  // 1) Exactly one Spawn in the entire program
  if (context.SpawnSeen)
  {
    errors.Add(new CompilingError(Location, ErrorCode.Invalid,
      "Only one Spawn(x,y) allowed."));
    ok = false;
  }
  context.SpawnSeen = true;

  // 2) Two arguments must both be numbers


  var isIntX = X is Number n1 && n1.IsInt;
    var isIntY = Y is Number n2 && n2.IsInt;
    if (!isIntX || !isIntY)
    {
        errors.Add(new CompilingError(Location, ErrorCode.Invalid,
            "Spawn requires two numeric arguments."));
        return false;
    }

    X.CheckSemantic(context, scope, errors);
    Y.CheckSemantic(context, scope, errors);

    return true;
}


        public override string ToString() => $"Spawn({X}, {Y}) at {Location.Line}:{Location.Column}";
    }