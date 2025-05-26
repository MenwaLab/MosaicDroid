using System.Collections;

public class TokenStream : IEnumerable<Token>
{
    private readonly List<Token> tokens;
    private int position;
    public int Position => position;

    public TokenStream(IEnumerable<Token> tokens)
    {
        this.tokens = new List<Token>(tokens);
        position = 0;
    }

    public bool End => position >= tokens.Count; //y

    public void MoveNext(int k = 1) => position += k;
    public void MoveBack(int k = 1) => position -= k; //y

    public bool Next() => ++position < tokens.Count; //y
    
    public bool Next(TokenType type)
    {
        if (position < tokens.Count - 1 && LookAhead(1).Type == type)
        {
            position++;
            return true;
        }
        return false;
    }

    public bool Next(string value) //y
    {
        if (position < tokens.Count - 1 && LookAhead(1).Value == value)
        {
            position++;
            return true;
        }
        return false;
    }
    public Token Advance()
    {
        var tok = tokens[position];
        position++;
        return tok;
    }


    // Wall-E Specific Helpers: y
    public bool NextInstruction() => Next(TokenType.Instruction);
    public bool NextFunction() => Next(TokenType.Function);
    public bool NextLabel() => Next(TokenType.Label);

    public bool CanLookAhead(int k = 0) => tokens.Count - position > k;
    public Token LookAhead(int k = 0) => tokens[position + k];

    public IEnumerator<Token> GetEnumerator()
    {
        for (int i = position; i < tokens.Count; i++)
            yield return tokens[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(); //y
}