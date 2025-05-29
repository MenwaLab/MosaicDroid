public class GotoCommand : StatementNode
    {
        public string Label { get; }
        public Expression Condition { get; }


        public GotoCommand(string label, Expression condition, CodeLocation loc)
            : base(loc)
        {
            Label     = label;
            Condition = condition;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            if (!LabelExpression.IsValidLabel(Label))
    {
        errors.Add(new CompilingError(Location, ErrorCode.Invalid,
            $"Invalid label format '{Label}'"));
        return false;
    }
    if (!context.IsLabelDeclared(Label))
            {
                errors.Add(new CompilingError(Location, ErrorCode.Invalid,
                    $"Label '{Label}' not declared."));
                //okCond = false;
                return false;
            }
            // 1) Ensure the condition is boolean
            bool okCond = Condition.CheckSemantic(context, scope, errors);
            if (Condition.Type != ExpressionType.Boolean)
            {
                errors.Add(new CompilingError(Location, ErrorCode.Invalid,
                    "GoTo condition must be boolean."));
                okCond = false;
            }

            // 2) Ensure the label has been declared somewhere
            

            return okCond;
        }

public override void Accept(IStmtVisitor visitor)
      => visitor.VisitGoto(this);
        public override string ToString() =>
            $"GoTo [{Label}] ({Condition}) at {Location.Line}:{Location.Column}";
    }