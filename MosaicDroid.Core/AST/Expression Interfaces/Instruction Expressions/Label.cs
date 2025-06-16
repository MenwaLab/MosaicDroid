namespace MosaicDroid.Core
{
    public class LabelExpression : StatementNode
    {
        public string Name { get; }

        public LabelExpression(string name, CodeLocation location)
            : base(location)
        {
            Name = name;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            // ProgramExpression se encarga de registrar la etiqeta en el Context.
            // No requiere de chequeos adicionals aqui
            return true;
        }

        public static bool IsValidLabel(string label)
        {
            if (string.IsNullOrEmpty(label)) return false;
            if (char.IsDigit(label[0])) return false;
            return label.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-') &&
                   !label.Contains(" ");
        }

        public override void Accept(IStmtVisitor visitor)
              => visitor.VisitLabel(this);
        public override string ToString() => $"Label: {Name}";
    }
}
    