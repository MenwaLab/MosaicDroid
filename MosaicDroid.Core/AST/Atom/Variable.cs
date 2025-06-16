namespace MosaicDroid.Core
{
    public class VariableExpression : AtomExpression
    {
        public string Name { get; }

        public VariableExpression(string name, CodeLocation loc)
            : base(loc)
        {
            Name = name;
        }

        public override ExpressionType Type { get; set; }
        public override object? Value { get; set; }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            var varType = context.GetVariableType(Name); // consulta contexto para verificar existencia y tipo
            if (varType == ExpressionType.ErrorType)
            {
                ErrorHelpers.UndefinedVariable(errors, Location, Name); // reporta error si la variable no está declarada
                Type = ExpressionType.ErrorType;
                return false;
            }

            Type = varType;
            return true;
        }

        public override void Evaluate()
        {
            Value = 0; // despues en ejecucion se tendra el valor verdadero
        }

        public override TResult Accept<TResult>(IExprVisitor<TResult> visitor)
        {
            return visitor.VisitVariable(this);
        }
        public override string ToString() => Name;
    }
}

