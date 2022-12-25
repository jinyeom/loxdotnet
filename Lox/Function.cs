namespace Lox;

class Function : ICallable
{
    readonly Stmt.Function declaration;
    readonly Environment closure;

    public Function(Stmt.Function declaration, Environment closure)
    {
        this.declaration = declaration;
        this.closure = closure;
    }

    public int Arity()
    {
        return this.declaration.Parameters.Count();
    }

    public object? Call(Interpreter interpreter, List<object?> arguments)
    {
        var environment = new Environment(closure);
        for (var i = 0; i < declaration.Parameters.Count; i++)
        {
            var parameter = declaration.Parameters[i].Lexeme;
            var argument = arguments[i];
            environment.Define(parameter, argument);
        }
        try
        {
            interpreter.ExecuteBlock(declaration.Body, environment);
        }
        catch (Return returnValue)
        {
            return returnValue.Value;
        }
        return null;
    }

    public override string ToString()
    {
        return $"<fn {declaration.Name.Lexeme}>";
    }
}