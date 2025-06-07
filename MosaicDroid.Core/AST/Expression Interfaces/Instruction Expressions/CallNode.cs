namespace MosaicDroid.Core
{
    public abstract class CallNode : StatementNode
    {
        public string Name { get; }
        public IReadOnlyList<Expression> Args { get; }

        public CallNode(string name, IReadOnlyList<Expression> args, CodeLocation loc)
          : base(loc)
        {
            Name = name;
            Args = args;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            bool ok = true;
            var spec = ArgumentRegistry.Get(Name);
            if (spec == null)
            {
                ErrorHelpers.UnknownInstrFunc(errors, Location, Name);
                return false;
            }

            if (Args.Count != spec.ArgsCount)
            {

                ErrorHelpers.WrongArity(errors, Location, Name, spec.ArgsCount, Args.Count);
                //ok = false;
                return false;
            }

            for (int i = 0; i < Args.Count && i < spec.ExpectedTypes.Length; i++)
            {
                var expr = Args[i];
                ExpressionType actualType;

                if (expr is Number)
                    actualType = ExpressionType.Number;
                else if (expr is StringExpression || expr is ColorLiteralExpression)
                    actualType = ExpressionType.Text;
                else if (expr is VariableExpression v)
                    actualType = context.GetVariableType(v.Name);
                // function call o expresión compuesta
                else
                {
                    expr.CheckSemantic(context, scope, errors);
                    actualType = expr.Type;
                }

                if (actualType != spec.ExpectedTypes[i])
                {
                    ErrorHelpers.ArgMismatch(errors, expr.Location, Name, i + 1, spec.ExpectedTypes[i], actualType);
                    ok = false;
                }
                else
                {
                    // solo si el tipo de arg es correcto entonces sigue la recursión
                    if (!(expr is Number || expr is StringExpression || expr is ColorLiteralExpression))
                        ok &= expr.CheckSemantic(context, scope, errors);
                }
            }
            ok &= ExtraArgumentChecks(context, scope, errors);
            return ok;
        }
        protected virtual bool ExtraArgumentChecks(Context context, Scope scope, List<CompilingError> errors)
        {
            // By default, do nothing extra
            return true;
        }
        public override abstract void Accept(IStmtVisitor visitor);
    }
    public class CommandNode : CallNode
    {
        public CommandNode(string name, IReadOnlyList<Expression> args, CodeLocation loc)
          : base(name, args, loc) { }

        public override void Accept(IStmtVisitor visitor)
        {
            // no-op
        }
    }
}