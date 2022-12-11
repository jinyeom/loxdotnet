namespace Lox;

class Parser
{
    class ParseError : Exception
    {

    }

    readonly IList<Token> tokens;
    int current = 0;

    public Parser(IList<Token> tokens)
    {
        this.tokens = tokens;
    }

    bool IsAtEnd { get { return CurrentToken.Type == TokenType.Eof; } }

    Token CurrentToken { get { return tokens[current]; } }

    Token PreviousToken { get { return tokens[current - 1]; } }

    public IList<Stmt?> Parse()
    {
        var statements = new List<Stmt?>();
        while (!IsAtEnd)
        {
            statements.Add(Declaration());
        }
        return statements;
    }

    Stmt? Declaration()
    {
        try
        {
            if (Match(TokenType.Var))
            {
                return VarDeclaration();
            }
            return Statement();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }

    Stmt VarDeclaration()
    {
        var name = Consume(TokenType.Identifier, "Expect variable name.");
        Expr? initializer = null;
        if (Match(TokenType.Equal))
        {
            initializer = Expression();
        }
        Consume(TokenType.Semicolon, "Expect ';' after variable declaration.");
        return new Stmt.Var(name, initializer);
    }

    Stmt Statement()
    {
        if (Match(TokenType.Print))
        {
            return PrintStatement();
        }
        return ExpressionStatement();
    }

    Stmt PrintStatement()
    {
        Expr value = Expression();
        Consume(TokenType.Semicolon, "Expect ';' after value.");
        return new Stmt.Print(value);
    }

    Stmt ExpressionStatement()
    {
        Expr value = Expression();
        Consume(TokenType.Semicolon, "Expect ';' after value.");
        return new Stmt.Expression(value);
    }

    Expr Expression()
    {
        return Assignment();
    }

    Expr Assignment()
    {
        var expr = Equality();
        if (Match(TokenType.Equal))
        {
            var equal = PreviousToken;
            var value = Assignment();
            if (expr is Expr.Variable)
            {
                var name = ((Expr.Variable)expr).Name;
                return new Expr.Assign(name, value);
            }
            Error(equal, "Invalid assignment target.");
        }
        return expr;
    }

    Expr Equality()
    {
        var expr = Comparison();
        while (Match(TokenType.BangEqual, TokenType.EqualEqual))
        {
            var op = PreviousToken;
            var right = Comparison();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    Expr Comparison()
    {
        var expr = Term();
        while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
        {
            var op = PreviousToken;
            var right = Term();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    Expr Term()
    {
        var expr = Factor();
        while (Match(TokenType.Minus, TokenType.Plus))
        {
            var op = PreviousToken;
            var right = Factor();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    Expr Factor()
    {
        var expr = Unary();
        while (Match(TokenType.Slash, TokenType.Star))
        {
            var op = PreviousToken;
            var right = Unary();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    Expr Unary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            var op = PreviousToken;
            var right = Unary();
            return new Expr.Unary(op, right);
        }
        return Primary();
    }

    Expr Primary()
    {
        if (Match(TokenType.False))
        {
            return new Expr.Literal(false);
        }
        if (Match(TokenType.True))
        {
            return new Expr.Literal(true);
        }
        if (Match(TokenType.Nil))
        {
            return new Expr.Literal(null);
        }
        if (Match(TokenType.Number, TokenType.String))
        {
            return new Expr.Literal(PreviousToken.Literal);
        }
        if (Match(TokenType.LeftParenthesis))
        {
            var expr = Expression();
            Consume(TokenType.RightParenthesis, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }
        if (Match(TokenType.Identifier))
        {
            return new Expr.Variable(PreviousToken);
        }
        throw Error(CurrentToken, "Expect expression.");
    }

    bool Match(params TokenType[] types)
    {
        foreach (var t in types)
        {
            if (Check(t))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    bool Check(TokenType t)
    {
        return IsAtEnd ? false : CurrentToken.Type == t;
    }

    Token Advance()
    {
        if (!IsAtEnd)
        {
            current++;
        }
        return PreviousToken;
    }

    Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }
        throw Error(CurrentToken, message);
    }

    ParseError Error(Token token, string message)
    {
        Program.Error(token, message);
        return new ParseError();
    }

    void Synchronize()
    {
        Advance();

        while (!IsAtEnd)
        {
            if (PreviousToken.Type == TokenType.Semicolon)
            {
                return;
            }

            switch (CurrentToken.Type)
            {
                case TokenType.Class:
                case TokenType.Fun:
                case TokenType.Var:
                case TokenType.For:
                case TokenType.If:
                case TokenType.While:
                case TokenType.Print:
                case TokenType.Return:
                    return;
            }

            Advance();
        }
    }
}
