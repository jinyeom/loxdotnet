namespace Lox;

interface ICallable
{
    public int Arity();
    public object? Call(Interpreter interpreter, List<object?> arguments);
}