namespace TinyLanguageCompiler.Exceptions;

public class FunctionNotFoundException : Exception
{
    public FunctionNotFoundException(string message) : base($"Function not found: {message}")
    {
        Console.WriteLine($"Function not found: {message}");

        Environment.Exit(1);
    }
}
