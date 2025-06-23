using System.Resources;

namespace MosaicDroid.Core
{
    public class LexicalAnalyzer
    {
        private readonly Dictionary<string, string> operators = new();
        private readonly Dictionary<string, string> keywords = new();
        private readonly Dictionary<string, string> texts = new();

        public void RegisterOperator(string op, string tokenValue) => operators[op] = tokenValue;
        public void RegisterKeyword(string keyword, string tokenValue) => keywords[keyword] = tokenValue;
        public void RegisterText(string start, string end) => texts[start] = end;


        private static readonly ResourceManager _resmgr =
           new ResourceManager("MosaicDroid.Core.Resources.Strings", typeof(MatrixInterpreterVisitor).Assembly);
        public IEnumerable<Token> GetTokens(string code, List<CompilingError> errors)
        {
            var tokens = new List<Token>();
            var stream = new TokenReader(code);

            try
            {
                while (!stream.EOF)
                {
                    if (stream.Peek() == '\n') // cuando Peek() es '\n', genera Jumpline
                    {
                        tokens.Add(new Token(TokenType.Jumpline, TokenValues.Jumpline, stream.Location));
                        stream.ReadAny();
                        continue;
                    }
                    if (stream.ReadWhiteSpace()) continue; // Ignora espacios en blanco

                    // etiquetas del tipo [loop1]
                    if (stream.Match("["))
                    {
                        tokens.Add(new Token(TokenType.Delimeter, TokenValues.OpenBrackets, stream.Location));
                        if (stream.ReadID(out string label))
                            tokens.Add(new Token(TokenType.Label, label, stream.Location));
                        if (stream.Match("]"))
                            tokens.Add(new Token(TokenType.Delimeter, TokenValues.ClosedBrackets, stream.Location));
                        continue;
                    }

                    if (stream.ReadID(out string id))
                    {
                        if (keywords.TryGetValue(id, out string? tokenValue))
                        {
                            TokenType type = tokenValue switch
                            {
                                TokenValues.Spawn or TokenValues.Color or TokenValues.Size or TokenValues.DrawLine
                                or TokenValues.DrawCircle or TokenValues.DrawRectangle or TokenValues.Fill or TokenValues.Move
                                => TokenType.Instruction,

                                TokenValues.GoTo => TokenType.GoTo,

                                TokenValues.GetActualX or TokenValues.GetActualY or TokenValues.GetCanvasSize
                                or TokenValues.GetColorCount or TokenValues.IsBrushColor or TokenValues.IsBrushSize
                                or TokenValues.IsCanvasColor => TokenType.Function,
                                _ => TokenType.Variable
                            };
                            tokens.Add(new Token(type, tokenValue, stream.Location));
                        }
                        else
                        {
                            // Mirar el ultimo token para decidir el context
                            Token? lastToken = tokens.Count > 0 ? tokens[^1] : null;
                            bool afterParenOrComma = lastToken != null &&
                                lastToken.Type == TokenType.Delimeter &&
                                 (lastToken.Value == TokenValues.OpenParenthesis || lastToken.Value == TokenValues.Comma);

                            bool afterAssign = lastToken != null && lastToken.Type == TokenType.Assign;
                            if (afterParenOrComma || afterAssign) // Si va tras paréntesis/coma/asignación, se trata como Variable.

                            {
                                tokens.Add(new Token(TokenType.Variable, id, stream.Location));
                            }

                            else
                            {
                                char nextChar = stream.CanLookAhead(1) ? stream.Peek(1) : '\0';

                                bool atLineEnd = nextChar == '\n' || nextChar == '\r' || nextChar == '\0';

                                bool followsOperatorOrAssign = lastToken != null && (lastToken.Type == TokenType.Operator || lastToken.Type == TokenType.Bool_OP || lastToken.Type == TokenType.Assign || (lastToken.Type == TokenType.Delimeter
                              && (lastToken.Value == TokenValues.OpenParenthesis || lastToken.Value == TokenValues.Comma)));

                                // if (nextChar == '\n' || nextChar == '\r' || nextChar == '\0')
                                if (atLineEnd && !followsOperatorOrAssign)
                                {
                                    tokens.Add(new Token(TokenType.Label, id, stream.Location)); // Si va al final de línea y no desoues de un operador, se considera Label.
                                }

                                else if (IsValidIdentifier(id))
                                    tokens.Add(new Token(TokenType.Variable, id, stream.Location));

                                else
                                    ErrorHelpers.InvalidIdentifier(errors, stream.Location, id);
                            }

                        }
                        continue;
                    }

                    // Matchear ints
                    if (stream.ReadNumber(out string number))

                    {
                        if (!double.TryParse(number, out _))
                            ErrorHelpers.InvalidInteger(errors, stream.Location, number);

                        tokens.Add(new Token(TokenType.Integer, number, stream.Location));
                        continue;
                    }


                    // Matchear Strings/Colors
                    if (MatchText(stream, tokens, errors)) continue;

                    // Matchear Simbolos (Operadores, Delimiters, Assignments)
                    if (MatchSymbol(stream, tokens)) continue;

                    // Caracteres desconocidos
                    var unknownChar = stream.ReadAny();
                    ErrorHelpers.UnrecognizedChar(errors, stream.Location, unknownChar);
                }
            }
            catch(PixelArtRuntimeException)
            {
                errors.Add(new LexicalError( stream.Location, LexicalErrorCode.UnexpectedEOI, _resmgr.GetString("Inv_DrwLine") ));
            }

            return tokens;
        }

