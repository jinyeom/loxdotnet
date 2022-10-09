namespace Lox;

using System.Text;

static class Program
{
    const string UsageMessage = "Usage: lox [script]";
    const string PromptSymbol = ">";

    static bool hadError = false;

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

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
    }
}