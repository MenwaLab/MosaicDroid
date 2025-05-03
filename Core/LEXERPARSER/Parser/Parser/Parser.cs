public class Parser
{
    private readonly List<Token> tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    private Token? Peek(int offset)
    {
        int index = current + offset;
        return index < tokens.Count ? tokens[index] : null;
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
        // Skip empty lines (JUMPLINEs)
        if (Match(TokenType.JUMPLINE))
        {
            Advance();
            statements.Add(new JumpLineExpression());
            continue;
        }

        // Check for label declarations: VARIABLE followed by JUMPLINE
        if (Match(TokenType.VARIABLE))
        {
            Token currentToken = Current;
            Token? nextToken = Peek(1);

            if (nextToken != null && nextToken.Type == TokenType.JUMPLINE && IsValidLabelName(currentToken.Value))
            {
                // Consume the VARIABLE token and add as label
                string labelName = currentToken.Value;
                Advance(); // Consume VARIABLE
                statements.Add(new LabelDeclarationExpression(labelName));
                continue; // Skip further processing for this iteration
            }
        }

        // Parse other statements (instructions, assignments, etc.)
        IExpression statement = ParseStatement();
        statements.Add(statement);

        // Skip JUMPLINE after the statement if present
        if (current < tokens.Count && Match(TokenType.JUMPLINE))
            Advance();
    }

    return new ProgramExpression(statements);
}
    private IExpression ParseStatement()
{
    // If the token is a command-like token (either COMMAND or GOTO), parse it as a command.
    if (Match(TokenType.INSTRUCTION) || Match(TokenType.GOTO))
        return ParseInstruction();
    
    // For assignments: if the next token is an identifier and is followed by ASSIGN, treat it as an assignment.
    if (Match(TokenType.VARIABLE))
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
                return ParseFunction(idToken.Value);
            
            // Otherwise, simply an identifier expression.
            return new VariableExpression(idToken.Value);
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
    private IExpression ParseFunction(string function)
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
        switch (function)
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
                if (!(arguments[0] is LiteralColorExpression colorEx))
                    throw new Exception("First argument for GetColorCount must be a color literal");
                return new GetColorCountExpression(colorEx.Value, arguments[1], arguments[2], arguments[3], arguments[4]);
            case "IsBrushColor":
            if (arguments.Count != 1 || arguments[0] is not LiteralColorExpression bc)
                throw new Exception("Argument for IsBrushColor must be a color literal");
            return new IsBrushColorExpression(bc.Value);
            case "IsBrushSize":
            if (arguments.Count != 1)
                throw new Exception("IsBrushSize expects 1 argument");
            return new IsBrushSizeExpression(arguments[0]);

            case "IsCanvasColor":
            if (arguments.Count != 3 || arguments[0] is not LiteralColorExpression canvasColor)
                throw new Exception("First argument for IsCanvasColor must be a color literal");
            return new IsCanvasColorExpression(canvasColor.Value, arguments[1], arguments[2]);
            default:
                throw new Exception($"Unknown function: {function}");
        }
    }

    public IExpression Parse()
    {
        return ParseAssignment();
    }

    private IExpression ParseAssignment()
    {
        if (Match(TokenType.VARIABLE))
        {
            string varName = Advance().Value;

            if (Match(TokenType.ASSIGN))
            {
                Advance();
                IExpression expr = ParseExpression();
                return new AssignExpression(varName, expr);
            }

            return new VariableExpression(varName);
        }

        return ParseExpression();
    }

    private IExpression ParseInstruction()
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
    if (Match(TokenType.COLOR))
{
    Token colorToken = Advance();
    return new LiteralColorExpression(colorToken.Value);
}

    if (Match(TokenType.VARIABLE) || Match(TokenType.INSTRUCTION) || Match(TokenType.FUNCTION) )
{
    Token idToken = Advance();
    string identifier = idToken.Value;

    if (Match(TokenType.DELIMETER) && Current.Value == "(")
        return ParseFunction(identifier);

    return new VariableExpression(identifier);
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
    private bool IsValidLabelName(string value)
{
    // Must start with a letter (no digits or '-')
    if (string.IsNullOrEmpty(value) || !char.IsLetter(value[0]))
        return false;

    // Can only contain letters, numbers, and hyphens
    return value.All(c => char.IsLetterOrDigit(c) || c == '-');
}

}