        private bool MatchSymbol(TokenReader stream, List<Token> tokens)
        {
            foreach (var op in operators.Keys.OrderByDescending(k => k.Length))
            {
                if (!stream.Match(op)) continue;
                var val = operators[op];

                TokenType type = val switch
                {

                    TokenValues.And or TokenValues.Or or TokenValues.Equal or TokenValues.GreaterEqual
                    or TokenValues.LessEqual or TokenValues.Greater or TokenValues.Less => TokenType.Bool_OP,

                    TokenValues.Assign => TokenType.Assign,

                    TokenValues.OpenParenthesis or TokenValues.ClosedParenthesis or TokenValues.Comma
                    => TokenType.Delimeter,

                    _ => TokenType.Operator
                };
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
                stream.ReadAny(); // consume  "
                if (!stream.ReadUntil("\"", out string text))
                {
                    ErrorHelpers.UnterminatedString(errors, stream.Location);
                    return false;
                }

                bool isColor = ColorValidationHelper.IsValidWpfColor(text);
                var type = isColor ? TokenType.Color : TokenType.String;
                tokens.Add(new Token(type, text, startLoc));
                return true;
            }

            foreach (var start in texts.Keys.OrderByDescending(k => k.Length))
            {
                if (!stream.Match(start)) continue;

                if (!stream.ReadUntil(texts[start], out string text))
                {
                    ErrorHelpers.UnterminatedText(errors, stream.Location, texts[start]);
                    return false;
                }

                TokenType type = ColorValidationHelper.IsValidWpfColor(text) ? TokenType.Color : TokenType.String;
                tokens.Add(new Token(type, text, stream.Location));
                return true;
            }
            return false;
        }

        private static bool IsValidIdentifier(string id) => !string.IsNullOrEmpty(id) &&
        (char.IsLetter(id[0]) || id[0] == '_') && id.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-');
    }
    class TokenReader
    {
        string code;
        int pos;
        int line;
        int lastLB;

        private static readonly ResourceManager _resmgr =
          new ResourceManager("MosaicDroid.Core.Resources.Strings", typeof(MatrixInterpreterVisitor).Assembly);
        public TokenReader(string code)
        {
            this.code = code;
            pos = 0;
            line = 1;
            lastLB = -1;
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

        public char Peek()
        {
            if (pos < 0 || pos >= code.Length)
              //  throw new InvalidOperationException();
            throw new PixelArtRuntimeException($"{_resmgr.GetString("UnexpectedEOI")}");

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
                return '\0'; // retorna null si out of bounds
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
                // lee la parte decimal
                number += '.';
                while (!EOL && char.IsDigit(Peek()))
                    number += ReadAny();
            }

            if (number.Length == 0)
                return false;

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
                //throw new InvalidOperationException();
                //throw new PixelArtRuntimeException($"{_resmgr.GetString("UnexpectedEOI")}");
            throw new PixelArtRuntimeException($"{_resmgr.GetString("UnexpectedEOI")} ¨{Location.Line}:{Location.Column}");

            if (EOL)
            {
                line++;
                lastLB = pos;
            }
            return code[pos++];
        }
    }
}