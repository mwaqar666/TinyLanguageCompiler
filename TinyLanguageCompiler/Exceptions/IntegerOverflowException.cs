namespace TinyLanguageCompiler.Exceptions;

public class IntegerOverflowException : Exception
{
    public IntegerOverflowException(string message) : base($"Integer Overflow: {message}")
    {
        Console.WriteLine($"Integer Overflow: {message}");

        Environment.Exit(1);
    }
}
