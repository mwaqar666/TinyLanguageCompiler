using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler;

public class ExpressionEvaluator
{
    private readonly Function _currentFunction;
    private readonly Tokenizer _tokenizer;


    public ExpressionEvaluator(Tokenizer tokenizer, Function functionContext)
    {
        _tokenizer = tokenizer;
        _currentFunction = functionContext;
    }
}
