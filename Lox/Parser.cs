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

    /// <summary>
    /// Whether the current token is EOF.
    /// </summary>
    bool IsAtEnd { get { return CurrentToken.Type == TokenType.Eof; } }

    /// <summary>
    /// Current token.
    /// </summary>
    Token CurrentToken { get { return tokens[current]; } }

    /// <summary>
    /// Previous token.
    /// </summary>
    Token PreviousToken { get { return tokens[current - 1]; } }

    public Expr? Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseError)
        {
            return null;
        }
    }

    /// <summary>
    /// Helper method that expands Expression.
    /// </summary>
    /// <returns></returns>
    Expr Expression()
    {
        return Equality();
    }

    /// <summary>
    /// Helper method that expands Equality.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Helper method that expands Comparison.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Helper method that expands Term.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Helper method that expands Factor.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Helper method that expands Unary.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Helper method that expands Primary.
    /// </summary>
    /// <returns></returns>
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
        throw Error(CurrentToken, "Expect expression.");
    }

    /// <summary>
    /// Helper method that determines whether the current token has any of the given type.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Helper method that checks if the current token is of the argument type.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    bool Check(TokenType t)
    {
        return IsAtEnd ? false : CurrentToken.Type == t;
    }

    /// <summary>
    /// Helper method that advances the token pointer.
    /// </summary>
    /// <returns>Current token before advancing.</returns>
    Token Advance()
    {
        if (!IsAtEnd)
        {
            current++;
        }
        return PreviousToken;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }
        throw Error(CurrentToken, message);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    ParseError Error(Token token, string message)
    {
        Program.Error(token, message);
        return new ParseError();
    }

    /// <summary>
    /// Helper method that discards tokens until the beginning of the next statement.
    /// </summary>
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
