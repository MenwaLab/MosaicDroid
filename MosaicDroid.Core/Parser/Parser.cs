namespace MosaicDroid.Core
{
    using System.Resources;
    using System.Text.RegularExpressions;


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

            // Saltar líneas en blanco del inicio
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Jumpline)
                _stream.MoveNext();

            // Debe comenzar con Spawn(...)
            if (!_stream.CanLookAhead())
                return program;

            var first = _stream.LookAhead();

            if (first.Type != TokenType.Instruction || first.Value != TokenValues.Spawn)
            {
                if ((first.Type == TokenType.Instruction || first.Type == TokenType.Variable) && first.Value.StartsWith("Spawn"))
                {
                     ErrorHelpers.MissingOpenParen(_errors, first.Location, "Spawn");
                }
                else
                {
                    ErrorHelpers.ExpectedSpawn(_errors, first.Location);
                }

                return program;
            }
            var firstStmt = ParseInstruction();
            statements.Add(firstStmt);
            EnsureNewlineAfter(firstStmt);

            while (_stream.CanLookAhead())
            {
                var la = _stream.LookAhead();
                ASTNode node;

                switch (la.Type)
                {
                    case TokenType.Jumpline:
                        _stream.MoveNext(); // saltar las líneas en blanco dentro del código
                        continue;

                    case TokenType.Label:
                        if (Regex.IsMatch(la.Value, @"^(Spawn|Color|Size|DrawLine|DrawCircle|DrawRectangle|Fill|Move)(?=[^(\s])")) // verifica si no es una instruccion malformada
                        {
                            ErrorHelpers.MissingOpenParen(_errors, la.Location, la.Value);
                            _stream.MoveNext();
                            continue;
                        }
                        var lbelTok = _stream.Advance();
                        if (program.LabelIndices.ContainsKey(lbelTok.Value))
                        {
                            // semantic error: “Lo siento, la etiqueta {0} solo se puede declarar una vez.”
                            ErrorHelpers.DuplicateLabel( _errors,lbelTok.Location,lbelTok.Value);
                        }
                        else {
                            program.LabelIndices[lbelTok.Value] = lbelTok.Location;
                        }
                       
                        EnsureNewlineAfter(lbelTok.Location);
                        continue;


                    case TokenType.Instruction:
                        node = ParseInstruction();
                        statements.Add(node);
                        EnsureNewlineAfter(node);
                        continue;

                    case TokenType.GoTo:
                        node = ParseGoTo();
                        statements.Add(node);
                        EnsureNewlineAfter(node);
                        continue;

                    case TokenType.Variable:

                        if (la.Value.StartsWith("-") && Regex.IsMatch(la.Value.Substring(1), @"^[A-Za-z][A-Za-z0-9_]*$"))
                        {
                            ErrorHelpers.InvalidVariableName(_errors, la.Location, la.Value);
                            Synchronize();
                            continue;
                        }

                        // Instrucción malformada como DrawLine1,0)
                        if (Regex.IsMatch(la.Value, @"^(Spawn|Color|Size|DrawLine|DrawCircle|DrawRectangle|Fill|Move)(?=[^(\s])"))
                        {
                            ErrorHelpers.MissingOpenParen(_errors, la.Location, la.Value);
                            _stream.MoveNext();
                            continue;
                        }

                        if (_stream.CanLookAhead(1) && _stream.LookAhead(1).Type == TokenType.Jumpline)
                        {
                            var labelTok = _stream.Advance();
                            program.LabelIndices[labelTok.Value] = labelTok.Location;
                            EnsureNewlineAfter(labelTok.Location);
                            continue;
                        }

                        if (_stream.CanLookAhead(1) && _stream.LookAhead(1).Type == TokenType.Assign)
                        {
                            node = ParseAssignment();
                            statements.Add(node);
                            EnsureNewlineAfter(node);
                            continue;
                        }
                        ErrorHelpers.UnexpectedToken(_errors, la.Location, la.Value);
                        _stream.MoveNext();
                        continue;
                    default:
                        ErrorHelpers.UnexpectedToken(_errors, la.Location, la.Value);
                        Synchronize();
                        break;
                }
            }

            program.Statements.AddRange(statements);
            program.Errors.AddRange(_errors);
            return program;

        }
        private void EnsureNewlineAfter(ASTNode stmt)
        {
            if (!_stream.CanLookAhead())
                return;
           

             if (_stream.LookAhead().Type != TokenType.Jumpline)
                ErrorHelpers.MissingNewLine(_errors, stmt.Location, "statement");

            // now consume any newlines
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Jumpline)
                _stream.MoveNext();
        }
        private void EnsureNewlineAfter(CodeLocation loc)
        {
            if (!(_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Jumpline))
                ErrorHelpers.MissingNewLine(_errors, loc, "label");

            // now swallow any blank lines
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Jumpline)
                _stream.MoveNext();
        }
        private void Synchronize() // avanza hasta que encuentre un jumpline, lo consume para recuperar el parseo 
        {
            while (_stream.CanLookAhead() && _stream.LookAhead().Type != TokenType.Jumpline)
                _stream.MoveNext();
            if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Jumpline)
                _stream.MoveNext();
        }

        private ASTNode ParseInstruction()
        {
            var instr = _stream.Advance();
            switch (instr.Value)
            {
                case TokenValues.Spawn:

                    var args = ParseArgumentList();
                    return new SpawnCommand(args, instr.Location);

                case TokenValues.Color:
                    EatDelimiter(TokenValues.OpenParenthesis);

                    var tokCol = _stream.Advance();
                    if (tokCol.Type != TokenType.String && tokCol.Type != TokenType.Color)
                    {
                        ErrorHelpers.MissingQuotation(_errors, tokCol.Location);
                    }

                    EatDelimiter(TokenValues.ClosedParenthesis);
                    var lit = new ColorLiteralExpression(tokCol.Value, tokCol.Location);
                    return new ColorCommand([lit], instr.Location);

                case TokenValues.Size:
                    var sizeArgs = ParseArgumentList();
                    return new SizeCommand(sizeArgs, instr.Location);

                case TokenValues.DrawLine:
                    var lineArgs = ParseArgumentList();
                    return new DrawLineCommand(lineArgs, instr.Location);

                case TokenValues.DrawCircle:
                    var circleArgs = ParseArgumentList();
                    return new DrawCircleCommand(circleArgs, instr.Location);

                case TokenValues.DrawRectangle:
                    var rectangleArgs = ParseArgumentList();
                    return new DrawRectangleCommand(rectangleArgs, instr.Location);

                case TokenValues.Fill:
                    var fillArgs = ParseArgumentList();
                    return new FillCommand(fillArgs, instr.Location);

                case TokenValues.Move:
                    var moveArgs = ParseArgumentList();
                    return new MoveCommand(moveArgs, instr.Location);

                default:
                    ErrorHelpers.UnknownInstrFunc(_errors, instr.Location, instr.Value);
                    return new NoOpExpression(instr.Location);
            }
        }

        private ASTNode ParseAssignment()
        {
            var varTok = _stream.Advance();

            if (!Regex.IsMatch(varTok.Value, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
            {
                ErrorHelpers.InvalidVariableName(_errors, varTok.Location, varTok.Value);
            }

            _stream.Advance();                   // '<-'
            var expr = ParseExpression();

            if (!_stream.CanLookAhead() || _stream.LookAhead().Type != TokenType.Jumpline)
            {
                ErrorHelpers.MissingNewLine(_errors, expr.Location, "var assign");
            }

            return new AssignExpression(varTok.Value, expr, varTok.Location);
        }

        private GotoCommand ParseGoTo()
        {
            var tok = _stream.Advance();
            EatDelimiter(TokenValues.OpenBrackets);

            var lbl = _stream.Advance().Value;
            EatDelimiter(TokenValues.ClosedBrackets);
            EatDelimiter(TokenValues.OpenParenthesis);

            var cond = ParseExpression();
            EatDelimiter(TokenValues.ClosedParenthesis);

            return new GotoCommand(lbl, cond, tok.Location);
        }

        private Expression ParseExpression() => ParseLogicalAnd(); // garantiza la precedencia de OR por encima de AND
    

        private Expression ParseLogicalAnd()
        {
            var left = ParseLogicalOr();
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Bool_OP && _stream.LookAhead().Value == TokenValues.And)
            {
                _stream.Advance();
                var right = ParseLogicalOr();
                left = new LogicalAndExpression(left, right, left.Location);
            }
            return left;
        }
        private Expression ParseLogicalOr()
        {
            var left = ParseEquality();
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Bool_OP && _stream.LookAhead().Value == TokenValues.Or)
            {
                _stream.Advance();

                var right = ParseEquality();
                left = new LogicalOrExpression(left, right, left.Location);
            }
            return left;
        }

        private Expression ParseEquality()
        {
            var left = ParseRelational();
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Bool_OP && _stream.LookAhead().Value == TokenValues.Equal)
            {
                _stream.Advance();
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
                    _stream.Advance();
                    var right = ParseAdditive();
                    switch (op)
                    {
                        case TokenValues.Greater: left = new LogicalGreaterExpression(left, right, left.Location); break;
                        case TokenValues.Less: left = new LogicalLessExpression(left, right, left.Location); break;
                        case TokenValues.GreaterEqual: left = new LogicalGreaterEqualExpression(left, right, left.Location); break;
                        case TokenValues.LessEqual: left = new LogicalLessEqualExpression(left, right, left.Location); break;
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
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Operator && (_stream.LookAhead().Value == TokenValues.Add || _stream.LookAhead().Value == TokenValues.Sub))
            {
                var op = _stream.Advance().Value;
                var right = ParseTerm();

                if (op == TokenValues.Add)
                {
                    var sum = new Add(left.Location);
                    sum.Left = left;
                    sum.Right = right;
                    left = sum;
                }
                else
                {
                    var sub = new Sub(left.Location);
                    sub.Left = left;
                    sub.Right = right;
                    left = sub;
                }
            }
            return left;
        }

        private Expression ParseTerm()
        {
            var left = ParsePower();
            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Operator &&
            (_stream.LookAhead().Value == TokenValues.Mul
            || _stream.LookAhead().Value == TokenValues.Div || _stream.LookAhead().Value == TokenValues.Mod))
            {
                var op = _stream.Advance().Value;
                var right = ParsePower();
                switch (op)
                {
                    case TokenValues.Mul: var m = new Mul(left.Location); m.Left = left; m.Right = right; left = m; break;
                    case TokenValues.Div: var d = new Div(left.Location); d.Left = left; d.Right = right; left = d; break;
                    case TokenValues.Mod: var mod = new ModulusExpression(left.Location); mod.Left = left; mod.Right = right; left = mod; break;
                }
            }
            return left;
        }

        private Expression ParseFactor()
        {
            // Signo menos seguido de un #: # negativo
            if (_stream.CanLookAhead()
                && _stream.LookAhead().Type == TokenType.Operator
                && _stream.LookAhead().Value == TokenValues.Sub
                && _stream.CanLookAhead(1)
                && _stream.LookAhead(1).Type == TokenType.Integer)
            {
                // consume el '-' y el #, entonces lo a # negativo
                var minusTok = _stream.Advance();      // '-'
                var numTok = _stream.Advance();      // #
                double v = double.Parse(numTok.Value);
                return new Number(-v, minusTok.Location);
            }

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
            ErrorHelpers.UnexpectedToken(_errors, _stream.LookAhead().Location, _stream.LookAhead().Value);
            _stream.MoveNext();
            return new NoOpExpression(_stream.LookAhead().Location);
        }
      
        private Expression ParsePower()
        {
            var left = ParseFactor();
            if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Operator && _stream.LookAhead().Value == TokenValues.Pow)
            {
                _stream.Advance();
                var right = ParsePower();
                var p = new PowerExpression(left.Location);
                p.Left = left;
                p.Right = right;
                left = p;
            }
            return left;
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

            return FunctionFactory.Create(fnTok.Value, args, fnTok.Location, _errors);
        }

        private void EatDelimiter(string d)
        {
            if (!(_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Delimeter && _stream.LookAhead().Value == d))
                 ErrorHelpers.MissingCloseParen(_errors, _stream.LookAhead().Location, d);
            else
                _stream.MoveNext();
        }
        private List<Expression> ParseArgumentList()
        {
            var args = new List<Expression>();
            EatDelimiter(TokenValues.OpenParenthesis);

            // si inmediatamente ) => 0 argumentos
            if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Delimeter
                && _stream.LookAhead().Value == TokenValues.ClosedParenthesis)
            {
                _stream.MoveNext();
                return args;
            }

            args.Add(ParseExpression());

            while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Delimeter && _stream.LookAhead().Value == TokenValues.Comma)
            {
                _stream.MoveNext();
                args.Add(ParseExpression());
            }

            EatDelimiter(TokenValues.ClosedParenthesis);

            return args;
        }
    }
}