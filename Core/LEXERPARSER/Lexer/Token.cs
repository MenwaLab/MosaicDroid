public enum TokenType
{
    COMMAND, GOTO, OPERATOR, BOOL_OP, ASSIGN, INTEGER, IDENTIFIER, STRING,  DELIMETER, LABEL, COLOR, JUMPLINE
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
