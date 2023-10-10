namespace TinyLanguageCompiler.Exceptions;

public class SyntaxException : Exception
{
    public SyntaxException(string message) : base(message)
    {
        Console.WriteLine(message);

        Environment.Exit(1);
    }
}
