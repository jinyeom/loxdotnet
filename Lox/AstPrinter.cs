using System.Text;

namespace Lox;

class AstPrinter : Expr.IVisitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    public string Visit(Expr.Binary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
    }

    public string Visit(Expr.Assign expr)
    {
        return $"{expr.Name} = {expr.Value}";
    }

    public string Visit(Expr.Grouping expr)
    {
        return Parenthesize("group", expr.Expression);
    }

    public string Visit(Expr.Literal expr)
    {
        return expr.Value == null ? "nil" : expr.Value.ToString()!;
    }

    public string Visit(Expr.Unary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Right);
    }

    public string Visit(Expr.Variable expr)
    {
        return $"var {expr.Name}";
    }

    string Parenthesize(string name, params Expr[] exprs)
    {
        var builder = new StringBuilder($"({name}");
        foreach (var expr in exprs)
        {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        }
        builder.Append(")");
        return builder.ToString();
    }
}