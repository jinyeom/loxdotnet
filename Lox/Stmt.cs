namespace Lox;

abstract class Stmt
{
    internal interface IVisitor<R>
    {
        R Visit(Block stmt);
        R Visit(Expression stmt);
        R Visit(Print stmt);
        R Visit(Var stmt);
    }

    public abstract R Accept<R>(IVisitor<R> visitor);

    internal class Block : Stmt
    {
        public Block(IList<Stmt?> statements)
        {
            Statements = statements;
        }

        public IList<Stmt?> Statements { get; init; }

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
}
