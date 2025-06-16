namespace MosaicDroid.Core
{
    public class Context
    {
        public Dictionary<string, ExpressionType> Variables { get; } = new Dictionary<string, ExpressionType>();
        public HashSet<string> Labels { get; } = new HashSet<string>(); 
        public bool SpawnSeen { get; set; } = false; 

        public ExpressionType GetVariableType(string name)
        {
            return Variables.TryGetValue(name, out var t) ? t : ExpressionType.ErrorType;
        }
        public void SetVariableType(string name, ExpressionType type)
        {
            Variables[name] = type;
        }

        public void DeclareLabel(string label, CodeLocation loc, List<CompilingError> errors)
        {
            if (IsLabelDeclared(label))
                ErrorHelpers.DuplicateLabel(errors, loc, label);
            else
                Labels.Add(label);
        }

        public bool IsLabelDeclared(string label)
        {
            return Labels.Contains(label);
        }
    }
}