﻿namespace Lox;

/// <summary>
/// Expression.
/// </summary>
abstract class Expr
{
    /// <summary>
    /// Visitor interface.
    /// </summary>
    /// <typeparam name="R">Result type.</typeparam>
    internal interface IVisitor<R>
    {
        R Visit(Binary expr);
        R Visit(Grouping expr);
        R Visit(Literal expr);
        R Visit(Unary expr);
    }

    /// <summary>
    /// Abstract method for accepting a visitor.
    /// </summary>
    /// <typeparam name="R">Result type.</typeparam>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public abstract R Accept<R>(IVisitor<R> visitor);

    /// <summary>
    /// Binary expression.
    /// </summary>
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

    /// <summary>
    /// Grouping expression.
    /// </summary>
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

    /// <summary>
    /// Literal expression.
    /// </summary>
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

    /// <summary>
    /// Unary expression.
    /// </summary>
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
}