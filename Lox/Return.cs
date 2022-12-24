namespace Lox;

class Return : Exception
{
    public Return(object? value)
    {
        Value = value;
    }

    public object? Value { get; init; }
}
