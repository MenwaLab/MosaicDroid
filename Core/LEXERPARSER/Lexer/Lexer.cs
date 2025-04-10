using System.Text.RegularExpressions;

public class Lexer
{
    private static readonly Dictionary<string, TokenType> Language = new()
    {
        // Only these are COMMANDS
        { "Spawn", TokenType.COMMAND },
        { "Color", TokenType.COMMAND },
        { "Size", TokenType.COMMAND },
        { "DrawLine", TokenType.COMMAND },
        { "DrawCircle", TokenType.COMMAND },
        { "DrawRectangle", TokenType.COMMAND },
        { "Fill", TokenType.COMMAND },
        { "GoTo", TokenType.GOTO }  // GoTo is a command-like jump
    };

    private static readonly HashSet<string> AllColors = new()
    {
        "Red", "Blue", "Green", "Yellow", "Orange",
        "Purple", "Black", "White", "Transparent"
    };

    private static readonly Regex TokenOp = new(
        @"(\r?\n)|(\d+)|(\w+)|(""[^""]*"")|(<\-)|(==|>=|<=|>|<|&&|\|\|)|(\[.*?\])|([(),])",
        RegexOptions.Compiled
    );

    public static List<Token> Tokenizer(string input)
    {
        List<Token> tokens = new();

        foreach (Match match in TokenOp.Matches(input))
        {
            string value = match.Value;

            if (Language.ContainsKey(value)) tokens.Add(new Token(Language[value], value));
            else if (Regex.IsMatch(value, @"^\r?\n$")) tokens.Add(new Token(TokenType.JUMPLINE, value));
            else if (int.TryParse(value, out _)) tokens.Add(new Token(TokenType.INTEGER, value));
            else if (Regex.IsMatch(value, @"^(\+|\-|\*|/|\*\*|%)$")) tokens.Add(new Token(TokenType.OPERATOR, value));
            else if (Regex.IsMatch(value, @"^(==|>=|<=|>|<|&&|\|\|)$")) tokens.Add(new Token(TokenType.BOOL_OP, value));
            else if (value == "<-") tokens.Add(new Token(TokenType.ASSIGN, value));
            else if (Regex.IsMatch(value, @"^[(),]$")) tokens.Add(new Token(TokenType.DELIMETER, value));
            else if (Regex.IsMatch(value, @"^\[.*\]$")) tokens.Add(new Token(TokenType.LABEL, value));
            else if (Regex.IsMatch(value, "^\"[^\"]*\"$"))
            {
                string insideString = value.Substring(1, value.Length - 2); // Remove quotes
                if (AllColors.Contains(insideString))
                    tokens.Add(new Token(TokenType.COLOR, insideString));
                else
                    tokens.Add(new Token(TokenType.STRING, value));
            }
            else tokens.Add(new Token(TokenType.IDENTIFIER, value)); // Default catch-all for functions and vars
        }

        return tokens;
    }
}
