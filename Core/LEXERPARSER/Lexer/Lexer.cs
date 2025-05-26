public class LexicalAnalyzer
{
    private readonly Dictionary<string, string> operators = new();
    private readonly Dictionary<string, string> keywords = new();
    private readonly Dictionary<string, string> texts = new();

    public void RegisterOperator(string op, string tokenValue) => operators[op] = tokenValue;
    public void RegisterKeyword(string keyword, string tokenValue) => keywords[keyword] = tokenValue;
    public void RegisterText(string start, string end) => texts[start] = end;

    public IEnumerable<Token> GetTokens(string code, List<CompilingError> errors)
    {
        var tokens = new List<Token>();
        var stream = new TokenReader(code);

        while (!stream.EOF)
        {
            if (stream.Peek() == '\n')
    {
        tokens.Add(new Token(TokenType.Jumpline, TokenValues.Jumpline, stream.Location));
        stream.ReadAny();
        continue;
    }
            if (stream.ReadWhiteSpace()) continue;

            // Handle Labels (e.g., [loop1])
            if (stream.Match("["))
            {
                tokens.Add(new Token(TokenType.Delimeter, TokenValues.OpenBrackets, stream.Location));
                if (stream.ReadID(out string label))
                    tokens.Add(new Token(TokenType.Label, label, stream.Location));
                if (stream.Match("]"))
                    tokens.Add(new Token(TokenType.Delimeter, TokenValues.ClosedBrackets, stream.Location));
                continue;
            }

            // Match Identifiers (Instructions, Functions, or Variables)
            if (stream.ReadID(out string id))
            {
                if (keywords.TryGetValue(id, out string? tokenValue))
                {
                    // Determine TokenType based on TokenValue
                    TokenType type = tokenValue switch
                    {
                        TokenValues.Spawn or TokenValues.Color or TokenValues.Size or TokenValues.DrawLine
                            or TokenValues.DrawCircle or TokenValues.DrawRectangle or TokenValues.Fill
                            or TokenValues.GoTo => TokenType.Instruction,
                        TokenValues.GetActualX or TokenValues.GetActualY or TokenValues.GetCanvasSize
                            or TokenValues.GetColorCount or TokenValues.IsBrushColor
                            or TokenValues.IsBrushSize or TokenValues.IsCanvasColor => TokenType.Function,
                        _ => TokenType.Variable // Fallback (should not happen)
                    };
                    tokens.Add(new Token(type, tokenValue, stream.Location));
                }
                else
                {
                    char nextChar = stream.CanLookAhead(1) ? stream.Peek(1) : '\0';
        if (nextChar == '\n' || nextChar == '\r' || nextChar == '\0')
        {
            tokens.Add(new Token(TokenType.Label, id, stream.Location));
        }
                    else if (IsValidIdentifier(id))
                        tokens.Add(new Token(TokenType.Variable, id, stream.Location));
                    else
                        errors.Add(new CompilingError(stream.Location, ErrorCode.Invalid, $"Invalid identifier: {id}"));
                }
                continue;
            }

            // Match Numbers (Integers)
            if (stream.ReadNumber(out string number))
            {
                if (!int.TryParse(number, out _))
                    errors.Add(new CompilingError(stream.Location, ErrorCode.Invalid, "Invalid integer"));
                tokens.Add(new Token(TokenType.Integer, number, stream.Location));
                continue;
            }
            

            // Match Strings/Colors
            if (MatchText(stream, tokens, errors)) continue;

            // Match Symbols (Operators, Delimiters, Assignments)
            if (MatchSymbol(stream, tokens)) continue;

            // Handle Unknown Characters
            var unknownChar = stream.ReadAny();
            errors.Add(new CompilingError(stream.Location, ErrorCode.Unknown, unknownChar.ToString()));
        }

        return tokens;
    }

    private bool MatchSymbol(TokenReader stream, List<Token> tokens)
    {
        foreach (var op in operators.Keys.OrderByDescending(k => k.Length))
        {
            if (!stream.Match(op)) continue;
            var val = operators[op];
            
            // Determine TokenType based on operator
            TokenType type = val switch
            {
                
                TokenValues.And or TokenValues.Or or TokenValues.Equal 
                    or TokenValues.GreaterEqual or TokenValues.LessEqual 
                    or TokenValues.Greater or TokenValues.Less => TokenType.Bool_OP,
                TokenValues.Assign => TokenType.Assign,
                TokenValues.OpenParenthesis or TokenValues.ClosedParenthesis or TokenValues.Comma 
                    => TokenType.Delimeter,
                _ => TokenType.Operator
            };
            //tokens.Add(new Token(type, operators[op], stream.Location));
            tokens.Add(new Token(type, val, stream.Location));
            return true;
        }
        return false;
    }

private bool MatchText(TokenReader stream, List<Token> tokens, List<CompilingError> errors)
{
    if (stream.Peek() == '"')
    {
        var startLoc = stream.Location;
        stream.ReadAny(); // consume opening "
        if (!stream.ReadUntil("\"", out string text))
        {
            errors.Add(new CompilingError(
                stream.Location,
                ErrorCode.Expected,
                "Closing quote for string literal"));
            return false;
        }
        
        // exact, single‐member match (case‐insensitive)
        var names = Enum.GetNames(typeof(ColorOptions));
        bool isExactColor = names
            .Any(n => string.Equals(n, text, StringComparison.OrdinalIgnoreCase));

        var type = isExactColor
            ? TokenType.Color
            : TokenType.String;

        tokens.Add(new Token(type, text, startLoc));
        return true;
    }

    foreach (var start in texts.Keys.OrderByDescending(k => k.Length))
    {
        if (!stream.Match(start)) continue;
        if (!stream.ReadUntil(texts[start], out string text))
            errors.Add(new CompilingError(stream.Location, ErrorCode.Expected, texts[start]));
        TokenType type = IsColor(text) ? TokenType.Color : TokenType.String;
        tokens.Add(new Token(type, text, stream.Location));
        return true;
    }
    return false;
}
    private static bool IsColor(string text) =>
    Enum.TryParse<ColorOptions>(text, true, out _);


    private static bool IsValidIdentifier(string id) => !string.IsNullOrEmpty(id) && 
    (char.IsLetter(id[0]) || id[0] == '_') && 
    id.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-');
        //!char.IsDigit(id[0]) && id[0] != '-' && !id.Contains(" ");
}
class TokenReader
        {
            string code;
            int pos;
            int line;
            int lastLB;

            public TokenReader(string code)
            {
                this.code = code;
                this.pos = 0;
                this.line = 1;
                this.lastLB = -1;
            }

            public CodeLocation Location
            {
                get
                {
                    return new CodeLocation
                    {
                        Line = line,
                        Column = pos - lastLB
                    };
                }
            }

            /* Peek the next character */
            public char Peek()
            {
                if (pos < 0 || pos >= code.Length)
                    throw new InvalidOperationException();

                return code[pos];
            }
            

            public bool EOF
            {
                get { return pos >= code.Length; }
            }

            public bool EOL
            {
                get { return EOF || code[pos] == '\n'; }
            }

            public bool ContinuesWith(string prefix)
            {
                if (pos + prefix.Length > code.Length)
                    return false;
                for (int i = 0; i < prefix.Length; i++)
                    if (code[pos + i] != prefix[i])
                        return false;
                return true;
            }

            public bool Match(string prefix)
            {
                if (ContinuesWith(prefix))
                {
                    pos += prefix.Length;
                    return true;
                }

                return false;
            }

            public bool ValidIdCharacter(char c, bool beginning)
{
    if (beginning)
        return char.IsLetter(c);

    return char.IsLetterOrDigit(c) || c == '-' || c == '_';
    return c == '_' 
        || (!beginning && c == '-')  // allow hyphens after the first character
        || (beginning ? char.IsLetter(c) : char.IsLetterOrDigit(c));
}


            public bool ReadID(out string id)
            {
                id = "";
                while (!EOL && ValidIdCharacter(Peek(), id.Length == 0))
                    id += ReadAny();
                return id.Length > 0;
            }
            public char Peek(int offset)
{
    if (pos + offset < 0 || pos + offset >= code.Length)
        return '\0'; // Return null character if out of bounds
    return code[pos + offset];
}
public bool CanLookAhead(int offset = 1)
{
    return pos + offset >= 0 && pos + offset < code.Length;
}

            public bool ReadNumber(out string number)
            {
                number = "";
                while (!EOL && char.IsDigit(Peek()))
                    number += ReadAny();
                if (!EOL && Match("."))
                {
                    // read decimal part
                    number += '.';
                    while (!EOL && char.IsDigit(Peek()))
                        number += ReadAny();
                }

                if (number.Length == 0)
                    return false;

                // Load Number posfix, i.e., 34.0F
                // Not supported exponential formats: 1.3E+4
                while (!EOL && char.IsLetterOrDigit(Peek()))
                    number += ReadAny();

                return number.Length > 0;
            }

            public bool ReadUntil(string end, out string text)
            {
                text = "";
                while (!Match(end))
                {
                    if (EOL || EOF)
                        return false;
                    text += ReadAny();
                }
                return true;
            }

            public bool ReadWhiteSpace()
            {
                if (char.IsWhiteSpace(Peek()))
                {
                    ReadAny();
                    return true;
                }
                return false;
            }

            public char ReadAny()
            {
                if (EOF)
                    throw new InvalidOperationException();

                if (EOL)
                {
                    line++;
                    lastLB = pos;
                }
                return code[pos++];
            }
        }
