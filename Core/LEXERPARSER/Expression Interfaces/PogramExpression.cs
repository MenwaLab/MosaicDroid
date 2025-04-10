using System;
using System.Collections.Generic;

public class ProgramExpression : IExpression
{
    private readonly List<IExpression> statements;

    public ProgramExpression(List<IExpression> statements)
    {
        this.statements = statements;
    }

    // The Interpret method iterates through all statements
    // and executes them sequentially.
    public int Interpret(Context context)
    {
        int result = 0;
        foreach (var stmt in statements)
        {
            result = stmt.Interpret(context);
        }
        return result;
    }

    public override string ToString()
    {
        return $"Program with {statements.Count} statement(s)";
    }
}
