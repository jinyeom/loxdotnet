namespace Lox;

class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    Environment environment;

    public Interpreter()
    {
        Globals = new Environment();
        Globals.Define("clock", new NativeFunctions.Clock());

        environment = Globals;
    }

    public Environment Globals { get; init; }

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError error)
        {
            Program.RuntimeError(error);
        }
    }

    void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }

    string? Stringify(object? value)
    {
        if (value == null)
        {
            return "nil";
        }
        if (value is double)
        {
            var text = value.ToString()!;
            return text.EndsWith(".0") ? text[..^2] : text;
        }
        return value.ToString();
    }

    public object? Visit(Expr.Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);
        switch (expr.Op.Type)
        {
            case TokenType.Greater:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left > (double)right;
            case TokenType.GreaterEqual:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left >= (double)right;
            case TokenType.Less:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left < (double)right;
            case TokenType.LessEqual:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left <= (double)right;
            case TokenType.EqualEqual:
                CheckNumberOperands(expr.Op, left, right);
                return IsEqual(left, right);
            case TokenType.BangEqual:
                CheckNumberOperands(expr.Op, left, right);
                return !IsEqual(left, right);
            case TokenType.Plus:
                if (left is double && right is double)
                {
                    return (double)left + (double)right;
                }
                else if (left is string && right is string)
                {
                    return (string)left + (string)right;
                }
                throw new RuntimeError(expr.Op, "Operands must be two numbers or two strings.");
            case TokenType.Minus:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left - (double)right;
            case TokenType.Slash:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left / (double)right;
            case TokenType.Star:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left * (double)right;
        }
        // Should be unreachable.
        return null;
    }

    public object? Visit(Expr.Call expr)
    {
        var callee = Evaluate(expr.Callee);
        var arguments = new List<object?>();
        foreach (var argument in expr.Arguments)
        {
            arguments.Add(Evaluate(argument));
        }
        if (!(callee is ICallable))
        {
            throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
        }
        var function = (ICallable)callee;
        if (arguments.Count != function.Arity())
        {
            throw new RuntimeError(expr.Paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
        }
        return function.Call(this, arguments);
    }

    public object? Visit(Expr.Assign expr)
    {
        var value = Evaluate(expr.Value);
        environment.Assign(expr.Name, value);
        return value;
    }

    public object? Visit(Expr.Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object? Visit(Expr.Literal expr)
    {
        return expr.Value;
    }

    public object? Visit(Expr.Logical expr)
    {
        var left = Evaluate(expr.Left);
        if (expr.Op.Type == TokenType.Or)
        {
            if (IsTruthy(left))
            {
                return left;
            }
        }
        else
        {
            if (!IsTruthy(left))
            {
                return left;
            }
        }
        return Evaluate(expr.Right);
    }

    public object? Visit(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right);
        switch (expr.Op.Type)
        {
            case TokenType.Bang:
                return !IsTruthy(right);
            case TokenType.Minus:
                CheckNumberOperand(expr.Op, right);
                return -(double)right;
        }
        // Should be unreachable.
        return null;
    }

    public object? Visit(Expr.Variable expr)
    {
        return environment.Get(expr.Name);
    }

    public object? Visit(Stmt.Expression stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object? Visit(Stmt.Function stmt)
    {
        var function = new Function(stmt);
        environment.Define(stmt.Name.Lexeme, function);
        return null;
    }

    public object? Visit(Stmt.If stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.ThenStatement);
        }
        else if (stmt.ElseStatement != null)
        {
            Execute(stmt.ElseStatement);
        }
        return null;
    }

    public object? Visit(Stmt.Print stmt)
    {
        var value = Evaluate(stmt.Expr);
        Console.WriteLine(Stringify(value));
        return null;
    }

    public object? Visit(Stmt.Return stmt)
    {
        object? value = null;
        if (stmt.Value != null)
        {
            value = Evaluate(stmt.Value);
        }
        throw new Return(value);
    }

    public object? Visit(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(environment));
        return null;
    }

    public object? Visit(Stmt.Var stmt)
    {
        object? value = null;
        if (stmt.Initializer != null)
        {
            value = Evaluate(stmt.Initializer);
        }
        environment.Define(stmt.Name.Lexeme, value);
        return null;
    }

    public object? Visit(Stmt.While stmt)
    {
        while (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.Body);
        }
        return null;
    }

    public void ExecuteBlock(List<Stmt?> statements, Environment environment)
    {
        var previous = this.environment;
        try
        {
            this.environment = environment;
            foreach (var statement in statements)
            {
                if (statement != null)
                {
                    Execute(statement);
                }
            }
        }
        finally
        {
            this.environment = previous;
        }
    }

    object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    static bool IsTruthy(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj is bool)
        {
            return (bool)obj;
        }
        return true;
    }

    static bool IsEqual(object? a, object? b)
    {
        if (a == null && b == null)
        {
            return true;
        }
        if (a == null)
        {
            return false;
        }
        return a.Equals(b);
    }

    void CheckNumberOperand(Token op, object? operand)
    {
        if (!(operand is double))
        {
            throw new RuntimeError(op, "Operand must be a number.");
        }
    }

    void CheckNumberOperands(Token op, object? left, object? right)
    {
        if (!(left is double && right is double))
        {
            throw new RuntimeError(op, "Operands must be numbers.");
        }
    }
}
