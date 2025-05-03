public class Context
    {
        // Variables: name -> type (Number, Text, Boolean)
        public Dictionary<string, ExpressionType> Variables { get; } = new Dictionary<string, ExpressionType>();

        // Labels: set of declared labels
        public HashSet<string> Labels { get; } = new HashSet<string>();

        /// <summary>
        /// Declare a new variable. Reports an error if already exists.
        /// </summary>
        public void DeclareVariable(string name, ExpressionType type, CodeLocation loc, List<CompilingError> errors)
        {
            if (Variables.ContainsKey(name))
                errors.Add(new CompilingError(loc, ErrorCode.Invalid, $"Variable '{name}' is already declared."));
            else
                Variables[name] = type;
        }

        /// <summary>
        /// Retrieves the declared type of a variable, or ErrorType if undeclared.
        /// </summary>
        public ExpressionType GetVariableType(string name)
        {
            return Variables.TryGetValue(name, out var t) ? t : ExpressionType.ErrorType;
        }

        /// <summary>
        /// Declare a new label. Reports an error if already exists.
        /// </summary>
        public void DeclareLabel(string label, CodeLocation loc, List<CompilingError> errors)
        {
            if (Labels.Contains(label))
                errors.Add(new CompilingError(loc, ErrorCode.Invalid, $"Label '{label}' is already defined."));
            else
                Labels.Add(label);
        }

        /// <summary>
        /// Checks if a label has been declared.
        /// </summary>
        public bool IsLabelDeclared(string label)
        {
            return Labels.Contains(label);
        }
    }