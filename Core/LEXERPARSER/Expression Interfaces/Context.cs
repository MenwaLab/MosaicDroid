public class Context
{
    private readonly Dictionary<string, int> variables = new();

    public void SetVariable(string name, int value)
    {
        variables[name] = value;
    }

    public int GetVariable(string name)
    {
        return variables.TryGetValue(name, out int value) ? value : 0;
    }
}
