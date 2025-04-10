public class Parser
{
    private readonly List<Token> tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    private Token Current => current < tokens.Count 
    ? tokens[current] 
    : throw new Exception("No more tokens"); //asi?

    private Token Advance() => tokens[current++];
    private bool Match(TokenType type) => Current.Type == type;
    private void Expect(TokenType type)
    {
        if (!Match(type))
            throw new Exception($"Expected {type}, got {Current?.Type}");
        Advance();
    }
    public ProgramExpression ParseProgram()
    {
        List<IExpression> statements = new List<IExpression>();

        while (current < tokens.Count)
        {
            // Skip any extra newline tokens.
            if (Match(TokenType.JUMPLINE))
            {
                Advance();
                // Optionally add a jump-line to represent a statement separator.
                statements.Add(new JumpLineExpression());
                continue;
            }
            // If the token is a label declaration (e.g. a token of type LABEL), produce a LabelDeclarationExpression.
            if (Match(TokenType.LABEL))
            {
                string label = Advance().Value;
                statements.Add(new LabelDeclarationExpression(label));
                continue;
            }
            // Otherwise, parse a statement.
            IExpression stmt = ParseStatement();
            statements.Add(stmt);

            // Optionally, if a newline follows the statement, consume it.
            if (current < tokens.Count && Match(TokenType.JUMPLINE))
            {
                Advance();
                // You may choose to add a JumpLineExpression here or simply ignore it.
                // statements.Add(new JumpLineExpression());
            }
        }

        return new ProgramExpression(statements);
    }
    private IExpression ParseStatement()
{
    // If the token is a command-like token (either COMMAND or GOTO), parse it as a command.
    if (Match(TokenType.COMMAND) || Match(TokenType.GOTO))
        return ParseCommand();
    
    // For assignments: if the next token is an identifier and is followed by ASSIGN, treat it as an assignment.
    if (Match(TokenType.IDENTIFIER))
    {
        // Lookahead: if the next token is ASSIGN, then it's an assignment.
        Token idToken = Advance();
        if (Match(TokenType.ASSIGN))
        {
            // Assignment: <identifier> <- <expression>
            Advance(); // consume the ASSIGN token
            IExpression expr = ParseExpression();
            return new AssignExpression(idToken.Value, expr);
        }
        else
        {
            // It might be a function call if immediately followed by "(".
            if (Match(TokenType.DELIMETER) && Current.Value == "(")
                return ParseFunctionCall(idToken.Value);
            
            // Otherwise, simply an identifier expression.
            return new IdentifierExpression(idToken.Value);
        }
    }
    
    // Otherwise, parse as an expression.
    return ParseExpression();
}

    // Expression: start at the lowest-precedence level (logical OR).
    private IExpression ParseExpression() => ParseLogicalOr();

    // Logical OR: <logical-and> ( "||" <logical-and> )*
    private IExpression ParseLogicalOr()
    {
        IExpression left = ParseLogicalAnd();
        while (Match(TokenType.BOOL_OP) && Current.Value == "||")
        {
            Advance(); // consume "||"
            IExpression right = ParseLogicalAnd();
            left = new LogicalOrExpression(left, right);
        }
        return left;
    }

    // Logical AND: <equality> ( "&&" <equality> )*
    private IExpression ParseLogicalAnd()
    {
        IExpression left = ParseEquality();
        while (Match(TokenType.BOOL_OP) && Current.Value == "&&")
        {
            Advance(); // consume "&&"
            IExpression right = ParseEquality();
            left = new LogicalAndExpression(left, right);
        }
        return left;
    }

    // Equality: <relational> ( "==" <relational> )*
    private IExpression ParseEquality()
    {
        IExpression left = ParseRelational();
        while (Match(TokenType.BOOL_OP) && Current.Value == "==")
        {
            Advance(); // consume "=="
            IExpression right = ParseRelational();
            left = new EqualExpression(left, right);
        }
        return left;
    }

    // Relational: <additive> ( ( ">" | "<" | ">=" | "<=" ) <additive> )*
    private IExpression ParseRelational()
    {
        IExpression left = ParseAdditive();
        while (Match(TokenType.BOOL_OP) && 
              (Current.Value == ">" || Current.Value == "<" || Current.Value == ">=" || Current.Value == "<="))
        {
            string op = Advance().Value;
            IExpression right = ParseAdditive();
            switch (op)
            {
                case ">":
                    left = new GreaterExpression(left, right);
                    break;
                case "<":
                    left = new LessExpression(left, right);
                    break;
                case ">=":
                    left = new GreaterEqualExpression(left, right);
                    break;
                case "<=":
                    left = new LessEqualExpression(left, right);
                    break;
                default:
                    throw new Exception("Unsupported relational operator: " + op);
            }
        }
        return left;
    }
    private IExpression ParseAdditive()
    {
        IExpression left = ParseTerm();
        while (Match(TokenType.OPERATOR) && (Current.Value == "+" || Current.Value == "-"))
        {
            string op = Advance().Value;
            IExpression right = ParseTerm();
            switch (op)
            {
                case "+":
                    left = new AddExpression(left, right);
                    break;
                case "-":
                    left = new SubtractExpression(left, right);
                    break;
            }
        }
        return left;
    }

    // Term: <factor> ( ("*" | "/" | "%" | "**") <factor> )*
    private IExpression ParseTerm()
    {
        IExpression left = ParseFactor();
        while (Match(TokenType.OPERATOR) && 
              (Current.Value == "*" || Current.Value == "/" || Current.Value == "%" || Current.Value == "**"))
        {
            string op = Advance().Value;
            IExpression right = ParseFactor();
            switch (op)
            {
                case "*": left = new MultiplyExpression(left, right); break;
                case "/": left = new DivideExpression(left, right); break;
                case "%": left = new ModuloExpression(left, right); break;
                case "**": left = new PowerExpression(left, right); break;
            }
        }
        return left;
    }
    private IExpression ParseFunctionCall(string functionName)
    {
        Expect(TokenType.DELIMETER); // consume "("
        List<IExpression> arguments = new List<IExpression>();

        // If next token is not ")", parse the arguments.
        if (!(Match(TokenType.DELIMETER) && Current.Value == ")"))
        {
            arguments.Add(ParseExpression());
            while (Match(TokenType.DELIMETER) && Current.Value == ",")
            {
                Advance(); // consume comma
                arguments.Add(ParseExpression());
            }
        }
        Expect(TokenType.DELIMETER); // consume ")"

        // Create the proper function call node.
        switch (functionName)
        {
            case "GetActualX":
                return new GetActualXExpression();
            case "GetActualY":
                return new GetActualYExpression();
            case "GetCanvasSize":
                return new GetCanvasSizeExpression();
            case "GetColorCount":
                if (arguments.Count != 5)
                    throw new Exception("GetColorCount expects 5 arguments");
                if (!(arguments[0] is StringExpression lse))
                    throw new Exception("First argument for GetColorCount must be a literal string");
                return new GetColorCountExpression(lse.Value, arguments[1], arguments[2], arguments[3], arguments[4]);
            case "IsBrushColor":
                if (arguments.Count != 1)
                    throw new Exception("IsBrushColor expects 1 argument");
                if (!(arguments[0] is StringExpression lse2))
                    throw new Exception("Argument for IsBrushColor must be a literal string");
                return new IsBrushColorExpression(lse2.Value);
            case "IsBrushSize":
                if (arguments.Count != 1)
                    throw new Exception("IsBrushSize expects 1 argument");
                return new IsBrushSizeExpression(arguments[0]);
            case "IsCanvasColor":
                if (arguments.Count != 3)
                    throw new Exception("IsCanvasColor expects 3 arguments");
                if (!(arguments[0] is StringExpression lse3))
                    throw new Exception("First argument for IsCanvasColor must be a literal string");
                return new IsCanvasColorExpression(lse3.Value, arguments[1], arguments[2]);
            default:
                throw new Exception($"Unknown function: {functionName}");
        }
    }

    public IExpression Parse()
    {
        return ParseAssignment();
    }

    private IExpression ParseAssignment()
    {
        if (Match(TokenType.IDENTIFIER))
        {
            string varName = Advance().Value;

            if (Match(TokenType.ASSIGN))
            {
                Advance();
                IExpression expr = ParseExpression();
                return new AssignExpression(varName, expr);
            }

            return new IdentifierExpression(varName);
        }

        return ParseExpression();
    }

    private IExpression ParseCommand()
    {
        Token token = Advance(); // consume the COMMAND token.
        switch (token.Value)
        {
            case "Spawn":
                Expect(TokenType.DELIMETER); // expect "("
                IExpression xExpr = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ","
                IExpression yExpr = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ")"
                return new SpawnExpression(xExpr, yExpr);
            case "GoTo":
                // Expect a label token of type LABEL.
                Token labelToken = Advance();   // If your lexer has separated the label [loop-ends-here]
                // Expect a condition in parentheses.
                Expect(TokenType.DELIMETER); // expecting "("
                IExpression condition = ParseExpression();
                Expect(TokenType.DELIMETER); // expecting ")"
                return new GotoExpression(labelToken.Value, condition);


            case "Color":
                Expect(TokenType.DELIMETER); // expect "("
                Token colorToken = Advance(); // assume a string or color token
                Expect(TokenType.DELIMETER); // expect ")"
                return new ColorExpression(colorToken.Value);
            case "Size":
                Expect(TokenType.DELIMETER); // expect "("
                IExpression sizeExpr = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ")"
                return new SizeExpression(sizeExpr);
            case "DrawLine":
                Expect(TokenType.DELIMETER); // expect "("
                IExpression dirX = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ","
                IExpression dirY = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ","
                IExpression distance = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ")"
                return new DrawLineExpression(dirX, dirY, distance);
            case "DrawCircle":
                Expect(TokenType.DELIMETER); // expect "("
                IExpression circleDirX = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ","
                IExpression circleDirY = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ","
                IExpression radius = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ")"
                return new DrawCircleExpression(circleDirX, circleDirY, radius);
            case "DrawRectangle":
                Expect(TokenType.DELIMETER); // expect "("
                IExpression rectDirX = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ","
                IExpression rectDirY = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ","
                IExpression rectDistance = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ","
                IExpression rectWidth = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ","
                IExpression rectHeight = ParseExpression();
                Expect(TokenType.DELIMETER); // expect ")"
                return new DrawRectangleExpression(rectDirX, rectDirY, rectDistance, rectWidth, rectHeight);
            case "Fill":
                Expect(TokenType.DELIMETER); // expect "("
                Expect(TokenType.DELIMETER); // expect ")"
                return new FillExpression();
            default:
                throw new Exception($"Unknown command: {token.Value}");
        }
    }
private void SkipNewlines()
{
    while (current < tokens.Count && Match(TokenType.JUMPLINE))
        Advance();
}

    private IExpression ParseFactor()
    {
        SkipNewlines();
        if (Match(TokenType.INTEGER))
    {
        int value = int.Parse(Advance().Value);
        return new IntegerExpression(value);
    }

    if (Match(TokenType.STRING))
    {
        Token strToken = Advance();
        return new StringExpression(strToken.Value);
    }

    if (Match(TokenType.IDENTIFIER) || Match(TokenType.COMMAND))
{
    Token idToken = Advance();
    string identifier = idToken.Value;

    if (Match(TokenType.DELIMETER) && Current.Value == "(")
        return ParseFunctionCall(identifier);

    return new IdentifierExpression(identifier);
}


    // Grouping: e.g., ( expression )
    if (Match(TokenType.DELIMETER) && Current.Value == "(")
    {
        Advance(); // consume "("
        IExpression expr = ParseExpression();
        Expect(TokenType.DELIMETER); // should be ")"
        return expr;
    }

    throw new Exception($"Unexpected token in factor: {Current.Value}");

    }
    
}
