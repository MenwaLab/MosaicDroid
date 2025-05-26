public class LabelExpression : ASTNode
    {
        public string Name { get; }

        public LabelExpression(string name, CodeLocation location)
            : base(location)
        {
            Name = name;
        }

        public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
        {
            // The ProgramExpression will handle registering labels in the Context.
            // No additional checks here.
            return true;
        }

        public static bool IsValidLabel(string label)
{
    if (string.IsNullOrEmpty(label)) return false;
    if (char.IsDigit(label[0])) return false;
    return label.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-') && 
           !label.Contains(" ");
}

        public override string ToString() => $"Label: {Name}";
    }
    