namespace Lox;

class Resolver : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    readonly Interpreter interpreter;
    readonly Stack<Dictionary<string, bool>> scopes;

    public Resolver(Interpreter interpreter)
    {
        this.interpreter = interpreter;
        this.scopes = new Stack<Dictionary<string, bool>>();
    }

    object? Visit(Stmt.Block stmt)
    {
        BeginScope();
        Resolve(stmt.Statements);
        EndScope();
        return null;
    }

    object? Visit(Stmt.Var stmt)
    {
        Declare(stmt.Name);
        if (stmt.Initializer != null)
        {
            Resolve(stmt.Initializer);
        }
        Define(stmt.Name);
        return null;
    }

    object? Visit(Expr.Variable expr)
    {
        if (scopes.Count == 0)
        {
            return null;
        }
        if (scopes.Peek().TryGetValue(expr.Name.Lexeme, out bool defined) && !defined)
        {
            Program.Error(expr.Name, "Can't read local variable in its own initializer.");
        }
        ResolveLocal(expr, expr.Name);
        return null;
    }

    object? Visit(Expr.Assign expr)
    {
        Resolve(expr.Value);
        ResolveLocal(expr, expr.Name);
        return null;
    }

    object? Visit(Stmt.Function stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);
        ResolveFunction(stmt);
        return null;
    }

    void ResolveFunction(Stmt.Function function)
    {
        BeginScope();
        foreach (var parameter in function.Parameters)
        {
            Declare(parameter);
            Define(parameter);
        }
        Resolve(function.Body);
        EndScope();
    }

    void ResolveLocal(Expr expr, Token name)
    {
        var steps = 0;
        foreach (var scope in scopes)
        {
            if (scope.ContainsKey(name.Lexeme))
            {
                interpreter.Resolve(expr, steps);
                return;
            }
            steps++;
        }
    }

    void BeginScope()
    {
        scopes.Push(new Dictionary<string, bool>());
    }

    void EndScope()
    {
        scopes.Pop();
    }

    void Declare(Token name)
    {
        if (scopes.Count == 0)
        {
            return;
        }
        var scope = scopes.Peek();
        scope[name.Lexeme] = false;
    }

    void Define(Token name)
    {
        if (scopes.Count == 0)
        {
            return;
        }
        var scope = scopes.Peek();
        scope[name.Lexeme] = true;
    }

    void Resolve(List<Stmt?> statements)
    {
        foreach (var statement in statements)
        {
            Resolve(statement);
        }
    }

    void Resolve(Stmt? statement)
    {
        if (statement == null)
        {
            return;
        }
        statement.Accept(this);
    }

    void Resolve(Expr? expression)
    {
        if (expression == null)
        {
            return;
        }
        expression.Accept(this);
    }
}