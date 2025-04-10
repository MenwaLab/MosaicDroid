public enum TokenType
{
    INSTRUCTION, GOTO, OPERATOR, BOOL_OP, ASSIGN, INTEGER, VARIABLE, FUNCTION, STRING,  DELIMETER, LABEL, COLOR, JUMPLINE
}
public class Token{
    public TokenType Type {get;}
    public string Value {get;}

    public Token(TokenType type, string value)
    {
        Type=type;
        Value=value;
    }

 

    public override string ToString() => $"{Type}: {Value}";
}
