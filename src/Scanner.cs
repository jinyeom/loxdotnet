namespace Lox;

class Scanner
{
    readonly string source;
    readonly List<Token> tokens = new List<Token>();
    int start = 0;
    int current = 0;
    int line = 1;

    /// <summary>
    /// Helper property that indicates whether the scanner has consumed all characters.
    /// </summary>
    bool IsAtEnd { get => current >= source.Length; }

    /// <summary>
    /// Scanner constructor.
    /// </summary>
    /// <param name="source"></param>
    public Scanner(string source)
    {
        this.source = source;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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
            default:
                // TODO: handle error from a separate class, not Program. 
                Program.Error(line, $"Unexpected character: {currentCharacter}");
                break;
        }
    }

    /// <summary>
    /// Helper method that returns the current character and advances to the next character in source.
    /// </summary>
    /// <returns></returns>
    char Advance()
    {
        return source[current++];
    }

    /// <summary>
    /// Helper method that adds a new token with the argument token type.
    /// </summary>
    /// <param name="type"></param>
    void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    /// <summary>
    /// Helper method that adds a new token with the argument token type and literal.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="literal"></param>
    void AddToken(TokenType type, object? literal)
    {
        var text = source[start..current];
        tokens.Add(new Token(type, text, literal, line));
    }
}