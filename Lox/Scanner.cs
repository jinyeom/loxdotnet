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

    /// <summary>
    /// Scanner constructor.
    /// </summary>
    /// <param name="source">Source to be scanned.</param>
    public Scanner(string source)
    {
        this.source = source;
        tokens = new List<Token>();
    }

    /// <summary>
    /// Whether the scanner has consumed all characters in the source.
    /// </summary>
    bool IsAtEnd { get { return current >= source.Length; } }

    /// <summary>
    /// Currently selected text in the source code.
    /// </summary>
    string CurrentText { get { return source[start..current]; } }

    /// <summary>
    /// Scan tokens until it reaches the end of file.
    /// </summary>
    /// <returns>List of tokens.</returns>
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

    /// <summary>
    /// Helper method that scans a single token.
    /// </summary>
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

    /// <summary>
    /// Helper method that determines whether the argument character is a digit.
    /// </summary>
    /// <param name="c"></param>
    /// <returns>True if the argument character is a digit.</returns>
    bool IsDigit(char c)
    {
        return char.IsDigit(c);
    }

    /// <summary>
    /// Helper method that determines whether the argument character is a letter or an underscore.
    /// </summary>
    /// <param name="c"></param>
    /// <returns>True if the argument character is a letter or an underscore.</returns>
    bool IsLetterOrUnderscore(char c)
    {
        return char.IsLetter(c) || c == '_';
    }

    /// <summary>
    /// Helper method that determines whether the argument character is alphanumeric (or underscore).
    /// </summary>
    /// <param name="c"></param>
    /// <returns>True if the argument character is alphanumeric or an underscore.</returns>
    bool IsAlphaNumericOrUnderscore(char c)
    {
        return IsDigit(c) || IsLetterOrUnderscore(c);
    }

    /// <summary>
    /// Helper method that returns the current character and advances to the next character in source.
    /// </summary>
    /// <returns>Consumed character before advancing the scanner.</returns>
    char Advance()
    {
        return source[current++];
    }

    /// <summary>
    /// Helper method that adds a new token with the argument token type.
    /// </summary>
    /// <param name="type">Token type.</param>
    void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    /// <summary>
    /// Helper method that adds a new token with the argument token type and literal.
    /// </summary>
    /// <param name="type">Token type.</param>
    /// <param name="literal">Literal object.</param>
    void AddToken(TokenType type, object? literal)
    {
        var text = CurrentText;
        var token = new Token(type, text, literal, line);
        tokens.Add(token);
    }

    /// <summary>
    /// Helper method that advances to the next character and returns true if the current character matches the argument character.
    /// </summary>
    /// <param name="expected">Character to match the current character against.</param>
    /// <returns>True if the current character matches the argument character, false otherwise.</returns>
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

    /// <summary>
    /// Helper method that returns the current character.
    /// </summary>
    /// <returns>Current character; returns a null character if the scanner is at EOF.</returns>
    char Peek()
    {
        if (IsAtEnd)
        {
            return '\0';
        }
        return source[current];
    }

    /// <summary>
    /// Helper method that adds a string literal token.
    /// </summary>
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

    /// <summary>
    /// Helper method that adds a number literal token.
    /// </summary>
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

    /// <summary>
    /// Helper method that returns the next character.
    /// </summary>
    /// <returns>Next character.</returns>
    char PeekNext()
    {
        if (current + 1 >= source.Length)
        {
            return '\0';
        }
        return source[current + 1];
    }

    /// <summary>
    /// Helper method that adds an identifier token.
    /// </summary>
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