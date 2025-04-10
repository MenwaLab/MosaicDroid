using System.Text.RegularExpressions;
using System.Collections.Generic;

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

    // Updated regex:
    // Group 1: newline (\r?\n)
    // Group 2: integer (\d+)
    // Group 3: identifier (allowing letters, digits, underscores and hyphens)
    // Group 4: string literal (""[^""]*"")
    // Group 5: assignment (<-)
    // Group 6: math operators (order matters: ** first)
    // Group 7: boolean operators (==, >=, etc.)
    // Group 8: label (\[.*?\])
    // Group 9: delimiters ([(),])
    private static readonly Regex TokenOp = new(
        @"(\r?\n)|(\d+)|([A-Za-z_][A-Za-z0-9_-]*)|(""[^""]*"")|(<\-)|(\*\*|\+|\-|\*|/|%)|(==|>=|<=|>|<|&&|\|\|)|(\[.*?\])|([(),])",
        RegexOptions.Compiled
    );

    public static List<Token> Tokenizer(string input)
    {
        List<Token> tokens = new();

        foreach (Match match in TokenOp.Matches(input))
        {
            string value = match.Value;

            // Check for newline (jump line). Group 1.
            if (match.Groups[1].Success)
                tokens.Add(new Token(TokenType.JUMPLINE, value));
            // Check if it's a known command word (group 3) from Language.
            else if (match.Groups[3].Success && Language.ContainsKey(value))
                tokens.Add(new Token(Language[value], value));
            // Check for integer (group 2).
            else if (match.Groups[2].Success && int.TryParse(value, out _))
                tokens.Add(new Token(TokenType.INTEGER, value));
            // Check for string literal (group 4).
            else if (match.Groups[4].Success)
            {
                string insideString = value.Substring(1, value.Length - 2); // Remove quotes
                if (AllColors.Contains(insideString))
                    tokens.Add(new Token(TokenType.COLOR, insideString));
                else
                    tokens.Add(new Token(TokenType.STRING, value));
            }
            // Check for assignment operator (group 5).
            else if (match.Groups[5].Success && value == "<-")
                tokens.Add(new Token(TokenType.ASSIGN, value));
            // Check for math operators (group 6).
            else if (match.Groups[6].Success && System.Text.RegularExpressions.Regex.IsMatch(value, @"^(\*\*|\+|\-|\*|/|%)$"))
                tokens.Add(new Token(TokenType.OPERATOR, value));
            // Check for boolean operators (group 7).
            else if (match.Groups[7].Success && System.Text.RegularExpressions.Regex.IsMatch(value, @"^(==|>=|<=|>|<|&&|\|\|)$"))
                tokens.Add(new Token(TokenType.BOOL_OP, value));
            // Check for label (group 8).
            else if (match.Groups[8].Success && System.Text.RegularExpressions.Regex.IsMatch(value, @"^\[.*\]$"))
                tokens.Add(new Token(TokenType.LABEL, value));
            // Check for delimiters (group 9).
            else if (match.Groups[9].Success && System.Text.RegularExpressions.Regex.IsMatch(value, @"^[(),]$"))
                tokens.Add(new Token(TokenType.DELIMETER, value));
            // Otherwise, treat it as an identifier.
            else
                tokens.Add(new Token(TokenType.IDENTIFIER, value));
        }

        return tokens;
    }
}
