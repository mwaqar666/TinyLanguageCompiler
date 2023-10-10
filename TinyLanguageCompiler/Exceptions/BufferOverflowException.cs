namespace TinyLanguageCompiler.Exceptions;

public class BufferOverflowException : Exception
{
    public BufferOverflowException(string message) : base($"Buffer Overflow: {message}")
    {
        Console.WriteLine($"Buffer Overflow: {message}");

        Environment.Exit(1);
    }
}
