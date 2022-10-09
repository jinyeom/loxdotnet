namespace Lox;

/// <summary>
/// Read-only record struct type for tokens used during lexing.
/// </summary>
/// <param name="Type"></param>
/// <param name="Lexeme"></param>
/// <param name="Literal"></param>
/// <param name="line"></param>
readonly record struct Token(TokenType Type, string Lexeme, object? Literal, int line);