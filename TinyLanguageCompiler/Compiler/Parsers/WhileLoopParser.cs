using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler.Parsers;

public class WhileLoopParser
{
    private readonly Tokenizer.Tokenizer _tokenizer;

    public WhileLoopParser(Tokenizer.Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public void ParseWhileLoopStatement()
    {
        _tokenizer.SymbolTable.IncrementCodeBlockLevel();

        Token whileKeyword = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(whileKeyword is not { Type: TokenType.WhileKeyword, Value: "while" });

        Token delimiterStart = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(delimiterStart is not { Type: TokenType.Delimiter, Value: "(" });

        ComparisonParser comparisonParser = new(_tokenizer);
        comparisonParser.ParseComparison();

        Token delimiterEnd = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(delimiterEnd is not { Type: TokenType.Delimiter, Value: ")" });

        Token bodyStart = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(bodyStart is not { Type: TokenType.Delimiter, Value: "{" });

        ParseWhileLoopBody();

        Token bodyEnd = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(bodyEnd is not { Type: TokenType.Delimiter, Value: "}" });

        _tokenizer.SymbolTable.DecrementCodeBlockLevel();
    }

    private void ParseWhileLoopBody()
    {
        Token possibleBodyBracketEnd = _tokenizer.NextToken();
        _tokenizer.PreviousToken();

        if (possibleBodyBracketEnd is { Type: TokenType.Delimiter, Value: "}" }) return;

        StatementParser statementParser = new(_tokenizer);

        while (true)
        {
            statementParser.ParseStatement();

            possibleBodyBracketEnd = _tokenizer.NextToken();
            _tokenizer.PreviousToken();

            if (possibleBodyBracketEnd is not { Type: TokenType.Delimiter, Value: "}" }) continue;

            break;
        }
    }
}
