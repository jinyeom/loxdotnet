namespace Lox;

abstract class Stmt
{
    internal interface IVisitor<R>
    {
        R Visit(Block stmt);
        R Visit(Expression stmt);
        R Visit(Function stmt);
        R Visit(If stmt);
        R Visit(Print stmt);
        R Visit(Var stmt);
        R Visit(While stmt);
    }

    public abstract R Accept<R>(IVisitor<R> visitor);

    internal class Block : Stmt
    {
        public Block(List<Stmt?> statements)
        {
            Statements = statements;
        }

        public List<Stmt?> Statements { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class Expression : Stmt
    {
        public Expression(Expr expr)
        {
            Expr = expr;
        }

        public Expr Expr { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class Function : Stmt
    {
        public Function(Token name, List<Token> parameters, List<Stmt?> body)
        {
            Name = name;
            Parameters = parameters;
            Body = body;
        }

        public Token Name { get; init; }
        public List<Token> Parameters { get; init; }
        public List<Stmt?> Body { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class If : Stmt
    {
        public If(Expr condition, Stmt thenStatement, Stmt? elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseStatement = elseStatement;
        }

        public Expr Condition { get; init; }
        public Stmt ThenStatement { get; init; }
        public Stmt? ElseStatement { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class Print : Stmt
    {
        public Print(Expr expr)
        {
            Expr = expr;
        }

        public Expr Expr { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class Var : Stmt
    {
        public Var(Token name, Expr? initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public Token Name { get; init; }
        public Expr? Initializer { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class While : Stmt
    {
        public While(Expr condition, Stmt body)
        {
            Condition = condition;
            Body = body;
        }

        public Expr Condition { get; init; }
        public Stmt Body { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
