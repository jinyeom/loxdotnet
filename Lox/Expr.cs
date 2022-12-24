namespace Lox;

abstract class Expr
{
    internal interface IVisitor<R>
    {
        R Visit(Binary expr);
        R Visit(Call expr);
        R Visit(Assign expr);
        R Visit(Grouping expr);
        R Visit(Literal expr);
        R Visit(Logical expr);
        R Visit(Unary expr);
        R Visit(Variable expr);
    }

    public abstract R Accept<R>(IVisitor<R> visitor);

    internal class Binary : Expr
    {
        public Binary(Expr left, Token op, Expr right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        public Expr Left { get; init; }
        public Token Op { get; init; }
        public Expr Right { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class Call : Expr
    {
        public Call(Expr callee, Token paren, List<Expr> arguments)
        {
            Callee = callee;
            Paren = paren;
            Arguments = arguments;
        }

        public Expr Callee { get; init; }
        public Token Paren { get; init; }
        public List<Expr> Arguments { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class Assign : Expr
    {
        public Assign(Token name, Expr value)
        {
            Name = name;
            Value = value;
        }

        public Token Name { get; init; }
        public Expr Value { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class Grouping : Expr
    {
        public Grouping(Expr expression)
        {
            Expression = expression;
        }

        public Expr Expression { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class Literal : Expr
    {
        public Literal(object? value)
        {
            Value = value;
        }

        public object? Value { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class Logical : Expr
    {
        public Logical(Expr left, Token op, Expr right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        public Expr Left { get; init; }
        public Token Op { get; init; }
        public Expr Right { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class Unary : Expr
    {
        public Unary(Token op, Expr right)
        {
            Op = op;
            Right = right;
        }

        public Token Op { get; init; }
        public Expr Right { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class Variable : Expr
    {
        public Variable(Token name)
        {
            Name = name;
        }

        public Token Name { get; init; }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.Visit(this);
        }
    }
}