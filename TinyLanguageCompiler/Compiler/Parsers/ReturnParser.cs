using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler.Parsers;

public class ReturnParser
{
    private readonly Tokenizer.Tokenizer _tokenizer;

    public ReturnParser(Tokenizer.Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public void ParseReturnStatement()
    {
        if (_tokenizer.CurrentFunction is null) throw new LogicalException("Return keyword is used outside function scope!");

        Token returnKeyword = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(returnKeyword is not { Type: TokenType.ReturnKeyword, Value: "return" });

        ExpressionParser expressionParser = new(_tokenizer);
        Expression returnExpression = expressionParser.ParseExpression();

        Token terminator = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(terminator is not { Type: TokenType.Terminator, Value: ";" });

        if (_tokenizer.CurrentFunction.ReturnType == returnExpression.ExpressionType) return;

        throw new TypeException("Return expression and function return type must be same");
    }
}
