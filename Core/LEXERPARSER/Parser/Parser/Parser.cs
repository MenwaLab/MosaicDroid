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
    bool spawnSeen = false;

    // 1) Saltar líneas en blanco (Jumpline) al inicio
    while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Jumpline)
        _stream.MoveNext();

    // 2) Debe comenzar con Spawn(...)
    if (!_stream.CanLookAhead())
        return program;

    var first = _stream.LookAhead();
    if (first.Type != TokenType.Instruction || first.Value != TokenValues.Spawn)
    {
        _errors.Add(new CompilingError(first.Location, ErrorCode.Expected, "Se esperaba Spawn(x, y) al inicio"));
        return program;  // no seguimos parseando
    }
    // Parseamos el Spawn inicial
    statements.Add(ParseSpawnCommand());
    spawnSeen = true;
    ExpectNewLine();

    // 3) Ahora, cero o más líneas
    while (_stream.CanLookAhead())
    {
        var la = _stream.LookAhead();
        ASTNode node;

        // Detectar un segundo Spawn
        if (la.Type == TokenType.Instruction && la.Value == TokenValues.Spawn)
        {
            // Error único y salimos
            _errors.Add(new CompilingError(la.Location, ErrorCode.Invalid, "Solo se permite una única instrucción Spawn"));
            break;
        }

        switch (la.Type)
        {
            case TokenType.Jumpline:
                // líneas en blanco dentro del código las saltamos
                _stream.MoveNext();
                continue;

            case TokenType.Label:
                node = ParseLabel();
                break;

            case TokenType.Instruction:
                node = la.Value == TokenValues.GoTo
                    ? (ASTNode)ParseGoTo()
                    : ParseInstruction();
                break;

            case TokenType.Variable:
                // asignaciones vs etiquetas-islas
                if (_stream.CanLookAhead(1) && _stream.LookAhead(1).Type == TokenType.Assign)
                    node = ParseAssignment();
                else if (_stream.CanLookAhead(1) && _stream.LookAhead(1).Type == TokenType.Jumpline)
                {
                    node = new LabelExpression(_stream.Advance().Value, la.Location);
                    _stream.Advance();  // comerse el jumpline
                }
                else
                {
                    _errors.Add(new CompilingError(la.Location, ErrorCode.Invalid, $"Variable inesperada: {la.Value}"));
                    _stream.MoveNext();
                    continue;
                }
                break;

            default:
                _errors.Add(new CompilingError(la.Location, ErrorCode.Invalid, $"Token inesperado: {la.Value}"));
                _stream.MoveNext();
                continue;
        }

        statements.Add(node);
        ExpectNewLine();
    }

    // Montar el programa
    foreach (var stmt in statements)
        program.Statements.Add(stmt);
    program.Errors.AddRange(_errors);
    return program;
}


        private void ExpectNewLine()
        {
                while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Jumpline)
        _stream.MoveNext();
        }

        private ASTNode ParseSpawnCommand()
        {
            var tok = _stream.Advance();  // Spawn
            EatDelimiter(TokenValues.OpenParenthesis);
            var x = ParseExpression(); EatDelimiter(TokenValues.Comma);
            var y = ParseExpression(); EatDelimiter(TokenValues.ClosedParenthesis);
            return new SpawnCommand(x, y, tok.Location);
        }

        private ASTNode ParseInstruction()
        {
            var instr = _stream.Advance();
            switch (instr.Value)
            {
                case TokenValues.Color:
                    EatDelimiter(TokenValues.OpenParenthesis);
                    var colTok = _stream.Advance();
                    var colLit = new ColorLiteralExpression(colTok.Value, colTok.Location);
                    EatDelimiter(TokenValues.ClosedParenthesis);
                    return new ColorCommand(colLit, instr.Location);

                case TokenValues.Size:
                    EatDelimiter(TokenValues.OpenParenthesis);
                    var sizeExpr = ParseExpression();
                    EatDelimiter(TokenValues.ClosedParenthesis);
                    return new SizeCommand(sizeExpr, instr.Location);

                case TokenValues.DrawLine:
                    EatDelimiter(TokenValues.OpenParenthesis);
                    var dx = ParseExpression(); EatDelimiter(TokenValues.Comma);
                    var dy = ParseExpression(); EatDelimiter(TokenValues.Comma);
                    var dist = ParseExpression();
                    EatDelimiter(TokenValues.ClosedParenthesis);
                    return new DrawLineCommand(dx, dy, dist, instr.Location);

                case TokenValues.DrawCircle:
                    EatDelimiter(TokenValues.OpenParenthesis);
                    var cx = ParseExpression(); EatDelimiter(TokenValues.Comma);
                    var cy = ParseExpression(); EatDelimiter(TokenValues.Comma);
                    var rad = ParseExpression();
                    EatDelimiter(TokenValues.ClosedParenthesis);
                    return new DrawCircleCommand(cx, cy, rad, instr.Location);

                case TokenValues.DrawRectangle:
                    EatDelimiter(TokenValues.OpenParenthesis);
                    var rx = ParseExpression(); EatDelimiter(TokenValues.Comma);
                    var ry = ParseExpression(); EatDelimiter(TokenValues.Comma);
                    var rd = ParseExpression(); EatDelimiter(TokenValues.Comma);
                    var rw = ParseExpression(); EatDelimiter(TokenValues.Comma);
                    var rh = ParseExpression();
                    EatDelimiter(TokenValues.ClosedParenthesis);
                    return new DrawRectangleCommand(rx, ry, rd, rw, rh, instr.Location);

                case TokenValues.Fill:
                    EatDelimiter(TokenValues.OpenParenthesis); EatDelimiter(TokenValues.ClosedParenthesis);
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
            EatDelimiter(TokenValues.OpenBrackets);
            var lbl = _stream.Advance().Value;
            EatDelimiter(TokenValues.ClosedBrackets);
            EatDelimiter(TokenValues.OpenParenthesis);
            var cond = ParseExpression();
            EatDelimiter(TokenValues.ClosedParenthesis);
            return new GotoCommand(lbl, cond, tok.Location);
        }

        private Expression ParseExpression() => ParseLogicalOr();

        private Expression ParseLogicalOr()
        {
            var left = ParseLogicalAnd();
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Bool_OP && _stream.LookAhead().Value == TokenValues.Or)
            {
                _stream.MoveNext();
                _stream.Advance();
                var right = ParseLogicalAnd();
                var node = new LogicalOrExpression(left, right, left.Location);
                left = node;
            }
            return left;
        }

        private Expression ParseLogicalAnd()
        {
            var left = ParseEquality();
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Bool_OP && _stream.LookAhead().Value == TokenValues.And)
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
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Bool_OP && _stream.LookAhead().Value == TokenValues.Equal)
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
                if (op == TokenValues.Less || op == TokenValues.Greater || op == TokenValues.LessEqual || op == TokenValues.GreaterEqual)
                {
                    _stream.MoveNext();
                    var right = ParseAdditive();
                    switch (op)
                    {
                        case TokenValues.Greater:  left = new LogicalGreaterExpression(left, right, left.Location); break;
                        case TokenValues.Less:  left = new LogicalLessExpression(left, right, left.Location);    break;
                        case TokenValues.GreaterEqual: left = new LogicalGreaterEqualExpression(left, right, left.Location); break;
                        case TokenValues.LessEqual: left = new LogicalLessEqualExpression(left, right, left.Location);    break;
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
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Operator && ( _stream.LookAhead().Value == TokenValues.Add || _stream.LookAhead().Value == TokenValues.Sub ))
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
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Operator && ( _stream.LookAhead().Value == TokenValues.Mul || _stream.LookAhead().Value == TokenValues.Div || _stream.LookAhead().Value == TokenValues.Mod || _stream.LookAhead().Value == TokenValues.Pow ))
            {
                var op = _stream.Advance().Value;
                var right = ParseFactor();
                switch (op)
                {
                    case TokenValues.Mul:  var m = new Mul(left.Location); m.Left = left; m.Right = right; left = m; break;
                    case TokenValues.Div:  var d = new Div(left.Location); d.Left = left; d.Right = right; left = d; break;
                    case TokenValues.Mod:  var mod = new ModulusExpression(left.Location); mod.Left = left; mod.Right = right; left = mod; break;
                    case TokenValues.Pow: var p = new PowerExpression(left.Location); p.Left = left; p.Right = right; left = p; break;
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

            if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Delimeter && _stream.LookAhead().Value == TokenValues.OpenParenthesis)
            {
                _stream.MoveNext();
                var expr = ParseExpression();
                EatDelimiter(TokenValues.ClosedParenthesis);
                return expr;
            }

            _errors.Add(new CompilingError(_stream.LookAhead().Location, ErrorCode.Invalid, "Unexpected token in factor"));
            _stream.MoveNext();
            return new NoOpExpression(_stream.LookAhead().Location);
        }

        private Expression ParseFunctionCall()
        {
            var fnTok = _stream.Advance();
            EatDelimiter(TokenValues.OpenParenthesis);
            var args = new List<Expression>();
            if (!(_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Delimeter && _stream.LookAhead().Value == TokenValues.ClosedParenthesis))
            {
                args.Add(ParseExpression());
                while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Delimeter && _stream.LookAhead().Value == TokenValues.Comma)
                {
                    _stream.MoveNext();
                    args.Add(ParseExpression());
                }
            }
            EatDelimiter(TokenValues.ClosedParenthesis);
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
