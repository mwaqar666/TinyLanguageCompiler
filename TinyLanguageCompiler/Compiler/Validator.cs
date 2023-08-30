using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler;

public class Validator
{
    private readonly List<Token> _tokens;

    public Validator(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public void ValidateTokenStream()
    {
        foreach (Token token in _tokens)
        {
            // token;
        }
    }
}