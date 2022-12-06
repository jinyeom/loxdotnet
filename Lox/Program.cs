namespace Lox;

using System.Text;

static class Program
{
    const string UsageMessage = "Usage: lox [script]";
    const string PromptSymbol = ">";

    static readonly Interpreter interpreter = new Interpreter();

    static bool hadError = false;
    static bool hadRuntimeError = false;

    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine(UsageMessage);
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            var expression = new Expr.Binary(
                new Expr.Unary(
                    new Token(TokenType.Minus, "-", null, 1),
                    new Expr.Literal(123)),
                new Token(TokenType.Star, "*", null, 1),
                new Expr.Grouping(
                    new Expr.Literal(45.67)));

            Console.WriteLine(new AstPrinter().Print(expression));

            RunPrompt();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    static void RunFile(string path)
    {
        var bytes = File.ReadAllBytes(path);
        // TODO: handle exceptions?
        var script = Encoding.Default.GetString(bytes);
        Run(script);
        if (hadError)
        {
            Environment.Exit(65);
        }
        if (hadRuntimeError)
        {
            Environment.Exit(70);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    static void RunPrompt()
    {
        while (true)
        {
            Console.Write($"{PromptSymbol} ");
            var line = Console.ReadLine();
            if (line == null)
            {
                break;
            }
            Run(line);
            hadError = false;
        }
    }

    static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var stmts = parser.Parse();

        if (hadError)
        {
            return;
        }

        interpreter.Interpret(stmts);
    }

    static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.Eof)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, $"at {token.Lexeme}", message);
        }
    }

    public static void RuntimeError(RuntimeError error)
    {
        Console.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
        hadRuntimeError = true;
    }
}