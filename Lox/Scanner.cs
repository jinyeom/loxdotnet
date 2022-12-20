namespace Lox;

class Scanner
{
    static readonly IDictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        { "and", TokenType.And },
        { "class", TokenType.Class },
        { "else", TokenType.Else },
        { "false", TokenType.False },
        { "for", TokenType.For },
        { "fun", TokenType.Fun },
        { "if", TokenType.If },
        { "nil", TokenType.Nil },
        { "or", TokenType.Or },
        { "print", TokenType.Print },
        { "return", TokenType.Return },
        { "super", TokenType.Super },
        { "this", TokenType.This },
        { "true", TokenType.True },
        { "var", TokenType.Var },
        { "while", TokenType.While },
    };

    readonly string source;
    readonly List<Token> tokens;

    int start = 0;
    int current = 0;
    int line = 1;

    public Scanner(string source)
    {
        this.source = source;
        tokens = new List<Token>();
    }

    bool IsAtEnd { get { return current >= source.Length; } }

    string CurrentText { get { return source[start..current]; } }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd)
        {
            start = current;
            ScanToken();
        }
        tokens.Add(new Token(TokenType.Eof, "", null, line));
        return tokens;
    }

    void ScanToken()
    {
        var currentCharacter = Advance();
        switch (currentCharacter)
        {
            case '(': AddToken(TokenType.LeftParenthesis); break;
            case ')': AddToken(TokenType.RightParenthesis); break;
            case '{': AddToken(TokenType.LeftBrace); break;
            case '}': AddToken(TokenType.RightBrace); break;
            case ',': AddToken(TokenType.Comma); break;
            case '.': AddToken(TokenType.Dot); break;
            case '-': AddToken(TokenType.Minus); break;
            case '+': AddToken(TokenType.Plus); break;
            case ';': AddToken(TokenType.Semicolon); break;
            case '*': AddToken(TokenType.Star); break;
            case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
            case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
            case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
            case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd)
                    {
                        Advance();
                    }
                }
                else
                {
                    AddToken(TokenType.Slash);
                }
                break;
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespaces.
                break;
            case '\n':
                line++;
                break;
            case '"':
                String();
                break;
            default:
                if (IsDigit(currentCharacter))
                {
                    Number();
                }
                else if (IsLetterOrUnderscore(currentCharacter))
                {
                    Identifier();
                }
                else
                {
                    Program.Error(line, $"Unexpected character: {currentCharacter}");
                }
                break;
        }
    }

    bool IsDigit(char c)
    {
        return char.IsDigit(c);
    }

    bool IsLetterOrUnderscore(char c)
    {
        return char.IsLetter(c) || c == '_';
    }

    bool IsAlphaNumericOrUnderscore(char c)
    {
        return IsDigit(c) || IsLetterOrUnderscore(c);
    }

    char Advance()
    {
        return source[current++];
    }

    void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    void AddToken(TokenType type, object? literal)
    {
        var text = CurrentText;
        var token = new Token(type, text, literal, line);
        tokens.Add(token);
    }

    bool Match(char expected)
    {
        if (IsAtEnd)
        {
            return false;
        }
        if (source[current] != expected)
        {
            return false;
        }
        current++;
        return true;
    }

    char Peek()
    {
        if (IsAtEnd)
        {
            return '\0';
        }
        return source[current];
    }

    void String()
    {
        while (!IsAtEnd && Peek() != '"')
        {
            if (Peek() == '\n')
            {
                line++;
            }
            Advance();
        }

        if (IsAtEnd)
        {
            Program.Error(line, "Unterminated string.");
            return;
        }

        // The closing ".
        Advance();

        // Trim the surrounding quotes.
        var first = start + 1;
        var last = current - 1;
        var literal = source[first..last];
        AddToken(TokenType.String, literal);
    }

    void Number()
    {
        var advanceDigits = () =>
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }
        };
        advanceDigits();
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance(); // "."
            advanceDigits();
        }
        var number = Convert.ToDouble(CurrentText);
        AddToken(TokenType.Number, number);
    }

    char PeekNext()
    {
        if (current + 1 >= source.Length)
        {
            return '\0';
        }
        return source[current + 1];
    }

    void Identifier()
    {
        while (IsAlphaNumericOrUnderscore(Peek()))
        {
            Advance();
        }

        var text = CurrentText;
        var type = TokenType.Identifier;
        if (keywords.ContainsKey(text))
        {
            type = keywords[text];
        }
        AddToken(type);
    }
}