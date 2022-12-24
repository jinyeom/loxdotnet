namespace Lox;

class NativeFunctions
{
    internal class Clock : ICallable
    {
        public int Arity()
        {
            return 0;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public override string ToString()
        {
            return "<native fn>";
        }
    }
}