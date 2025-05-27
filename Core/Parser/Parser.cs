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
                _errors.Add(new CompilingError(first.Location, ErrorCode.Expected, "'OpenParenthesis' expected"));
            }
            else
            {
                _errors.Add(new CompilingError(first.Location, ErrorCode.Expected, "Se esperaba Spawn(x, y) al inicio"));
            }
            
            return program;
        }
        var firstStmt = ParseInstruction();
        statements.Add(firstStmt);
        //ExpectNewLine();
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
                    if (Regex.IsMatch(la.Value, @"^(Spawn|Color|Size|DrawLine|DrawCircle|DrawRectangle|Fill)(?=[^(\s])"))
                    {
                        _errors.Add(new CompilingError(la.Location, ErrorCode.Expected, "'(' expected after instruction name"));
                        _stream.MoveNext();
                        continue;
                    }
                /* if (_stream.CanLookAhead(1) && _stream.LookAhead(1).Type == TokenType.Jumpline)
                {
                    var lblTok = _stream.Advance();
                    if (!Regex.IsMatch(lblTok.Value, @"^[A-Za-z][A-Za-z0-9_]*$")) // variable‐style check para valid label syntax
                    {
                        _errors.Add(new CompilingError(lblTok.Location, ErrorCode.Invalid,$"Invalid label name '{lblTok.Value}'"));
                    }

                    if (!_stream.CanLookAhead() || _stream.LookAhead().Type != TokenType.Jumpline)
                    {
                        _errors.Add(new CompilingError(lblTok.Location, ErrorCode.Expected,"Expected a newline after label declaration."));
                    } */
                    //node = new LabelExpression(lblTok.Value, lblTok.Location);
                    node = ParseLabel();
                    statements.Add(node); //y
                    //ExpectNewLine();
                    EnsureNewlineAfter(node);
                    continue;
                /* }
                break; */

                case TokenType.Instruction:
                    //if (la.Value == TokenValues.GoTo)
                    //node = ParseGoTo();
                    //else
                        node = ParseInstruction();
                        statements.Add(node);
                        //ExpectNewLine();
                        EnsureNewlineAfter(node);
                        continue;

                case TokenType.GoTo:
                    node = ParseGoTo();
                    statements.Add(node);
                    //ExpectNewLine();
                    EnsureNewlineAfter(node);
                    continue;

                case TokenType.Variable:

                    if (la.Value.StartsWith("-") && Regex.IsMatch(la.Value.Substring(1), @"^[A-Za-z][A-Za-z0-9_]*$"))
                    {
                        _errors.Add(new CompilingError(la.Location, ErrorCode.Invalid,
                        $"Invalid variable name '{la.Value}'"));
                        Synchronize();
                        continue;
                    }

                    // Instrucción malformada como DrawLine1,0)
                    if (Regex.IsMatch(la.Value, @"^(Spawn|Color|Size|DrawLine|DrawCircle|DrawRectangle|Fill)(?=[^(\s])"))
                    {
                        _errors.Add(new CompilingError(la.Location, ErrorCode.Expected,"'(' expected after instruction name"));
                        _stream.MoveNext();
                        continue;
                    }

                    if (_stream.CanLookAhead(1) && _stream.LookAhead(1).Type == TokenType.Assign)
                    {
                        node = ParseAssignment();
                        statements.Add(node);
                        //ExpectNewLine();
                        EnsureNewlineAfter(node);
                        continue;
                    }
                    _errors.Add(new CompilingError(la.Location, ErrorCode.Invalid, $"Unexpected variable usage: {la.Value}"));
                    _stream.MoveNext();
                    continue;
                default:
                    _errors.Add(new CompilingError(la.Location, ErrorCode.Invalid, $"Token inesperado: {la.Value}"));
                    Synchronize();
                    //_stream.MoveNext();
                    break;
            }

            //_errors.Add(new CompilingError(la.Location, ErrorCode.Invalid, $"Token inesperado: {la.Value}"));
            //_stream.MoveNext();
           // Synchronize();
        }

        program.Statements.AddRange(statements);
        program.Errors.AddRange(_errors);
        return program;

    }
    private void ExpectNewLine()
    {
        while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Jumpline)
            _stream.MoveNext();
    }
    private void EnsureNewlineAfter(ASTNode stmt) 
    //Immediately after consuming a statement, require at least one  (or end‐of‐file). 
    // If missing, emit a MissingJumpline error, then swallow any jumplines.
    {
        if (!(_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Jumpline))
        {
            _errors.Add(new CompilingError(
                stmt.Location,
                ErrorCode.Expected,      // or define a new ErrorCode.MissingJumpline
                "Missing newline after statement"
            ));
        }
        // swallow all actual newlines to stay in sync
        ExpectNewLine();
    }
    private void Synchronize()
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
                    _errors.Add(new CompilingError(tokCol.Location, ErrorCode.Expected, "Color argument must be a quoted string (e.g., \"Red\")"));
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

            default:
                _errors.Add(new CompilingError(instr.Location, ErrorCode.Invalid, $"Unknown instruction: {instr.Value}"));
                return new NoOpExpression(instr.Location);
        }
    }

    private LabelExpression ParseLabel() //no ref?
    {
        var tok = _stream.Advance();
        var label = new LabelExpression(tok.Value, tok.Location);

        /* if (_stream.CanLookAhead() && _stream.LookAhead().Type != TokenType.Jumpline)
        {
        _errors.Add(new CompilingError(tok.Location, ErrorCode.Expected, "Expected newline after label declaration"));
        } */
    
        return label;
    }

    private ASTNode ParseAssignment()
    {
        var varTok = _stream.Advance();  

        if (!Regex.IsMatch(varTok.Value, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
        {
            _errors.Add(new CompilingError(varTok.Location, ErrorCode.Invalid, $"Invalid variable name '{varTok.Value}'"));
        }

        _stream.Advance();                   // '<-'
        var expr = ParseExpression();

        if (!_stream.CanLookAhead() || _stream.LookAhead().Type != TokenType.Jumpline)
        {
            _errors.Add(new CompilingError(expr.Location, ErrorCode.Expected, "Expected a newline after variable assignment."));
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

    public Expression ParseExpression() => ParseLogicalOr();
//change back to priv
    private Expression ParseLogicalOr()
    {
        var left = ParseLogicalAnd();
        while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Bool_OP && _stream.LookAhead().Value == TokenValues.Or)
        {
            //_stream.MoveNext();
            _stream.Advance();
            
            var right =  ParseLogicalAnd();
             left= new LogicalOrExpression(left, right, left.Location); //var node=
            //left = node;
        }
        return left;
    }

    private Expression ParseLogicalAnd()
    {
        var left = ParseEquality();
        while (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Bool_OP && _stream.LookAhead().Value == TokenValues.And)
        {
            _stream.Advance();//wasnt here
            //_stream.MoveNext();
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
            
            if (op == TokenValues.Add)
            {
                var sum = new Add(left.Location);
                sum.Left  = left;
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
        ( _stream.LookAhead().Value == TokenValues.Mul 
        || _stream.LookAhead().Value == TokenValues.Div || _stream.LookAhead().Value == TokenValues.Mod))
        // || _stream.LookAhead().Value == TokenValues.Pow ))
        {
            var op = _stream.Advance().Value;
            var right = ParsePower();
            switch (op)
            {
                case TokenValues.Mul:  var m = new Mul(left.Location); m.Left = left; m.Right = right; left = m; break;
                case TokenValues.Div:  var d = new Div(left.Location); d.Left = left; d.Right = right; left = d; break;
                case TokenValues.Mod:  var mod = new ModulusExpression(left.Location); mod.Left = left; mod.Right = right; left = mod; break;
                //case TokenValues.Pow: var p = new PowerExpression(left.Location); p.Left = left; p.Right = right; left = p; break;
            }
        }
        return left;
    }

    private Expression ParseFactor()
    {
        //var left = ParsePower();
             // Signo menos seguido de un #: # negativo
        if (_stream.CanLookAhead() 
            && _stream.LookAhead().Type == TokenType.Operator 
            && _stream.LookAhead().Value == TokenValues.Sub
            && _stream.CanLookAhead(1)
            && _stream.LookAhead(1).Type == TokenType.Integer)
        {
            // consume el '-' y el #, then wrap into a # negativo
            var minusTok = _stream.Advance();      // '-'
            var numTok   = _stream.Advance();      // #
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

        _errors.Add(new CompilingError(_stream.LookAhead().Location, ErrorCode.Expected, $"Unexpected token {_stream.LookAhead().Value} in factor"));
        _stream.MoveNext();
        return new NoOpExpression(_stream.LookAhead().Location);
    }
private Expression ParsePrimary()
{
    // Handle integer literals
    if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Integer)
    {
        var tok = _stream.Advance();
        return new Number(double.Parse(tok.Value), tok.Location);
    }

    // Handle string literals
    if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.String)
    {
        var tok = _stream.Advance();
        return new StringExpression(tok.Value, tok.Location);
    }

    // Handle color literals
    if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Color)
    {
        var tok = _stream.Advance();
        return new ColorLiteralExpression(tok.Value, tok.Location);
    }

    // Handle function calls
    if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Function)
    {
        return ParseFunctionCall();
    }

    // Handle variables
    if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Variable)
    {
        var tok = _stream.Advance();
        return new VariableExpression(tok.Value, tok.Location);
    }

    // Handle parenthesized expressions
    if (_stream.CanLookAhead() && _stream.LookAhead().Type == TokenType.Delimeter && _stream.LookAhead().Value == TokenValues.OpenParenthesis)
    {
        _stream.MoveNext();
        var expr = ParseExpression();
        EatDelimiter(TokenValues.ClosedParenthesis);
        return expr;
    }

    // Error handling for unexpected tokens
    _errors.Add(new CompilingError(_stream.LookAhead().Location, ErrorCode.Expected, $"Unexpected token {_stream.LookAhead().Value} in primary expression"));
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
            _errors.Add(new CompilingError(_stream.LookAhead().Location, ErrorCode.Expected, $"'{d}' expected"));
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