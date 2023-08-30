using TinyLanguageCompiler.Exceptions;

namespace TinyLanguageCompiler.Compiler;

public static class ExceptionFactory
{
    public static void CreateSyntaxExceptionIf(bool condition)
    {
        if (!condition) return;

        throw new SyntaxException("Invalid token");
    }

    public static void CreateLogicalExceptionIf(bool condition, string message)
    {
        if (!condition) return;

        throw new LogicalException(message);
    }
}