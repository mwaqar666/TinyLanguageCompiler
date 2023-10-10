namespace TinyLanguageCompiler.Exceptions;

public class TypeException : Exception
{
    public TypeException(string message) : base($"Type mismatch exception: {message}")
    {
        Console.WriteLine($"Type mismatch exception: {message}");

        Environment.Exit(1);
    }
}
