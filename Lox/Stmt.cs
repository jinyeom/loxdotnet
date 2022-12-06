﻿namespace Lox;

abstract class Stmt
{
    /// <summary>
    /// Visitor interface.
    /// </summary>
    /// <typeparam name="R">Result type.</typeparam>
    internal interface IVisitor<R>
    {
        R Visit(Expression stmt);
        R Visit(Print stmt);
        R Visit(Var stmt);
    }

    /// <summary>
    /// Abstract method for accepting a visitor.
    /// </summary>
    /// <typeparam name="R">Result type.</typeparam>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public abstract R Accept<R>(IVisitor<R> visitor);

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
