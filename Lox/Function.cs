namespace Lox;

class Function : ICallable
{
    readonly Stmt.Function declaration;

    public Function(Stmt.Function declaration)
    {
        this.declaration = declaration;
    }

    public int Arity()
    {
        return this.declaration.Parameters.Count();
    }

    public object? Call(Interpreter interpreter, List<object?> arguments)
    {
        var environment = new Environment(interpreter.Globals);
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