namespace Lox;

class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    public void Interpret(IList<Stmt> statements)
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

    public object? Visit(Expr.Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object? Visit(Expr.Literal expr)
    {
        return expr.Value;
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

    public object? Visit(Stmt.Expression stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object? Visit(Stmt.Print stmt)
    {
        var value = Evaluate(stmt.Expr);
        Console.WriteLine(Stringify(value));
        return null;
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
