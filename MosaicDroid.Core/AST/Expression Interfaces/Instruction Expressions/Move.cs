using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaicDroid.Core
{
    public class MoveCommand : CallNode
    {
        public MoveCommand(IReadOnlyList<Expression> args, CodeLocation loc)
          : base(TokenValues.Move, args, loc)
        {
        }

        public override bool CheckSemantic(Context ctx, Scope scope, List<CompilingError> errors)
        {
            return base.CheckSemantic(ctx, scope, errors);

        }

        public override string ToString() =>
            $"Move({Args[0]}, {Args[1]}) at {Location.Line}:{Location.Column}";

        public override void Accept(IStmtVisitor visitor) =>
            visitor.VisitMove(this);
    }
}