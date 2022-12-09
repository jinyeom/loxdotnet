namespace Lox;

class Environment
{
    IDictionary<string, object?> values = new Dictionary<string, object?>();

    public void Define(string name, object? value)
    {
        values[name] = value;
    }

    public object? Get(Token name)
    {
        if (values.ContainsKey(name.Lexeme))
        {
            return values[name.Lexeme];
        }
        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
}
