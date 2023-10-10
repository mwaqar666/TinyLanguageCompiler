namespace TinyLanguageCompiler.Exceptions;

public class LogicalException : Exception
{
    public LogicalException(string message) : base(message)
    {
        Console.WriteLine(message);

        Environment.Exit(1);
    }
}
