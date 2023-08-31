using TinyLanguageCompiler.Exceptions;

namespace TinyLanguageCompiler.Compiler;

public static class ExceptionFactory
{
    public static void CreateSyntaxExceptionIf(bool condition)
    {
        if (!condition) return;

        throw new SyntaxException("Invalid token");
    }
}