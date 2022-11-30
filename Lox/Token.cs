namespace Lox;

/// <summary>
/// Enumeration of token types used in Lox.
/// </summary>
enum TokenType : byte
{
    // Single-character tokens
    LeftParenthesis,
    RightParenthesis,
    LeftBrace,
    RightBrace,
    Comma,
    Dot,
    Minus,
    Plus,
    Semicolon,
    Slash,
    Star,

    // One or two character tokens
    Bang,
    BangEqual,
    Equal,
    EqualEqual,
    Greater,
    GreaterEqual,
    Less,
    LessEqual,

    // Literals
    Identifier,
    String,
    Number,

    // Keywords
    And,
    Class,
    Else,
    False,
    Fun,
    For,
    If,
    Nil,
    Or,
    Print,
    Return,
    Super,
    This,
    True,
    Var,
    While,

    Eof
}

/// <summary>
/// Read-only record struct type for tokens used during lexing.
/// </summary>
/// <param name="Type"></param>
/// <param name="Lexeme"></param>
/// <param name="Literal"></param>
/// <param name="Line"></param>
readonly record struct Token(TokenType Type, string Lexeme, object? Literal, int Line);
