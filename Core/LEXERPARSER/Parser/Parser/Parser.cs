public enum ColorOptions
{
  Red, Blue, Green, Yellow, Orange, Purple, Black, White, Transparent
}
    public class Parser
    {
        private readonly TokenStream _stream;
        private readonly List<CompilingError> _errors;

        public Parser(TokenStream stream, List<CompilingError> errors)
        {
            _stream = stream;
            _errors = errors;
        }

        public ProgramExpression ParseProgram()
        {
            var program = new ProgramExpression(new CodeLocation());
            var statements = new List<ASTNode>();

            // Must start with Spawn
            if (!_stream.CanLookAhead())
                return program;

            var first = _stream.LookAhead();
            if (!(first.Type == TokenType.Instruction && first.Value == TokenValues.Spawn))
            {
                _errors.Add(new CompilingError(first.Location, ErrorCode.Expected, "Spawn expected"));
            }
            else
            {
                statements.Add(ParseSpawnCommand());
                ExpectNewLine();
            }

            // Then zero or more lines
            while (_stream.CanLookAhead())
            {
                var la = _stream.LookAhead();
                ASTNode node;

            


                switch (la.Type)
                {
                    case TokenType.Label:
                        node = ParseLabel();
                        break;
                    case TokenType.Instruction:
                        node = la.Value == TokenValues.GoTo 
                        ? (ASTNode)ParseGoTo() 
                        : ParseInstruction();
                        break;
                    case TokenType.Variable:
    if (_stream.CanLookAhead(1))
    {
        if (_stream.LookAhead(1).Type == TokenType.Jumpline)
        {
            // Treat as LabelExpression if followed by a newline
            node = new LabelExpression(_stream.Advance().Value, la.Location);
            _stream.Advance(); // Consume the newline
        }
        else if (_stream.LookAhead(1).Type == TokenType.Assign)
        {
            // Treat as a variable assignment
            node = ParseAssignment();
        }
        else
        {
            _errors.Add(new CompilingError(la.Location, ErrorCode.Invalid, $"Unexpected var: {la.Value}"));
            _stream.MoveNext();
            continue;
        }
    }
    else
    {
        _errors.Add(new CompilingError(la.Location, ErrorCode.Invalid, $"Unexpected token: {la.Value}"));
        _stream.MoveNext();
        continue;
    }
    break;
                    default:
                        _errors.Add(new CompilingError(la.Location, ErrorCode.Invalid, $"Unexpected token: {la.Value}"));
                        _stream.MoveNext();
                        continue;
                }

                statements.Add(node);
                ExpectNewLine();
            }

            // Populate
            foreach (var stmt in statements)
                program.Statements.Add(stmt);
            program.Errors.AddRange(_errors);
            return program;
        }

        private void ExpectNewLine()
        {
            /* if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Jumpline)
                _stream.MoveNext(); */
                while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Jumpline)
        _stream.MoveNext();
        }

        private ASTNode ParseSpawnCommand()
        {
            var tok = _stream.Advance();  // Spawn
            EatDelimiter("(");
            var x = ParseExpression(); EatDelimiter(",");
            var y = ParseExpression(); EatDelimiter(")");
            return new SpawnCommand(x, y, tok.Location);
        }

        private ASTNode ParseInstruction()
        {
            var instr = _stream.Advance();
            switch (instr.Value)
            {
                case TokenValues.Color:
                    EatDelimiter("(");
                    var colTok = _stream.Advance();
                    var colLit = new ColorLiteralExpression(colTok.Value, colTok.Location);
                    EatDelimiter(")");
                    return new ColorCommand(colLit, instr.Location);

                case TokenValues.Size:
                    EatDelimiter("(");
                    var sizeExpr = ParseExpression();
                    EatDelimiter(")");
                    return new SizeCommand(sizeExpr, instr.Location);

                case TokenValues.DrawLine:
                    EatDelimiter("(");
                    var dx = ParseExpression(); EatDelimiter(",");
                    var dy = ParseExpression(); EatDelimiter(",");
                    var dist = ParseExpression();
                    EatDelimiter(")");
                    return new DrawLineCommand(dx, dy, dist, instr.Location);

                case TokenValues.DrawCircle:
                    EatDelimiter("(");
                    var cx = ParseExpression(); EatDelimiter(",");
                    var cy = ParseExpression(); EatDelimiter(",");
                    var rad = ParseExpression();
                    EatDelimiter(")");
                    return new DrawCircleCommand(cx, cy, rad, instr.Location);

                case TokenValues.DrawRectangle:
                    EatDelimiter("(");
                    var rx = ParseExpression(); EatDelimiter(",");
                    var ry = ParseExpression(); EatDelimiter(",");
                    var rd = ParseExpression(); EatDelimiter(",");
                    var rw = ParseExpression(); EatDelimiter(",");
                    var rh = ParseExpression();
                    EatDelimiter(")");
                    return new DrawRectangleCommand(rx, ry, rd, rw, rh, instr.Location);

                case TokenValues.Fill:
                    EatDelimiter("("); EatDelimiter(")");
                    return new FillCommand(instr.Location);

                default:
                    _errors.Add(new CompilingError(instr.Location, ErrorCode.Invalid, $"Unknown instruction: {instr.Value}"));
                    return new NoOpExpression(instr.Location);
            }
        }

        private LabelExpression ParseLabel()
        {
            var tok = _stream.Advance();
            return new LabelExpression(tok.Value, tok.Location);
        }

        private ASTNode ParseAssignment()
        {
            var varTok = _stream.Advance();      // variable
            _stream.Advance();                   // '<-'
            var expr = ParseExpression();
            return new AssignExpression(varTok.Value, expr, varTok.Location);
        }

        private GotoCommand ParseGoTo()
        {
            var tok = _stream.Advance();        // GoTo
            EatDelimiter("[");
            var lbl = _stream.Advance().Value;
            EatDelimiter("]");
            EatDelimiter("(");
            var cond = ParseExpression();
            EatDelimiter(")");
            return new GotoCommand(lbl, cond, tok.Location);
        }

        private Expression ParseExpression() => ParseLogicalOr();

        private Expression ParseLogicalOr()
        {
            var left = ParseLogicalAnd();
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Bool_OP && _stream.LookAhead().Value == "||")
            {
                _stream.MoveNext();
                var right = ParseLogicalAnd();
                var node = new LogicalOrExpression(left, right, left.Location);
                left = node;
            }
            return left;
        }

        private Expression ParseLogicalAnd()
        {
            var left = ParseEquality();
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Bool_OP && _stream.LookAhead().Value == "&&")
            {
                _stream.MoveNext();
                var right = ParseEquality();
                left = new LogicalAndExpression(left, right, left.Location);
            }
            return left;
        }

        private Expression ParseEquality()
        {
            var left = ParseRelational();
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Bool_OP && _stream.LookAhead().Value == "==")
            {
                _stream.MoveNext();
                var right = ParseRelational();
                left = new LogicalEqualExpression(left, right, left.Location);
            }
            return left;
        }

        private Expression ParseRelational()
        {
            var left = ParseAdditive();
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Bool_OP)
            {
                var op = _stream.LookAhead().Value;
                if (op == ">" || op == "<" || op == ">=" || op == "<=")
                {
                    _stream.MoveNext();
                    var right = ParseAdditive();
                    switch (op)
                    {
                        case ">":  left = new LogicalGreaterExpression(left, right, left.Location); break;
                        case "<":  left = new LogicalLessExpression(left, right, left.Location);    break;
                        case ">=": left = new LogicalGreaterEqualExpression(left, right, left.Location); break;
                        case "<=": left = new LogicalLessEqualExpression(left, right, left.Location);    break;
                    }
                    continue;
                }
                break;
            }
            return left;
        }

        private Expression ParseAdditive()
        {
            var left = ParseTerm();
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Operator && ( _stream.LookAhead().Value == "+" || _stream.LookAhead().Value == "-" ))
            {
                var op = _stream.Advance().Value;
                var right = ParseTerm();
                var sum = new Add(left.Location);
                sum.Left  = left;
                sum.Right = right;
                left = sum;
            }
            return left;
        }

        private Expression ParseTerm()
        {
            var left = ParseFactor();
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Operator && ( _stream.LookAhead().Value == "*" || _stream.LookAhead().Value == "/" || _stream.LookAhead().Value == "%" || _stream.LookAhead().Value == "**" ))
            {
                var op = _stream.Advance().Value;
                var right = ParseFactor();
                switch (op)
                {
                    case "*":  var m = new Mul(left.Location); m.Left = left; m.Right = right; left = m; break;
                    case "/":  var d = new Div(left.Location); d.Left = left; d.Right = right; left = d; break;
                    case "%":  var mod = new ModulusExpression(left.Location); mod.Left = left; mod.Right = right; left = mod; break;
                    case "**": var p = new PowerExpression(left.Location); p.Left = left; p.Right = right; left = p; break;
                }
            }
            return left;
        }

        private Expression ParseFactor()
        {
            if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Integer)
            {
                var tok = _stream.Advance();
                return new Number(double.Parse(tok.Value), tok.Location);
            }

            if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.String)
            {
                var tok = _stream.Advance();
                return new StringExpression(tok.Value, tok.Location);
            }

            if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Color)
            {
                var tok = _stream.Advance();
                return new ColorLiteralExpression(tok.Value, tok.Location);
            }

            if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Function)
                return ParseFunctionCall();

            if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Variable)
            {
                var tok = _stream.Advance();
                return new VariableExpression(tok.Value, tok.Location);
            }

            if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Delimeter && _stream.LookAhead().Value == "(")
            {
                _stream.MoveNext();
                var expr = ParseExpression();
                EatDelimiter(")");
                return expr;
            }

            _errors.Add(new CompilingError(_stream.LookAhead().Location, ErrorCode.Invalid, "Unexpected token in factor"));
            _stream.MoveNext();
            return new NoOpExpression(_stream.LookAhead().Location);
        }

        private Expression ParseFunctionCall()
        {
            var fnTok = _stream.Advance();
            EatDelimiter("(");
            var args = new List<Expression>();
            if (!(_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Delimeter && _stream.LookAhead().Value == ")"))
            {
                args.Add(ParseExpression());
                while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Delimeter && _stream.LookAhead().Value == ",")
                {
                    _stream.MoveNext();
                    args.Add(ParseExpression());
                }
            }
            EatDelimiter(")");
            // TODO: provide or import a FunctionFactory
            return FunctionFactory.Create(fnTok.Value, args, fnTok.Location, _errors);
        }

        private void EatDelimiter(string d)
        {
            if (!(_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Delimeter && _stream.LookAhead().Value == d))
                _errors.Add(new CompilingError(_stream.LookAhead().Location, ErrorCode.Expected, $"'{d}' expected"));
            else
                _stream.MoveNext();
        }
    }
